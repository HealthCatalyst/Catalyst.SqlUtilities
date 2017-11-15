<Query Kind="Program">
  <Reference Relative="ColumnExtractor\bin\Release\ColumnExtractor.dll">C:\SC\SQL Parser\ColumnExtractor\bin\Release\ColumnExtractor.dll</Reference>
  <Namespace>ColumnExtractor</Namespace>
  <Namespace>Microsoft.SqlServer.TransactSql.ScriptDom</Namespace>
</Query>

void Main()
{
	var sql = @"SELECT
	DATEADD(D,Something,CAST('2011-09-19 16:52:00.000' AS DATE))
	FROM SAM.InfectiousDisease.CAUTI26MetricCensusPatientDay a
	INNER JOIN Foo f ON f.id = a.id
	";

	//sql.Dump("SQL");
	var parser = new Parser(true, true, true);
	var results = parser.GetTables(sql);

	parser.GetErrors(sql).Dump("ERRORS");
	//parser.GetStatements(sql).Dump("STATEMENTS");

	results.Dump("RESULTS");
	results.SelectMany(r => r.Columns).Select(c => c.FullyQualifiedNM).Dump("COLUMNS");
	results.SelectMany(r => r.PossibleColumns).Select(c => c.FullyQualifiedNM).Distinct().Dump("UNKNOWN COLUMNS");
}