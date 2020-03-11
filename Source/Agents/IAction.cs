namespace EGoap.Source.Agents
{
    //Interface for actions
    public interface IAction
    {
        ExecutionStatus Status { get; }

        void StartExecution();
        void StartInterruption();

        void Update();
    }
}