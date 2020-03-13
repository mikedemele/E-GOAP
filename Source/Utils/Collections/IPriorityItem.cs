namespace EGoap.Source.Utils.Collections
{
    public interface IPriorityItem<out TPriority>
    {
        TPriority GetPriority();
    }
}