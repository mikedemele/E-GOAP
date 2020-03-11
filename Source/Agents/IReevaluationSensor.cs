namespace EGoap.Source.Agents
{
    // Interface for reevaluation sensor
    public interface IReevaluationSensor
    {
        bool IsReevaluationNeeded { get; }
    }
}