using System.Collections.Generic;

namespace EGoap.Source.Planning
{
    // Interface for planners
    public interface IPlanner
    {
        Plan FormulatePlan(IKnowledgeProvider knowledgeProvider, HashSet<PlanningAction> availableActions,
            Goal goal);
        
        // Delete all learned experience
        void ClearExperience();
    }
}