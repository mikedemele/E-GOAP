namespace EGoap.Source.Utils.Time
{
    public interface ITimer
    {
        float ElapsedSeconds { get; }
        bool IsPaused { get; }

        bool Resume();
        bool Pause();
    }
}