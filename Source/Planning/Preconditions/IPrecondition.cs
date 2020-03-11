namespace EGoap.Source.Planning.Preconditions
{
    // Interface for preconditions
    public interface IPrecondition
    {
        // Returns a new IPrecondition, based on this one, whose requirements
        // are adjusted based on the given effect.
        SymbolId SymbolId { get; }

        ValueRange AsValueRange { get; }
        bool IsSatisfiable { get; }
        bool IsSatisfiedBy(WorldState worldState);

        double GetDistanceFrom(WorldState worldState);

        string ToString();
    }
}