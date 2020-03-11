using System.Diagnostics;

namespace EGoap.Source.Utils.Time
{
    /**
	 * This class relies on System.Diagnostics.Stopwatch and allows to measure execution times.
	 */
    public class StopwatchExecutionTimer : ITimer
    {
        public StopwatchExecutionTimer(bool startPaused = false)
        {
            StartTime = CurrentTime;
            AccumulatedTime = 0;
            IsPaused = startPaused;
        }

        protected long StartTime { get; set; }

        protected long AccumulatedTime { get; set; }

        protected static long CurrentTime => Stopwatch.GetTimestamp();

        public bool Resume()
        {
            if (IsPaused)
            {
                StartTime = CurrentTime;
                IsPaused = false;
                return true;
            }

            return false;
        }

        public bool Pause()
        {
            if (!IsPaused)
            {
                AccumulatedTime += CurrentTime - StartTime;
                IsPaused = true;
                return true;
            }

            return false;
        }

        public float ElapsedSeconds =>
        (
            IsPaused
                ? AccumulatedTime
                : AccumulatedTime + (CurrentTime - StartTime)
        ) / (float) Stopwatch.Frequency;

        public bool IsPaused { get; protected set; }
    }
}