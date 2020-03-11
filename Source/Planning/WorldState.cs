using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EGoap.Source.Debug;
using EGoap.Source.Planning.Effects;
using EGoap.Source.Planning.Preconditions;

namespace EGoap.Source.Planning
{
    // State describing the world for a planner
    [Serializable]
    public struct WorldState : IKnowledgeProvider, IEquatable<WorldState>, IEnumerable<KeyValuePair<SymbolId, int>>
    {
        private readonly IDictionary<SymbolId, int> symbols;

        private WorldState(IDictionary<SymbolId, int> symbols)
        {
            DebugUtils.Assert(symbols != null, "symbols dictionary must not be null");
            this.symbols = symbols;
        }

        public WorldState(WorldState state)
        {
            symbols = state.symbols;
        }

        public bool Contains(SymbolId symbolId)
        {
            return symbols != null && symbols.ContainsKey(symbolId);
        }

        public int this[SymbolId key]
        {
            get
            {
                try
                {
                    return symbols[key];
                }
                catch (KeyNotFoundException e)
                {
                    throw new UnknownSymbolException(
                        key,
                        $"No value for {key} is stored in {GetType()}",
                        e
                    );
                }
                catch (NullReferenceException e)
                {
                    throw new UnknownSymbolException(
                        key,
                        $"Unable to retrieve value for {key}: {GetType()} is empty",
                        e
                    );
                }
            }
        }

        #region IKnowledgeProvider implementation

        public int GetSymbolValue(SymbolId symbolId)
        {
            return this[symbolId];
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<KeyValuePair<SymbolId, int>> GetEnumerator()
        {
            return symbols.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable implementation

        public bool Equals(WorldState other)
        {
            // TODO: Maybe simply compare hashCodes? (Make sure that GetHashCode() is implemented properly first.)
            System.Diagnostics.Debug.Assert(symbols != null, nameof(symbols) + " != null");
            return symbols == null && other.symbols == null
                   || symbols.Count == other.symbols.Count && !symbols.Except(other.symbols).Any();
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(WorldState))
                return false;
            var other = (WorldState) obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // 0 for no symbols
                // Order is not important.
                int? sum = 0;
                foreach (var kvp in symbols) sum += 17 * kvp.Key.GetHashCode() + 23 * kvp.Value.GetHashCode();

                return sum ?? 0;
            }
        }

        public IEnumerable<IPrecondition> ToPreconditions()
        {
            if (symbols == null) return new IPrecondition[0];
            return symbols.Select(pair => new IsEqual(pair.Key, pair.Value)).Cast<IPrecondition>().ToList();
        }

        public IEnumerable<IEffect> ToEffects()
        {
            if (symbols == null) return new IEffect[0];

            return symbols.Select(pair => new SetValue(pair.Key, pair.Value)).Cast<IEffect>().ToList();
        }

        public bool ContainedInState(WorldState other)
        {
            return symbols.Where(symbol => other.Contains(symbol.Key)).All(symbol => symbol.Value == other[symbol.Key]);
        }

        public override string ToString()
        {
            return
                $"[{(symbols == null ? "" : symbols.Aggregate("", (soFar, kvp) => soFar + $"{kvp.Key}={kvp.Value}; "))}]";
        }

        public Builder BuildUpon()
        {
            return new Builder(this);
        }

        public class Builder
        {
            private readonly IDictionary<SymbolId, int> symbols;

            public Builder(WorldState original = default)
            {
                //check for default instance (null)
                symbols = original.symbols == null
                    ? new Dictionary<SymbolId, int>()
                    : new Dictionary<SymbolId, int>(original.symbols);
            }

            public int this[SymbolId key]
            {
                get
                {
                    try
                    {
                        return symbols[key];
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new UnknownSymbolException(
                            key,
                            $"No value for {key} is stored in {GetType()}",
                            e
                        );
                    }
                }
                set => SetSymbol(key, value);
            }

            public Builder ClearSymbols()
            {
                symbols.Clear();
                return this;
            }

            public Builder UnsetSymbol(SymbolId key)
            {
                symbols.Remove(key);
                return this;
            }

            public Builder SetSymbol(SymbolId key, int value)
            {
                symbols[key] = value;
                return this;
            }

            public WorldState Build()
            {
                return new WorldState(symbols);
            }
        }
    }

//	public struct WorldState
//	{
//		private readonly IDictionary<string, WorldStateSymbol> symbols;
//
//		private WorldState(IDictionary<string, WorldStateSymbol> symbols)
//		{
//			DebugUtils.Assert(symbols != null, "symbols dictionary must not be null");
//			this.symbols = symbols;
//		}
//
//		public WorldStateSymbol this[string key]
//		{
//			get {
//				return this.symbols[key];
//			}
//		}
//
//		public Builder BuildUpon()
//		{
//			return new Builder(this);
//		}
//
//		public class Builder
//		{
//			private readonly IDictionary<string, WorldStateSymbol> symbols;
//
//			public Builder(WorldState original = default(WorldState))
//			{
//				if(original.symbols == null)	// Check for default instance
//				{
//					this.symbols = new Dictionary<string, WorldStateSymbol>();
//				}
//				else
//				{
//					this.symbols = new Dictionary<string, WorldStateSymbol>(original.symbols);
//				}
//			}
//
//			public Builder ClearSymbols()
//			{
//				this.symbols.Clear();
//				return this;
//			}
//
//			public Builder UnsetSymbol(string key)
//			{
//				this.symbols.Remove(key);
//				return this;
//			}
//
//			public Builder SetSymbol(string key, WorldStateSymbol symbol)
//			{
//				this.symbols[key] = symbol;
//				return this;
//			}
//
//			public WorldStateSymbol this [string key]
//			{
//				get {
//					return this.symbols[key];
//				}
//				set {
//					this.SetSymbol(key, value);
//				}
//			}
//
//			public WorldState Build()
//			{
//				return new WorldStateSymbol(this.symbols);
//			}
//		}
//	}
}