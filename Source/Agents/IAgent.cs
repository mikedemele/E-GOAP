using EGoap.Source.Planning;

namespace EGoap.Source.Agents
{
    //Interface for agents
    public interface IAgent
    {
        void Update();

        Goal CurrentGoal {get;}
    }
}