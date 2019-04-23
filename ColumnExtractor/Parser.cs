using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColumnExtractor.Models;
using ColumnExtractor.Traverse;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ColumnExtractor
{
		public class Parser
		{
				private readonly bool _log;
				private readonly bool _outputSelectStar;
				private readonly bool _bracketOutput;
        private readonly bool _outputPivotGeneratedColumns;

        public Parser()
				{
						_log = false;
						_outputSelectStar = false;
						_bracketOutput = false;
				}

				public Parser(bool log, bool outputSelectStar, bool bracketOutput, bool outputPivotGeneratedColumns = false)
				{
						_log = log;
						_outputSelectStar = outputSelectStar;
						_bracketOutput = bracketOutput;
            _outputPivotGeneratedColumns = outputPivotGeneratedColumns;
        }

				public ParsedData GetColumns(string sql)
				{
						var parser = new TSql140Parser(true);
						var parsedData = new ParsedData();

						using (var reader = new StringReader(sql))
						{
								var result = parser.Parse(reader, out var errors);
								var traverser = new Traverser(_log);
								var columnData = traverser.TraverseObject(result, null).Distinct().ToArray();
								var tableData = columnData
                  .Where(c => _outputPivotGeneratedColumns || !c.IsPivotGeneratedColumn)
                  .Where(c => c?.AmbiguousTableReferences != null)
									.SelectMany(c => c.AmbiguousTableReferences)
									.Concat(columnData.Select(c => c.AbsoluteTableReference))
									.Where(c => c != null)
									.Distinct().ToArray();

								parsedData.databaselist = tableData.Select(t => t.GetDatabase()).Where(c => c != null).Distinct().ToList();
								parsedData.schemalist = tableData.Select(t => t.GetSchema()).Where(c => c != null).Distinct().ToList();
								parsedData.tablelist = tableData.Select(t => t.GetFullyQualifiedName(_bracketOutput)).Where(c => c != null).Distinct().ToList();
								parsedData.columnlist = columnData.Select(c => c.GetFullyQualifiedName(_bracketOutput)).Where(c => c != null).Distinct().ToList();
								parsedData.errors = errors.Select(e => e.ToString()).ToList();
						}

						return parsedData;
				}

				public Column[] GetTraverserData(string sql)
				{
						var parser = new TSql140Parser(true);

						using (var reader = new StringReader(sql))
						{
								var result = parser.Parse(reader, out var errors);
								var traverser = new Traverser(_log);
								return traverser.TraverseObject(result, null).Distinct().ToArray();
						}
				}

				public TableData[] GetTables(string sql)
				{
						var parser = new TSql140Parser(true);

						using (var reader = new StringReader(sql))
						{
								var result = parser.Parse(reader, out var errors);
								var traverser = new Traverser(_log);
								var columnData = traverser.TraverseObject(result, null).Distinct().ToArray();
								var tableData = columnData
										.Where(c => c?.AmbiguousTableReferences != null)
										.SelectMany(c => c.AmbiguousTableReferences)
										.Concat(columnData.Select(c => c.AbsoluteTableReference))
										.Where(c => c != null)
										.Distinct()
										.Select(t => new TableData(t, _bracketOutput))
										.Distinct()
										.ToArray();

								foreach (var table in tableData)
								{
										table.Columns = columnData
                      .Where(c => _outputPivotGeneratedColumns || !c.IsPivotGeneratedColumn)
											.Where(c => c.AbsoluteTableReference != null && table.Equals(new TableData(c.AbsoluteTableReference, _bracketOutput)))
											.Select(c => new ColumnData(c, _bracketOutput))
											.Where(cd => _outputSelectStar || cd.ColumnNM != "*")
											.Distinct()
											.ToArray();

										table.PossibleColumns = columnData
                      .Where(c => _outputPivotGeneratedColumns || !c.IsPivotGeneratedColumn)
                      .Where(c => c.AmbiguousTableReferences != null && c.AmbiguousTableReferences.Any(t => table.Equals(new TableData(t, _bracketOutput))))
											.Select(c => new ColumnData(c, _bracketOutput))
											.Where(cd => _outputSelectStar || cd.ColumnNM != "*")
											.Distinct()
											.ToArray();
								}

								return tableData;
						}
				}

				public IEnumerable<ParseError> GetErrors(string sql)
				{
						var parser = new TSql140Parser(true);

						using (var reader = new StringReader(sql))
						{
								parser.Parse(reader, out var errors);
								return errors;
						}
				}

				public IEnumerable<SelectStatement> GetStatements(string sql)
				{
						var parser = new TSql140Parser(true);

						using (var reader = new StringReader(sql))
						{
								var result = parser.Parse(reader, out var errors);
								foreach (var selectStatement in (result as TSqlScript).Batches.SelectMany(b => b.Statements.Cast<SelectStatement>()))
								{
										yield return selectStatement;
								}
						}
				}
		}
}