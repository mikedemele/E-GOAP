using System.Collections.Generic;

using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
	//Interface for goal selector, that decides the most relevant goal
    public interface IGoalSelector
    {
	    //All currently relevant goals (relevance > 0),
	    //sorted from most to least relevant.
	    IEnumerable<Goal> RelevantGoals { get; }
	    
	    //Goal to use when no relevant goals are available  or no plan could be found
	    Goal FallbackGoal { get; }

        void ForceReevaluation();
    }
}