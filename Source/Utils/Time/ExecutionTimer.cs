namespace EGoap.Source.Utils.Time
{
	/**
	 * This class relies on UnityEngine.Time and allows to measure execution times.
	 */
	public class ExecutionTimer : ITimer
	{
		private float startTime;
		private float accumulatedTime;
		private bool isPaused;
		
		public ExecutionTimer()
			: this(false)
		{

		}
		
		public ExecutionTimer(bool startPaused)
		{
			StartTime = CurrentTime;
			AccumulatedTime = 0;
			IsPaused = startPaused;
		}

		public bool Resume() 
		{
			if(IsPaused)
			{
				StartTime = CurrentTime;
				IsPaused = false;
				return true;
			} 
			else 
			{
				return false;
			}
		}

		public bool Pause()
		{
			if(!IsPaused) 
			{
				AccumulatedTime += CurrentTime - startTime;
				IsPaused = true;
				return true;
			} 
			else 
			{
				return false;
			}
		}

		public float ElapsedSeconds
		{
			get {
				return IsPaused 
					? AccumulatedTime 
					: AccumulatedTime + (CurrentTime - StartTime);
			}
		}

		public bool IsPaused
		{
			get {
				return isPaused;
			}
			protected set {
				isPaused = value;
			}
		}

		protected float StartTime
		{
			get {
				return startTime;
			}
			set {
				startTime = value;
			}
		}

		protected float AccumulatedTime
		{
			get {
				return accumulatedTime;
			}
			set {
				accumulatedTime = value;
			}
		}

		protected static float CurrentTime
		{
			get {
				return UnityEngine.Time.unscaledTime;
			}
		}
	}
}

