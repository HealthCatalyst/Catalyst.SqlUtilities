using System.Collections.Generic;
using System.Linq;

namespace ColumnExtractor.Traverse
{
		public class Table
		{
				public string Alias;
				public string Name;
				public string Schema;
				public string Database;
				public string Server;
				public string FullyQualifiedName => GetFullyQualifiedName(true);
				public string[] SelectColumns = new string[0];
				public string[] PossibleSelectColumns = new string[0];

				public string GetServer()
				{
						return Server;
				}

				public string GetDatabase()
				{
						var parts = new[] { Server, Database };
						var availableParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

						return availableParts.Any() ? $"[{string.Join("].[", availableParts)}]" : null;
				}

				public string GetSchema()
				{
						var parts = new[] { Server, Database, Schema };
						var availableParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

						return availableParts.Any() ? $"[{string.Join("].[", availableParts)}]" : null;
				}

				public string GetFullyQualifiedName(bool brackets)
				{
						var parts = new[] { Server, Database, Schema, Name };
						var availableParts = parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

						return brackets
								? availableParts.Any() ? $"[{string.Join("].[", availableParts)}]" : null
								: availableParts.Any() ? $"{string.Join(".", availableParts)}" : null;
				}

				public override bool Equals(object obj)
				{
						return obj is Table summary &&
									 Alias == summary.Alias &&
									 Name == summary.Name &&
									 Schema == summary.Schema &&
									 Database == summary.Database &&
									 Server == summary.Server;
				}

				public override int GetHashCode()
				{
						var hashCode = -601341549;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name != null ? Name.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Schema != null ? Schema.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Database != null ? Database.ToLowerInvariant() : null);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Server != null ? Server.ToLowerInvariant() : null);
						return hashCode;
				}
		}
}