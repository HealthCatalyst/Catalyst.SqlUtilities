<Query Kind="Program">
  <Reference Relative="ColumnExtractor\bin\Debug\ColumnExtractor.dll">C:\Source\Catalyst.SqlUtilities\ColumnExtractor\bin\Debug\ColumnExtractor.dll</Reference>
  <Reference Relative="ColumnExtractor\bin\Debug\Microsoft.SqlServer.TransactSql.ScriptDom.dll">C:\Source\Catalyst.SqlUtilities\ColumnExtractor\bin\Debug\Microsoft.SqlServer.TransactSql.ScriptDom.dll</Reference>
  <Namespace>ColumnExtractor</Namespace>
  <Namespace>Microsoft.SqlServer.TransactSql.ScriptDom</Namespace>
  <Namespace>ColumnExtractor.Traverse</Namespace>
  <Namespace>ColumnExtractor.Models</Namespace>
</Query>

void Main()
{
	var sql = @"
SELECT 
        year(invoiceDate) as [year],left(datename(month,invoicedate),3)as [month], 
        InvoiceAmount as Amount 
    FROM Invoice
PIVOT
(
    SUM(Amount)
    FOR [month] IN (jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec)
)AS pvt
	";

	Dump(sql);
	var parser = new Parser(true, true, true, false);
	var traverser = new Traverser(true);
	var results = parser.GetTables(sql);

	parser.GetErrors(sql).Dump("ERRORS");
	parser.GetStatements(sql).Dump("STATEMENTS");

	results.Dump("RESULTS");
	results.SelectMany(r => r.Columns).Select(c => c.FullyQualifiedNM).Dump("COLUMNS");
	results.SelectMany(r => r.PossibleColumns).Select(c => c.FullyQualifiedNM).Distinct().Dump("UNKNOWN COLUMNS");
}

private void Dump(string sql)
{
	var parser = new TSql140Parser(true);
	var parsedData = new ParsedData();

	using (var reader = new StringReader(sql))
	{
		var result = parser.Parse(reader, out var errors);
		var traverser = new Traverser(false);
		var columnData = traverser.TraverseObject(result, null).Distinct().ToArray();
		columnData.Dump();
	}
}