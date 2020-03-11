namespace EGoap.Source.Utils.Time
{
    public interface IResettableTimer : ITimer
    {
        void Reset(bool startPaused = true);
    }
}