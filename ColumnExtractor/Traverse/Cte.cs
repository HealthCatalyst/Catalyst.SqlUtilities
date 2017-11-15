using System.Collections.Generic;

namespace ColumnExtractor.Traverse
{
		public class Cte
		{
				public string Alias;
				public string Name;
				public List<Table> LinkedTables = new List<Table>();

				public override bool Equals(object obj)
				{
						return obj is Cte summary &&
									 Alias == summary.Alias &&
									 Name == summary.Name;
				}

				public override int GetHashCode()
				{
						var hashCode = -601341547;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Alias);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name != null ? Name.ToLowerInvariant() : null);
						return hashCode;
				}
		}
}