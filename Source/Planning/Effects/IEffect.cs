namespace EGoap.Source.Planning.Effects
{
    // Interface for value changing effects for world state symbols
    public interface IEffect
    {
        SymbolId SymbolId { get; }
        // Value to assing
        int? ValueAssigned { get; }
        // Amount of value change
        int? ValueDelta { get; }
        // World state after the change is applied
        WorldState ApplyTo(WorldState initialState);
    }
}