using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnExtractor.Traverse
{
		public class Column
		{
				public string Alias;
				public string Name;
				public Table AbsoluteTableReference;
				public IEnumerable<Table> AmbiguousTableReferences;
				public IEnumerable<Cte> CteReferences;
        public bool IsPivotGeneratedColumn;

				public string FullyQualifiedName => GetFullyQualifiedName(true);

				public string GetFullyQualifiedName(bool brackets)
				{
						var parts = new[]
						{
								AbsoluteTableReference?.Server, AbsoluteTableReference?.Database, AbsoluteTableReference?.Schema,
								AbsoluteTableReference?.Name, Name
						};
						var availableParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

						return brackets
							? availableParts.Any() ? $"[{string.Join("].[", availableParts)}]".Replace("[*]", "*") : null
							: availableParts.Any() ? $"{string.Join(".", availableParts)}" : null;
				}

        protected bool Equals(Column other)
        {
          return string.Equals(Name, other.Name) && Equals(AbsoluteTableReference, other.AbsoluteTableReference) && Equals(AmbiguousTableReferences, other.AmbiguousTableReferences) && IsPivotGeneratedColumn == other.IsPivotGeneratedColumn;
        }

        public override bool Equals(object obj)
        {
          if (ReferenceEquals(null, obj)) return false;
          if (ReferenceEquals(this, obj)) return true;
          if (obj.GetType() != this.GetType()) return false;
          return Equals((Column) obj);
        }

        public override int GetHashCode()
        {
          unchecked
          {
            var hashCode = (Name != null ? Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (AbsoluteTableReference != null ? AbsoluteTableReference.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (AmbiguousTableReferences != null ? AmbiguousTableReferences.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsPivotGeneratedColumn.GetHashCode();
            return hashCode;
          }
        }
    }
}