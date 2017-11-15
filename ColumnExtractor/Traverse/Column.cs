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

				public override bool Equals(object obj)
				{
						return obj is Column summary &&
									 Name.Equals(summary.Name, StringComparison.OrdinalIgnoreCase) &&
									 EqualityComparer<Table>.Default.Equals(AbsoluteTableReference, summary.AbsoluteTableReference) &&
									 EqualityComparer<IEnumerable<Table>>.Default.Equals(AmbiguousTableReferences,
										 summary.AmbiguousTableReferences);
				}

				public override int GetHashCode()
				{
						var hashCode = 1102383335;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name != null ? Name.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<Table>.Default.GetHashCode(AbsoluteTableReference);
						hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<Table>>.Default.GetHashCode(AmbiguousTableReferences);
						return hashCode;
				}
		}
}