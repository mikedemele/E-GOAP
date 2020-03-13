using System;
using System.Collections.Generic;
using System.Linq;

using EGoap.Source.Debug;
using EGoap.Source.Graphs;
using EGoap.Source.Planning.Internal;
using EGoap.Source.Utils.Time;

namespace EGoap.Source.Planning
{
    // Planner using forward search, that can learn from experience
    public class ForwardPlanner : IPlanner
    {
        private const int DefaultMaxPlanLength = 10;
        private const float DefaultSearchTimeSeconds = 1.0f;
        private const double DefaultExperienceWeight = 1;

        // Only raw single action experience actions
        private readonly HashSet<ExperienceAction> baseExperienceActions;
        // All known experience actions
        private readonly HashSet<ExperienceAction> experienceActions;
        // Experience graph
        private readonly Dictionary<WorldState, HashSet<ExperienceAction>> experienceGraph;

        // Pathfinder parameters
        private readonly IPathfinder<ForwardNode> pathfinder;
        private readonly int maxPlanLength;
        
        // Learning parameters
        private readonly double experienceWeight;
        private readonly bool learning;

        public ForwardPlanner(int maxPlanLength = DefaultMaxPlanLength,
            float searchTimeSeconds = DefaultSearchTimeSeconds, bool learn = false,
            double experienceWeight = DefaultExperienceWeight, HashSet<ExperienceAction> experience = null)
        {
            this.maxPlanLength = maxPlanLength;
            learning = learn;
            this.experienceWeight = experienceWeight;
            baseExperienceActions = new HashSet<ExperienceAction>();
            if (experience != null)
            {
                experienceActions = experience;
                foreach (var action in experience.Where(action => action.Actions.Count == 1))
                    baseExperienceActions.Add(action);
            }
            else
            {
                experienceActions = new HashSet<ExperienceAction>();
            }

            experienceGraph = new Dictionary<WorldState, HashSet<ExperienceAction>>();
            
            if (maxPlanLength <= 0)
                throw new ArgumentException("maxPlanLength must be positive", nameof(maxPlanLength));
            pathfinder = new AStarPathfinder<ForwardNode>(
                new AStarPathfinderConfiguration<ForwardNode>.Builder()
                    .UseHeuristic(PathfindingHeuristic)
                    .AssumeNonNegativeCosts()
                    .LimitSearchDepth(maxPlanLength)
                    .LimitSearchTime(searchTimeSeconds)
                    .Build()
            );
        }

        #region IPlanner implementation

        public Plan FormulatePlan(
            IKnowledgeProvider knowledgeProvider,
            HashSet<PlanningAction> availableActions,
            Goal goal
        )
        {
            var timer = new StopwatchExecutionTimer();
            var allActions = new HashSet<PlanningAction>();
            allActions.UnionWith(availableActions);
            allActions.UnionWith(experienceActions.Where(experienceAction =>
                availableActions.IsSupersetOf(experienceAction.Actions)));

            var initialWorldState = new RelevantSymbolsPopulator(allActions, goal)
                .PopulateWorldState(knowledgeProvider);

            try
            {
                var path = pathfinder.FindPath(
                    ForwardNode.MakeRegularNode(initialWorldState, new ForwardNodeExpander(allActions)),
                    ForwardNode.MakeGoalNode(goal)
                );

                if (learning)
                {
                    var beforeLearning = timer.ElapsedSeconds;
                    Learn(path);
                    UnityEngine.Debug.Log(
                        $"Learning time: {timer.ElapsedSeconds-beforeLearning}");
                }

                return new Plan(from edge in path.Edges select ((ForwardEdge) edge).Action, goal);
            }
            catch (PathNotFoundException e)
            {
                throw new PlanNotFoundException(this, maxPlanLength, goal, e);
            }
        }

        #endregion

        private double PathfindingHeuristic(ForwardNode sourceNode, ForwardNode targetNode)
        {
            DebugUtils.Assert(!sourceNode.IsGoal, "sourceNode must be a regular (non-goal) {0}", typeof(ForwardNode));
            DebugUtils.Assert(targetNode.IsGoal, "targetNode must be a goal {0}", typeof(ForwardNode));

            return targetNode.Goal.GetDistanceFrom(sourceNode.WorldState) * experienceWeight;
        }

        private void Learn(Path<ForwardNode> path)
        {
            var learned = false;
            // Only update shortcuts, when new ExperienceActions have been learned
            foreach (var edge in path.Edges)
            {
                if (!edge.GetAction().IsExperience)
                {
                    var state = edge.SourceNode.WorldState;
                    var action = edge.GetAction();
                    var experienceAction = new ExperienceAction(state, action);
                    if (baseExperienceActions.Add(experienceAction))
                    {
                        learned = true;
                        if (experienceGraph.TryGetValue(state, out var value))
                            learned = value.Add(experienceAction);
                        else
                            experienceGraph.Add(state, new HashSet<ExperienceAction> {experienceAction});

                        if (!experienceGraph.ContainsKey(experienceAction.TargetState))
                             experienceGraph.Add(experienceAction.TargetState, new HashSet<ExperienceAction>());
                    }
                }
            }

            if (learned)
            {
                CalculateShortcuts();
            }
        }

        // Parse experience graph for shortcuts and 
        private void CalculateShortcuts()
        {
            var timer = new ResettableStopwatchExecutionTimer();
            // Floyd-Warshall
            // Initialize Matrix
            var size = experienceGraph.Count;
            var graph = new double[size, size];
            var actionList = new List<ExperienceAction>[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var initialAction = InitializeExperienceGraphMatrixNode(i, j);
                    var initialActionList = new List<ExperienceAction>();
                    double initialCost;
                    if (initialAction != null)
                    {
                        initialActionList.Add(initialAction);
                        initialCost = initialAction.Cost;
                    }
                    else
                    {
                        initialCost = double.PositiveInfinity;
                    }

                    graph[i, j] = initialCost;
                    actionList[i, j] = initialActionList;
                }
            }

            // Calculate shortcuts
            for (var k = 0; k < size; k++)
            {
                for (var i = 0; i < size; i++)
                {
                    for (var j = 0; j < size; j++)
                    {
                        if (graph[i, k] + graph[k, j] < graph[i, j])
                        {
                            graph[i, j] = graph[i, k] + graph[k, j];
                            actionList[i, j] = actionList[i, k];
                            actionList[i, j].AddRange(new List<ExperienceAction>(actionList[k, j]));
                        }
                    }
                }
            }

            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {

                    if (graph[i, j] < double.PositiveInfinity)
                    {
                        var nextAct = actionList[i, j];
                        var baseActionList = new List<PlanningAction>();
                        foreach (var expAction in nextAct) baseActionList.AddRange(expAction.Actions);

                        var shortcut = new ExperienceAction(nextAct.First().StartState,
                            baseActionList.ToArray());

                        // if (experienceActions.Add(shortcut))
                        //     UnityEngine.Debug.LogWarning("New shortcut action: " + shortcut);
                        experienceActions.Add(shortcut);
                    }
                }
            }
            UnityEngine.Debug.Log($"Time calculating shortcuts: {timer.ElapsedSeconds}");
        }

        public void ClearExperience()
        {
            experienceActions.Clear();
            experienceGraph.Clear();
        }

        // Initialize a state's edges for the matrix representation used by LearnShortcuts 
        private ExperienceAction InitializeExperienceGraphMatrixNode(int i, int j)
        {
            if (i != j)
            {
                var actions = (from action in experienceGraph.ElementAt(i).Value
                    let targetIndex = experienceGraph.Keys.ToList().IndexOf(action.TargetState)
                    where targetIndex == j
                    select action).ToList();
                if (actions.Any())
                {
                    if (actions.Count > 1)
                    {
                        var min = actions.Min(action => action.Cost);
                        return actions.First(action => action.Cost.Equals(min));
                    }

                    return actions.First();
                }
            }
            return null;
        }
    }
}