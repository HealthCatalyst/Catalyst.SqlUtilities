using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ColumnExtractor
{
		public class Traverser
		{
				private string _pad => new string(' ', _level * 2);
				private int _level;
				private readonly string[] _dateFunctions = { "DATEADD", "DATEDIFF", "DATEDIFF_BIG", "DATEFROMPARTS", "DATENAME" };
				private readonly string[] _reservedKeywords =
				{
						"ABSOLUTE", "ACTION", "ADD", "ADMIN", "AFTER", "AGGREGATE", "ALIAS", "ALL", "ALLOCATE", "ALTER", "AND", "ANY",
						"ARE", "ARRAY", "AS", "ASC", "ASENSITIVE", "ASSERTION", "ASYMMETRIC", "AT", "ATOMIC", "AUTHORIZATION", "BACKUP",
						"BEFORE", "BEGIN", "BETWEEN", "BINARY", "BIT", "BLOB", "BOOLEAN", "BOTH", "BREADTH", "BREAK", "BROWSE", "BULK",
						"BY", "CALL", "CALLED", "CARDINALITY", "CASCADE", "CASCADED", "CASE", "CAST", "CATALOG", "CHAR", "CHARACTER",
						"CHECK", "CHECKPOINT", "CLASS", "CLOB", "CLOSE", "CLUSTERED", "COALESCE", "COLLATE", "COLLATION", "COLLECT",
						"COLUMN", "COMMIT", "COMPLETION", "COMPUTE", "CONDITION", "CONNECT", "CONNECTION", "CONSTRAINT", "CONSTRAINTS",
						"CONSTRUCTOR", "CONTAINS", "CONTAINSTABLE", "CONTINUE", "CONVERT", "CORR", "CORRESPONDING", "COVAR_POP",
						"COVAR_SAMP", "CREATE", "CROSS", "CUBE", "CUME_DIST", "CURRENT", "CURRENT_CATALOG", "CURRENT_DATE",
						"CURRENT_DEFAULT_TRANSFORM_GROUP", "CURRENT_PATH", "CURRENT_ROLE", "CURRENT_SCHEMA", "CURRENT_TIME",
						"CURRENT_TIMESTAMP", "CURRENT_TRANSFORM_GROUP_FOR_TYPE", "CURRENT_USER", "CURSOR", "CYCLE", "DATA", "DATABASE",
						"DATE", "DAY", "DBCC", "DEALLOCATE", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFERRABLE", "DEFERRED", "DELETE",
						"DENY", "DEPTH", "DEREF", "DESC", "DESCRIBE", "DESCRIPTOR", "DESTROY", "DESTRUCTOR", "DETERMINISTIC", "DIAGNOSTICS",
						"DICTIONARY", "DISCONNECT", "DISK", "DISTINCT", "DISTRIBUTED", "DOMAIN", "DOUBLE", "DROP", "DUMP", "DYNAMIC",
						"EACH", "ELEMENT", "ELSE", "END", "END-EXEC", "EQUALS", "ERRLVL", "ESCAPE", "EVERY", "EXCEPT", "EXCEPTION", "EXEC",
						"EXECUTE", "EXISTS", "EXIT", "EXTERNAL", "FALSE", "FETCH", "FILE", "FILLFACTOR", "FILTER", "FIRST", "FLOAT", "FOR",
						"FOREIGN", "FOUND", "FREE", "FREETEXT", "FREETEXTTABLE", "FROM", "FULL", "FULLTEXTTABLE", "FUNCTION", "FUSION",
						"GENERAL", "GET", "GLOBAL", "GO", "GOTO", "GRANT", "GROUP", "GROUPING", "HAVING", "HOLD", "HOLDLOCK", "HOST",
						"HOUR", "IDENTITY", "IDENTITY_INSERT", "IDENTITYCOL", "IF", "IGNORE", "IMMEDIATE", "IN", "INDEX", "INDICATOR",
						"INITIALIZE", "INITIALLY", "INNER", "INOUT", "INPUT", "INSERT", "INT", "INTEGER", "INTERSECT", "INTERSECTION",
						"INTERVAL", "INTO", "IS", "ISOLATION", "ITERATE", "JOIN", "KEY", "KILL", "LANGUAGE", "LARGE", "LAST", "LATERAL",
						"LEADING", "LEFT", "LESS", "LEVEL", "LIKE", "LIKE_REGEX", "LIMIT", "LINENO", "LN", "LOAD", "LOCAL", "LOCALTIME",
						"LOCALTIMESTAMP", "LOCATOR", "MAP", "MATCH", "MEMBER", "MERGE", "METHOD", "MINUTE", "MOD", "MODIFIES", "MODIFY",
						"MODULE", "MONTH", "MULTISET", "NAMES", "NATIONAL", "NATURAL", "NCHAR", "NCLOB", "NEW", "NEXT", "NO", "NOCHECK",
						"NONCLUSTERED", "NONE", "NORMALIZE", "NOT", "NULL", "NULLIF", "NUMERIC", "OBJECT", "OCCURRENCES_REGEX", "OF", "OFF",
						"OFFSETS", "OLD", "ON", "ONLY", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML", "OPERATION",
						"OPTION", "OR", "ORDER", "ORDINALITY", "OUT", "OUTER", "OUTPUT", "OVER", "OVERLAY", "PAD", "PARAMETER",
						"PARAMETERS", "PARTIAL", "PARTITION", "PATH", "PERCENT", "PERCENT_RANK", "PERCENTILE_CONT", "PERCENTILE_DISC",
						"PIVOT", "PLAN", "POSITION_REGEX", "POSTFIX", "PRECISION", "PREFIX", "PREORDER", "PREPARE", "PRESERVE", "PRIMARY",
						"PRINT", "PRIOR", "PRIVILEGES", "PROC", "PROCEDURE", "PUBLIC", "RAISERROR", "RANGE", "READ", "READS", "READTEXT",
						"REAL", "RECONFIGURE", "RECURSIVE", "REF", "REFERENCES", "REFERENCING", "REGR_AVGX", "REGR_AVGY", "REGR_COUNT",
						"REGR_INTERCEPT", "REGR_R2", "REGR_SLOPE", "REGR_SXX", "REGR_SXY", "REGR_SYY", "RELATIVE", "RELEASE", "REPLICATION",
						"RESTORE", "RESTRICT", "RESULT", "RETURN", "RETURNS", "REVERT", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROLLUP",
						"ROUTINE", "ROW", "ROWCOUNT", "ROWGUIDCOL", "ROWS", "RULE", "SAVE", "SAVEPOINT", "SCHEMA", "SCOPE", "SCROLL",
						"SEARCH", "SECOND", "SECTION", "SECURITYAUDIT", "SELECT", "SEMANTICKEYPHRASETABLE",
						"SEMANTICSIMILARITYDETAILSTABLE", "SEMANTICSIMILARITYTABLE", "SENSITIVE", "SEQUENCE", "SESSION", "SESSION_USER",
						"SET", "SETS", "SETUSER", "SHUTDOWN", "SIMILAR", "SIZE", "SMALLINT", "SOME", "SPACE", "SPECIFIC", "SPECIFICTYPE",
						"SQL", "SQLEXCEPTION", "SQLSTATE", "SQLWARNING", "START", "STATE", "STATEMENT", "STATIC", "STATISTICS",
						"STDDEV_POP", "STDDEV_SAMP", "STRUCTURE", "SUBMULTISET", "SUBSTRING_REGEX", "SYMMETRIC", "SYSTEM", "SYSTEM_USER",
						"TABLE", "TABLESAMPLE", "TEMPORARY", "TERMINATE", "TEXTSIZE", "THAN", "THEN", "TIME", "TIMESTAMP", "TIMEZONE_HOUR",
						"TIMEZONE_MINUTE", "TO", "TOP", "TRAILING", "TRAN", "TRANSACTION", "TRANSLATE_REGEX", "TRANSLATION", "TREAT",
						"TRIGGER", "TRUE", "TRUNCATE", "TRY_CONVERT", "TSEQUAL", "UESCAPE", "UNDER", "UNION", "UNIQUE", "UNKNOWN", "UNNEST",
						"UNPIVOT", "UPDATE", "UPDATETEXT", "USAGE", "USE", "USER", "USING", "VALUE", "VALUES", "VAR_POP", "VAR_SAMP",
						"VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAITFOR", "WHEN", "WHENEVER", "WHERE", "WHILE", "WIDTH_BUCKET", "WINDOW",
						"WITH", "WITHIN", "WITHIN GROUP", "WITHOUT", "WORK", "WRITE", "WRITETEXT", "XMLAGG", "XMLATTRIBUTES", "XMLBINARY",
						"XMLCAST", "XMLCOMMENT", "XMLCONCAT", "XMLDOCUMENT", "XMLELEMENT", "XMLEXISTS", "XMLFOREST", "XMLITERATE",
						"XMLNAMESPACES", "XMLPARSE", "XMLPI", "XMLQUERY", "XMLSERIALIZE", "XMLTABLE", "XMLTEXT", "XMLVALIDATE", "YEAR",
						"ZONE"
				};

				private readonly bool _log;
				private readonly bool _bracketOutput;

				public Traverser(bool log, bool bracketOutput)
				{
						_log = log;
						_bracketOutput = bracketOutput;
				}

				private void Dump(object value, string description = null, int depth = 3)
				{
						if (!_log) return;
						//value.Dump(description, depth);

						if (description != null)
						{
								var section = new string('-', 80);
								int difference = 78 - description.Length;
								var half = difference / 2;
								var padA = new string(' ', half);
								var padB = new string(' ', half + Math.Abs(difference - (half + half)));
								Console.WriteLine();
								Console.WriteLine($"+{section}+");
								Console.WriteLine($"|{padA} {description} {padB}|");
								Console.WriteLine($"+{section}+");
						}

						Console.WriteLine(value);
				}

				public IEnumerable<Column> TraverseObject(object obj, Cte[] ctes, Table[] parentTables = null)
				{
						if (obj == null) return Enumerable.Empty<Column>();

						if (parentTables == null) parentTables = new Table[0];
						if (ctes == null) ctes = new Cte[0];

						var columns = new List<Column>();

						Dump($"{_pad}TRAVERSING: {obj} | BLACKLIST: [{string.Join(", ", ctes.Select(c => c.Name))}]");

						foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
							.Where(p => !p.PropertyType.IsValueType
													&& !p.Name.Equals("ScriptTokenStream", StringComparison.OrdinalIgnoreCase)
													&& !p.Name.Equals("Type", StringComparison.OrdinalIgnoreCase)
													&& !p.PropertyType.FullName.Equals("System.String", StringComparison.OrdinalIgnoreCase))
							.OrderBy(p => !p.Name.Equals("WithCtesAndXmlNamespaces", StringComparison.OrdinalIgnoreCase)))
						{
								object value = property.GetIndexParameters().Length > 0
									? Helpers.AsEnumerable(property, obj)
									: property.GetValue(obj, null);

								if (value == null) continue;

								Dump($"{_pad}PROPERTY [{property.Name}]: {value.GetType().Name}");

								if (value.GetType().IsAssignableFrom(typeof(WithCtesAndXmlNamespaces)))
								{
										Dump($"{_pad}ANALYZING WithCtesAndXmlNamespaces");
										var result = HandleCtes(value as WithCtesAndXmlNamespaces, ctes, parentTables);
										ctes = ctes.Concat(result.Item1).Distinct().ToArray();
										//parentTables = parentTables.Concat(result.Item3).ToArray();
										columns.AddRange(result.Item2);
								}
								else if (value is QueryExpression)
								{
										Dump($"{_pad}ANALYZING QueryExpression");
										columns.AddRange(HandleQuery(value, ctes, parentTables).Item1);
								}
								else if (property.GetIndexParameters().Length > 0 || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
								{
										var collection = (IEnumerable)value;
										_level++;
										var i = 0;
										foreach (var item in collection)
										{
												Dump($"{_pad}| {property.Name}[{i++}]");
												columns.AddRange(TraverseObject(item, ctes));
										}
										_level--;
								}
								else
								{
										Dump($"{_pad}->");
										_level++;
										columns.AddRange(TraverseObject(value, ctes));
										_level--;
								}
						}

						return columns;
				}

				public Table GetTableFromReference(NamedTableReference namedTableReference)
				{
						var schema = namedTableReference.SchemaObject;
						var table = new Table
						{
								Database = schema.DatabaseIdentifier?.Value,
								Schema = schema.SchemaIdentifier?.Value,
								Server = schema.ServerIdentifier?.Value,
								Name = schema.BaseIdentifier?.Value,
								Alias = namedTableReference.Alias?.Value
						};

						Dump($"{_pad}FOUND TABLE: {table.Alias ?? "_"} {table.FullyQualifiedName}");

						return table;
				}

				public Column GetColumnFromIdentifiers(string[] identifiers, string alias, IEnumerable<Table> containers, IEnumerable<Table> parentContainers, IEnumerable<Cte> ctes, bool inPivot = false)
				{
						if (identifiers == null) return null;

						if (containers == null) containers = new Table[0];
						if (parentContainers == null) parentContainers = new Table[0];
						if (ctes == null) ctes = new Cte[0];

						Dump($"{_pad}PARSING COLUMN: tables: { string.Join(", ", containers.Select(c => c.FullyQualifiedName)) }, parent tables: { string.Join(", ", parentContainers.Select(c => c.FullyQualifiedName)) }, ctes: { string.Join(", ", ctes.Select(c => $"{c.Alias ?? "_"}.{c.Name}")) }");
						//Dump($"{_pad}PARSING COLUMN: tables: { string.Join(", ", containers.Count()) }, parent tables: { string.Join(", ", parentContainers.Count()) }, ctes: { string.Join(", ", ctes.Count()) }");

						var allTables = containers.Concat(parentContainers).Distinct().ToArray();

						var column = new Column
						{
								Alias = alias,
								Name = identifiers.Last()
						};
						var matches = new List<Table>();
						var linkedMatches = new List<Table>();
						var cteMatches = new List<Cte>();

						Dump($"{_pad}> FOUND [{identifiers.Length}] IDENTIFIERS: {string.Join(", ", identifiers)}");

						if (identifiers.Length == 1)
						{
								matches.AddRange(containers);
								cteMatches.AddRange(ctes);
								linkedMatches.AddRange(cteMatches.Where(cte => cte.LinkedTables.Count == 1
										&& (cte.LinkedTables.First().SelectColumns.Contains(column.Name.ToLowerInvariant())
												|| cte.LinkedTables.First().SelectColumns.Contains("*")
												|| inPivot))
									.SelectMany(cte => cte.LinkedTables)
									.Distinct());
								matches.AddRange(cteMatches.Where(cte => cte.LinkedTables.Any(lt =>
												lt.PossibleSelectColumns.Contains(column.Name.ToLowerInvariant())
												|| lt.PossibleSelectColumns.Contains("*")
												|| inPivot))
									.SelectMany(cte => cte.LinkedTables)
									.Distinct());
						}
						else if (identifiers.Length == 2)
						{
								var tableNameOrAlias = identifiers.First();
								var aliasMatches = allTables.Where(t => !string.IsNullOrWhiteSpace(t.Alias) && t.Alias.Equals(tableNameOrAlias, StringComparison.OrdinalIgnoreCase)).ToArray();

								if (aliasMatches.Length == 0)
								{
										var nameMatches = allTables.Where(t =>
											t.Alias == null && t.Name.Equals(tableNameOrAlias, StringComparison.OrdinalIgnoreCase)).ToArray();

										matches.AddRange(nameMatches);
								}
								else
								{
										matches.Add(aliasMatches.First());
								}

								cteMatches.AddRange(ctes.Where(cte => !string.IsNullOrWhiteSpace(cte.Alias) && cte.Alias.Equals(tableNameOrAlias, StringComparison.OrdinalIgnoreCase)
																							 || cte.Alias == null && cte.Name.Equals(tableNameOrAlias, StringComparison.OrdinalIgnoreCase)));

								linkedMatches.AddRange(cteMatches
											.Where(cte => cte.LinkedTables.Count == 1
														&& (cte.LinkedTables.First().SelectColumns.Contains(column.Name.ToLowerInvariant())
														|| cte.LinkedTables.First().SelectColumns.Contains("*")
														|| inPivot))
											.SelectMany(cte => cte.LinkedTables)
											.Distinct());

								matches.AddRange(cteMatches.Where(cte => cte.LinkedTables.Any(lt =>
												lt.PossibleSelectColumns.Contains(column.Name.ToLowerInvariant())
												|| lt.PossibleSelectColumns.Contains("*")
												|| inPivot))
									.SelectMany(cte => cte.LinkedTables)
									.Distinct());
						}
						else if (identifiers.Length == 3)
						{
								var schemaName = identifiers[0];
								var tableName = identifiers[1];

								matches.AddRange(allTables.Where(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)
																							&& t.Schema.Equals(schemaName, StringComparison.OrdinalIgnoreCase)));
						}
						else if (identifiers.Length == 4)
						{
								var databaseName = identifiers[0];
								var schemaName = identifiers[1];
								var tableName = identifiers[2];

								matches.AddRange(allTables.Where(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)
																							&& t.Schema.Equals(schemaName, StringComparison.OrdinalIgnoreCase)
																							&& t.Database.Equals(databaseName, StringComparison.OrdinalIgnoreCase)));
						}
						else if (identifiers.Length == 5)
						{
								var serverName = identifiers[0];
								var databaseName = identifiers[1];
								var schemaName = identifiers[2];
								var tableName = identifiers[3];

								matches.AddRange(allTables.Where(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)
																							&& t.Schema.Equals(schemaName, StringComparison.OrdinalIgnoreCase)
																							&& t.Database.Equals(databaseName, StringComparison.OrdinalIgnoreCase)
																							&& t.Server.Equals(serverName, StringComparison.OrdinalIgnoreCase)));
						}

						var tableNames = matches.Where(m => m.Server == null && m.Database == null && m.Schema == null)
							.Select(m => m.Name.ToLowerInvariant()).ToArray();
						cteMatches.AddRange(ctes.Where(cte => tableNames.Contains(cte.Name.ToLowerInvariant())));

						if (cteMatches.Any())
						{
								column.CteReferences = cteMatches.Select(t => new Cte { Name = t.Name, Alias = t.Alias }).ToArray();

								var cteNames = ctes.Select(cte => cte.Name.ToLowerInvariant()).ToArray();
								var reject = matches.Where(m => m.Server == null && m.Database == null && m.Schema == null && cteNames.Contains(m.Name.ToLowerInvariant()));
								matches = matches.Except(reject).ToList();
						}

						if (linkedMatches.Any() && !matches.Any())
						{
								Dump($"{_pad}> FOUND CTE-LINKED TABLES (MATCHES: {linkedMatches.Count}): [{ string.Join(", ", linkedMatches.Select(c => c.FullyQualifiedName)) }]");
								if (linkedMatches.Count > 1) column.AmbiguousTableReferences = linkedMatches;
								else if (linkedMatches.Count == 1) column.AbsoluteTableReference = linkedMatches.First();
						}
						else
						{
								Dump($"{_pad}> FOUND TABLES (MATCHES: {matches.Count}): [{ string.Join(", ", matches.Select(c => c.FullyQualifiedName)) }]");
								if (matches.Count > 1) column.AmbiguousTableReferences = matches;
								else if (matches.Count == 1) column.AbsoluteTableReference = matches.First();
						}

						var matchingTable = column.AbsoluteTableReference != null
							? $" | MATCHING TABLE: {column.AbsoluteTableReference.FullyQualifiedName}"
							: string.Empty;
						var matchingTables = column.AmbiguousTableReferences != null
							? $" | MATCHING TABLES: {string.Join(", ", column.AmbiguousTableReferences.Select(t => t.FullyQualifiedName))}"
							: string.Empty;
						var matchingCtes = column.CteReferences != null
							? $" | MATCHING CTES: {string.Join(", ", column.CteReferences.Select(t => t.Name))}"
							: string.Empty;

						Dump($"{_pad}> FOUND COLUMN: {column.FullyQualifiedName}{matchingTable}{matchingTables}{matchingCtes}");

						return column;
				}

				public Column GetColumnFromReference(ColumnReferenceExpression columnReference, string alias, IEnumerable<Table> containers, IEnumerable<Table> parentContainers, IEnumerable<Cte> ctes, bool inPivot = false)
				{
						if (columnReference?.MultiPartIdentifier?.Identifiers == null) return null;

						var identifiers = columnReference.MultiPartIdentifier.Identifiers.ToArray();
						var columnName = identifiers.Last().Value;
						var isKeyword = _reservedKeywords.Contains(columnName.ToUpperInvariant());
						if (isKeyword && identifiers.First().QuoteType == QuoteType.NotQuoted)
						{
								Dump($"{_pad}***FOUND KEYWORD*** {columnName}");
								return null;
						}

						return GetColumnFromIdentifiers(identifiers.Select(i => i.Value).ToArray(), alias, containers, parentContainers, ctes, inPivot);
				}

				public Column GetColumnFromStarReference(SelectStarExpression expression, IEnumerable<Table> containers, IEnumerable<Table> parentContainers, IEnumerable<Cte> ctes)
				{
						if (expression == null) return null;

						var identifiers = expression.Qualifier?.Identifiers != null ? expression.Qualifier?.Identifiers.ToArray() : new Identifier[0];
						var names = identifiers.Select(i => i.Value).Concat(new[] { "*" }).ToArray();

						return GetColumnFromIdentifiers(names, null, containers, parentContainers, ctes);
				}

				public (IEnumerable<Cte>, IEnumerable<Column>, IEnumerable<Table>) HandleCtes(WithCtesAndXmlNamespaces withCtesAndXmlNamespaces, Cte[] parentCtes, Table[] parentTables)
				{
						_level++;
						var tables = new List<Table>();
						var columns = new List<Column>();
						var ctes = new List<Cte>();

						if (withCtesAndXmlNamespaces == null)
						{
								Dump($"{_pad}WithCtesAndXmlNamespaces is NULL");
								_level--;
								return (Enumerable.Empty<Cte>(), Enumerable.Empty<Column>(), Enumerable.Empty<Table>());
						}

						var i = 0;

						foreach (var commonTableExpression in withCtesAndXmlNamespaces.CommonTableExpressions)
						{
								Dump($"{_pad}| CommonTableExpressions[{i++}]");
								if (commonTableExpression.QueryExpression != null)
								{
										Dump($"{_pad}PROPERTY [QueryExpression]: handling...");
										var result = HandleQuery(commonTableExpression.QueryExpression, parentCtes.ToArray(), parentTables.Distinct().ToArray());
										columns.AddRange(result.Item1);
										tables.AddRange(result.Item2);
								}

								var cte = new Cte { Name = commonTableExpression.ExpressionName?.Value, LinkedTables = tables.Distinct().ToList() };
								foreach (var linkedTable in cte.LinkedTables)
								{
										linkedTable.SelectColumns = columns
											.Where(c => ReferenceEquals(c.AbsoluteTableReference, linkedTable)).Select(c => c.Name.ToLowerInvariant())
											.ToArray();
										linkedTable.PossibleSelectColumns = columns
											.Where(c => c.AmbiguousTableReferences != null && c.AmbiguousTableReferences.Any(t => ReferenceEquals(t, linkedTable)))
											.Select(c => c.Name.ToLowerInvariant())
											.ToArray();
								}

								if (cte.LinkedTables.Any()) Dump($"{_pad}FOUND CTE-LINKED TABLES: {cte.Name}, tables: {string.Join(", ", cte.LinkedTables.Select(t => t.FullyQualifiedName))}");
								ctes.Add(cte);
								parentCtes = parentCtes.Concat(new[] { cte }).Distinct().ToArray();
						}

						_level--;
						return (ctes, columns.Where(c => c != null), tables);
				}

				public (IEnumerable<Column>, IEnumerable<Table>) HandleQuery(object obj, Cte[] ctes, Table[] parentTables = null)
				{
						_level++;
						var tables = new List<Table>();
						var columns = new List<Column>();

						if (obj == null)
						{
								Dump($"{_pad}Query is NULL");
								_level--;
								return (Enumerable.Empty<Column>(), Enumerable.Empty<Table>());
						}

						var properties = new[] { "FirstQueryExpression", "SecondQueryExpression", "QueryExpression" };
						var references = Helpers.GetPropertiesWithNames(obj, properties).ToArray();

						foreach (var reference in references.Where(r => r != null))
						{
								var value = reference.GetIndexParameters().Length > 0
									? Helpers.AsEnumerable(reference, obj)
									: reference.GetValue(obj, null);

								if (value == null) continue;

								Dump($"{_pad}PROPERTY [{reference.Name}]: {value.GetType().Name}");

								if (value is QueryExpression)
								{
										Dump($"{_pad}PROPERTY [{reference.Name}]: handling Query");
										var result = HandleQuery(value, ctes.ToArray(), tables.ToArray());
										columns.AddRange(result.Item1);
										tables.AddRange(result.Item2);
								}
						}

						var fromClause = Helpers.GetPropertyValueWithName(obj, "FromClause");
						if (fromClause != null)
						{
								var tableReferences = Helpers.GetEnumerablePropertyValueWithName(fromClause, "TableReferences");
								if (tableReferences != null)
								{
										Dump($"{_pad}PROPERTY [FromClause.TableReferences]: handling tables and columns...");
										foreach (var tableReference in tableReferences)
										{
												var result = HandleTables(tableReference, ctes, parentTables);
												ctes = ctes.Concat(result.Item1).ToArray();
												tables.AddRange(result.Item2);
												columns.AddRange(HandleColumns(tableReference, null, tables.ToArray(), parentTables, ctes));
										}
								}
						}

						var selectElements = Helpers.GetEnumerablePropertyValueWithName(obj, "SelectElements");
						if (selectElements != null)
						{
								Dump($"{_pad}PROPERTY [SelectElements]: handling columns...");
								foreach (var element in selectElements)
								{
										columns.AddRange(HandleColumns(element, null, tables.ToArray(), parentTables, ctes));
								}
						}

						var orderByClause = Helpers.GetPropertyValueWithName(obj, "OrderByClause");
						if (orderByClause != null)
						{
								var orderByElements = Helpers.GetEnumerablePropertyValueWithName(orderByClause, "OrderByElements");
								if (orderByElements != null)
								{
										Dump($"{_pad}PROPERTY [OrderByClause.OrderByElements]: handling columns...");
										foreach (var element in orderByElements)
										{
												columns.AddRange(HandleColumns(element, null, tables.ToArray(), parentTables, ctes));
										}
								}
						}

						var forClause = Helpers.GetPropertyValueWithName(obj, "ForClause");
						if (forClause != null)
						{
								var forColumns = Helpers.GetEnumerablePropertyValueWithName(forClause, "Columns");

								if (forColumns != null)
								{
										Dump($"{_pad}PROPERTY [ForClause]: handling columns...");

										foreach (var forColumn in forColumns)
										{
												columns.Add(GetColumnFromReference(forColumn as ColumnReferenceExpression, null, tables, parentTables, ctes));
										}
								}
						}

						var offsetClause = Helpers.GetPropertyValueWithName(obj, "OffsetClause");
						if (offsetClause != null)
						{
								Dump($"{_pad}PROPERTY [OffsetClause]: handling columns...");
								var fetchExpression = Helpers.GetPropertyValueWithName(offsetClause, "FetchExpression");
								var offsetExpression = Helpers.GetPropertyValueWithName(offsetClause, "OffsetExpression");
								columns.AddRange(HandleColumns(fetchExpression, null, tables.ToArray(), parentTables, ctes));
								columns.AddRange(HandleColumns(offsetExpression, null, tables.ToArray(), parentTables, ctes));
						}

						var havingClause = Helpers.GetPropertyValueWithName(obj, "HavingClause");
						if (havingClause != null)
						{
								var searchCondition = Helpers.GetPropertyValueWithName(havingClause, "SearchCondition");
								if (searchCondition != null)
								{
										Dump($"{_pad}PROPERTY [HavingClause]: handling columns...");
										columns.AddRange(HandleColumns(searchCondition, null, tables.ToArray(), parentTables, ctes));
								}
						}

						var topRowFilter = Helpers.GetPropertyValueWithName(obj, "TopRowFilter");
						if (topRowFilter != null)
						{
								var expression = Helpers.GetPropertyValueWithName(topRowFilter, "Expression");
								if (expression != null)
								{
										Dump($"{_pad}PROPERTY [TopRowFilter]: handling columns...");
										columns.AddRange(HandleColumns(expression, null, tables.ToArray(), parentTables, ctes));
								}
						}

						var groupByClause = Helpers.GetPropertyValueWithName(obj, "GroupByClause");
						if (groupByClause != null)
						{
								var groupingSpecifications = Helpers.GetEnumerablePropertyValueWithName(groupByClause, "GroupingSpecifications");
								if (groupingSpecifications != null)
								{
										Dump($"{_pad}PROPERTY [GroupByClause]: handling columns...");
										foreach (var groupingSpecification in groupingSpecifications)
										{
												columns.AddRange(HandleColumns(groupingSpecification, null, tables.ToArray(), parentTables, ctes));
										}
								}
						}

						var whereClause = Helpers.GetPropertyValueWithName(obj, "WhereClause");
						if (whereClause != null)
						{
								var searchCondition = Helpers.GetPropertyValueWithName(whereClause, "SearchCondition");
								if (searchCondition != null)
								{
										Dump($"{_pad}PROPERTY [WhereClause]: handling columns...");
										columns.AddRange(HandleColumns(searchCondition, null, tables.ToArray(), parentTables, ctes));
								}
						}

						_level--;

						return (columns.Where(c => c != null), tables);
				}

				public (IEnumerable<Cte>, IEnumerable<Table>) HandleTables(object obj, Cte[] parentCtes, Table[] parentTables = null)
				{
						if (obj == null) return (Enumerable.Empty<Cte>(), Enumerable.Empty<Table>()); ;

						var tables = new List<Table>();
						var ctes = new List<Cte>();

						//https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.transactsql.scriptdom.tablereferencewithalias.aspx?f=255&MSPPError=-2147217396
						if (obj.GetType().IsAssignableFrom(typeof(NamedTableReference)))
						{
								tables.Add(GetTableFromReference(obj as NamedTableReference));
						}
						else if (obj.GetType().IsAssignableFrom(typeof(QueryDerivedTable)))
						{
								var derived = obj as QueryDerivedTable;
								Dump($"{_pad}ANALYZING QueryDerivedTable...");

								if (derived?.QueryExpression != null)
								{
										Dump($"{_pad}PROPERTY [QueryExpression]: handling...");
										var result = HandleQuery(derived.QueryExpression, parentCtes, parentTables);
										var cte = new Cte { Name = derived.Alias?.Value, LinkedTables = result.Item2.Distinct().ToList() };

										foreach (var linkedTable in cte.LinkedTables)
										{
												linkedTable.SelectColumns = result.Item1
													.Where(c => ReferenceEquals(c.AbsoluteTableReference, linkedTable)).Select(c => c.Name.ToLowerInvariant())
													.ToArray();
												linkedTable.PossibleSelectColumns = result.Item1
													.Where(c => c.AmbiguousTableReferences != null && c.AmbiguousTableReferences.Any(t => ReferenceEquals(t, linkedTable)))
													.Select(c => c.Name.ToLowerInvariant())
													.ToArray();
										}

										if (cte.LinkedTables.Any()) Dump($"{_pad}FOUND DERIVED-LINKED TABLES: {cte.Name}, tables: {string.Join(", ", cte.LinkedTables.Select(t => t.FullyQualifiedName))}");
										ctes.Add(cte);
								}
						}
						else
						{
								var references = Helpers.GetPropertiesWithNames(obj, "FirstTableReference", "SecondTableReference", "TableReference", "Join").ToArray();
								if (references.Any()) _level++;

								foreach (var reference in references.Where(r => r != null))
								{
										var result = HandleTables(reference.GetValue(obj, null), parentCtes, parentTables);
										tables.AddRange(result.Item2);
										ctes.AddRange(result.Item1);
								}

								if (references.Any()) _level--;
						}

						return (ctes.Where(c => c != null), tables.Where(t => t != null));
				}

				public IEnumerable<Column> HandleColumns(object obj, string alias, Table[] tables, Table[] parentTables, Cte[] ctes)
				{
						if (obj == null) return Enumerable.Empty<Column>();

						var handleColumns = new List<Column>();
						if (obj.GetType().IsAssignableFrom(typeof(FunctionCall)))
						{
								var func = obj as FunctionCall;
								var name = func?.FunctionName?.Value;
								if (name != null && _dateFunctions.Contains(name))
								{
										Dump($"{_pad}HANDLING DATE FUNCTION...");
										foreach (var parameter in func.Parameters.Skip(1))
										{
												_level++;
												handleColumns.AddRange(HandleColumns(parameter, alias, tables, parentTables, ctes));
												_level--;
										}

										handleColumns.AddRange(HandleProperties(obj, alias, tables, parentTables, ctes, new[] { "Parameters" }));
								}
								else
								{
										handleColumns.AddRange(HandleProperties(obj, alias, tables, parentTables, ctes));
								}
						}
						else if (obj.GetType().IsAssignableFrom(typeof(ColumnReferenceExpression)))
						{
								handleColumns.Add(GetColumnFromReference(obj as ColumnReferenceExpression, alias, tables, parentTables, ctes));
						}
						else if (obj.GetType().IsAssignableFrom(typeof(SelectStarExpression)))
						{
								handleColumns.Add(GetColumnFromStarReference(obj as SelectStarExpression, tables, parentTables, ctes));
						}
						else if (obj.GetType().IsAssignableFrom(typeof(PivotedTableReference)))
						{
								if (obj is PivotedTableReference pivotedTableReference)
								{
										handleColumns.Add(GetColumnFromReference(pivotedTableReference.PivotColumn, null, tables, parentTables, ctes, true));
										if (pivotedTableReference.ValueColumns != null)
										{
												foreach (var valueColumn in pivotedTableReference.ValueColumns)
												{
														handleColumns.Add(GetColumnFromReference(valueColumn, null, tables, parentTables, ctes, true));
												}
										}
								}

								handleColumns.AddRange(HandleProperties(obj, alias, tables, parentTables, ctes));
						}
						else if (obj.GetType().IsAssignableFrom(typeof(UnpivotedTableReference)))
						{
								if (obj is UnpivotedTableReference unpivotedTableReference)
								{
										if (unpivotedTableReference.PivotColumn?.Value != null)
										{
												handleColumns.Add(GetColumnFromIdentifiers(new[] { unpivotedTableReference.PivotColumn.Value }, null, tables, parentTables, ctes, true));
										}
										if (unpivotedTableReference.ValueColumn?.Value != null)
										{
												handleColumns.Add(GetColumnFromIdentifiers(new[] { unpivotedTableReference.ValueColumn.Value }, null, tables, parentTables, ctes, true));
										}
								}

								handleColumns.AddRange(HandleProperties(obj, alias, tables, parentTables, ctes));
						}
						else if (obj is QueryExpression)
						{
								Dump($"{_pad}HANDLING QueryExpression...");
								handleColumns.AddRange(HandleQuery(obj, ctes, tables).Item1);
						}
						else if (!obj.GetType().IsValueType)
						{
								handleColumns.AddRange(HandleProperties(obj, alias, tables, parentTables, ctes));
						}

						return handleColumns.Where(c => c != null);
				}

				public IEnumerable<Column> HandleProperties(object obj, string alias, Table[] tables, Table[] parentTables, Cte[] ctes, string[] skipProperties = null)
				{
						if (obj == null) return Enumerable.Empty<Column>();

						if (skipProperties == null) skipProperties = new string[0];

						var handleColumns = new List<Column>();

						if (!obj.GetType().IsValueType)
						{
								var subQuery = Helpers.GetPropertyValueWithName(obj, "SubQuery");

								if (subQuery != null)
								{
										Dump($"{_pad}Is SubQuery...");
										var columns = TraverseObject(subQuery, ctes, tables.ToArray());

										handleColumns.AddRange(columns);
								}

								var newAlias = GetAlias(obj);

								alias = newAlias ?? alias;

								var properties = new[]
								{
										"FirstQueryExpression", "SecondQueryExpression", "FirstTableReference", "SecondTableReference", "TableReference", "Join",
										"FirstExpression", "SecondExpression", "WhenExpression", "ThenExpression", "ElseExpression", "Expression", "Expressions",
										"FirstExpression", "SecondExpression", "ThirdExpression", "QueryExpression", "Parameter", "SearchCondition", "Parameters",
										"WhenClauses", "OrderByClause", "OrderByElements", "OverClause", "InputExpression", "WithinGroupClause", "Partitions",
										"WindowFrameClause", "WindowDelimiter", "OffsetValue", "Top", "Bottom", "Predicate", "Values"
								};

								var references = Helpers.GetPropertiesWithNames(obj, properties.Except(skipProperties).ToArray()).ToArray();

								if (references.Any()) _level++;

								foreach (var reference in references.Where(r => r != null))
								{
										var value = reference.GetIndexParameters().Length > 0
											? Helpers.AsEnumerable(reference, obj)
											: reference.GetValue(obj, null);

										if (value == null) continue;

										Dump($"{_pad}PROPERTY [{reference.Name}]: {value.GetType().Name}");

										if (reference.GetIndexParameters().Length > 0 || typeof(IEnumerable).IsAssignableFrom(reference.PropertyType))
										{
												var collection = (IEnumerable)value;
												var i = 0;
												foreach (var item in collection)
												{
														Dump($"{_pad}| {reference.Name}[{i++}]");
														handleColumns.AddRange(HandleColumns(item, alias, tables, parentTables, ctes));
												}
										}
										else
										{
												handleColumns.AddRange(HandleColumns(value, alias, tables, parentTables, ctes));
										}
								}

								if (references.Any()) _level--;
						}

						return handleColumns.Where(c => c != null);
				}

				public string GetAlias(object obj)
				{
						string alias = null;

						var columnName = Helpers.GetPropertyValueWithName(obj, "ColumnName");

						if (columnName != null)
						{
								var value = Helpers.GetPropertyValueWithName(columnName, "Value");

								if (value != null) alias = value as string;
						}

						return alias;
				}
		}
}