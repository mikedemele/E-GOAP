namespace EGoap.Source.Planning.Internal
{
    internal interface IRegressiveStatePopulator
    {
        RegressiveState Populate(Goal goal, RegressiveState initialState = default);
    }
}