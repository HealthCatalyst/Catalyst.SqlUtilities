using System;
using System.Collections.Generic;

namespace ColumnExtractor
{
		public class ColumnData
		{
				public string AliasNM;
				public string ColumnNM;
				public string FullyQualifiedNM;

				public ColumnData(Column column, bool brackets)
				{
						AliasNM = column.Alias;
						ColumnNM = column.Name;
						FullyQualifiedNM = column.GetFullyQualifiedName(brackets);
				}

				public override bool Equals(object obj)
				{
						return obj is ColumnData data &&
							ColumnNM.Equals(data.ColumnNM, StringComparison.OrdinalIgnoreCase) &&
							FullyQualifiedNM.Equals(data.FullyQualifiedNM, StringComparison.OrdinalIgnoreCase);
				}

				public override int GetHashCode()
				{
						var hashCode = -411786916;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ColumnNM != null ? ColumnNM.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FullyQualifiedNM != null ? FullyQualifiedNM.ToLowerInvariant() : null);
						return hashCode;
				}
		}
}