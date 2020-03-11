namespace EGoap.Source.Agents
{
    public sealed class NullReevaluationSensor : IReevaluationSensor
    {
        #region IReevaluationSensor implementation

        public bool IsReevaluationNeeded => false;

        #endregion
    }
}