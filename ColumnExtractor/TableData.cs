using System.Collections.Generic;
using System.Linq;

namespace ColumnExtractor
{
		public class TableData
		{
				public string AliasNM;
				public string TableNM;
				public string SchemaNM;
				public string DatabaseNM;
				public string ServerNM;
				public string FullyQualifiedNM;

				public IEnumerable<ColumnData> Columns;
				public IEnumerable<ColumnData> PossibleColumns;

				public TableData()
				{
				}

				public TableData(Table t, bool brackets)
				{
						AliasNM = t.Alias;
						ServerNM = t.Server;
						DatabaseNM = t.Database;
						SchemaNM = t.Schema;
						TableNM = t.Name;
						FullyQualifiedNM = t.GetFullyQualifiedName(brackets);
				}

				public string GetFullyQualifiedName(bool brackets)
				{
						var parts = new[] { ServerNM, DatabaseNM, SchemaNM, TableNM };
						var availableParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

						return brackets
								? availableParts.Any() ? $"[{string.Join("].[", availableParts)}]" : null
								: availableParts.Any() ? $"{string.Join(".", availableParts)}" : null;
				}

				public override bool Equals(object obj)
				{
						return obj is TableData data &&
										TableNM == data.TableNM &&
										SchemaNM == data.SchemaNM &&
										DatabaseNM == data.DatabaseNM &&
										ServerNM == data.ServerNM;
				}

				public override int GetHashCode()
				{
						var hashCode = 1713068280;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TableNM != null ? TableNM.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SchemaNM != null ? SchemaNM.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DatabaseNM != null ? DatabaseNM.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ServerNM != null ? ServerNM.ToLowerInvariant() : null);
						return hashCode;
				}
		}
}