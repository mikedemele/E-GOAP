namespace EGoap.Source.Utils.Time
{
    public class ResettableStopwatchExecutionTimer : StopwatchExecutionTimer, IResettableTimer
    {
        public ResettableStopwatchExecutionTimer()
        {
        }

        public ResettableStopwatchExecutionTimer(bool startPaused) : base(startPaused)
        {
        }

        #region IResettableTimer implementation

        public void Reset(bool startPaused = true)
        {
            AccumulatedTime = 0;
            StartTime = CurrentTime;
            IsPaused = startPaused;
        }

        #endregion
    }
}