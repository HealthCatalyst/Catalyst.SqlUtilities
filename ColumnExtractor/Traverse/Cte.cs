using System.Collections.Generic;

namespace ColumnExtractor.Traverse
{
		public class Cte
		{
				public string Name;
				public List<Table> LinkedTables = new List<Table>();

				public override bool Equals(object obj)
				{
						return obj is Cte summary &&
									 Name == summary.Name;
				}

				public override int GetHashCode()
				{
						var hashCode = -601341547;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name != null ? Name.ToLowerInvariant() : null);
						return hashCode;
				}
		}
}