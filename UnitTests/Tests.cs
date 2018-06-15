using System.Linq;
using ColumnExtractor;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void SmokeTest()
		{
			var sql = @"SELECT * FROM Foo";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.IsNotNull(results);
		}

		[Test]
		public void Query1()
		{
			var sql = @"
WITH
LinkMeasureElementCTE AS (
	SELECT *
	FROM SAM.Pneumonia.QVExtElementBASE
),

ConfigReplaceCTE AS (
	SELECT
		[ExpressionCD]              = 'Base',
		[ReplaceSetAnalysisWithSTR] = '',
		[ReplaceAggrWithSTR]        = '@(=%DateCycleGroupByCOL)'		
	UNION ALL
	SELECT
		[ExpressionCD]              = 'Total',
		[ReplaceSetAnalysisWithSTR] = 'TOTAL ',
		[ReplaceAggrWithSTR]        = '@(=%DateCycleGroupByCOL)'		
	UNION ALL
	SELECT
		[ExpressionCD]              = 'PeriodCurrent',
		[ReplaceSetAnalysisWithSTR] = '{<@(=@(xPeriodCurrent))>}',
		[ReplaceAggrWithSTR]        = '@(=%MeasureRangeGroupByCOL)'	
	UNION ALL
	SELECT
		[ExpressionCD]              = 'PeriodPrior',
		[ReplaceSetAnalysisWithSTR] = '{<@(=@(xPeriodPrior))>}',
		[ReplaceAggrWithSTR]        = '@(=%MeasureRangeGroupByCOL)'
),



/** add standard set analysis expressions **/
StandardExpressionCTE AS (
	SELECT
		 MeasureID
		,ElementCD
		,c.ExpressionCD AS ExpressionCD
		,REPLACE(REPLACE(ExpressionSTR,SetAnalysisSTR,c.ReplaceSetAnalysisWithSTR),AggrSTR,c.ReplaceAggrWithSTR) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	CROSS JOIN ConfigReplaceCTE c

	UNION ALL
	SELECT
		 MeasureID
		,ElementCD
		,'BaseChng' AS ExpressionCD
		,'NUM(@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={Base}>}%ExpressionSTR))-@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={Total}>}%ExpressionSTR)),chr(9650)&'+CHAR(39)+MeasureFormatCD+';'+CHAR(39)+'&chr(9660)&'+CHAR(39)+MeasureFormatCD+CHAR(39)+')' AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION ALL
	SELECT
		 MeasureID
		,ElementCD
		,'BasePcntChng' AS ExpressionCD
		,'NUM((@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={Base}>}%ExpressionSTR))-@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={Total}>}%ExpressionSTR)))/(@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={Base}>}%ExpressionSTR))),'+CHAR(39)+'#,##0%'+CHAR(39)+')' AS ExpressionSTR
	FROM LinkMeasureElementCTE

	UNION ALL
	SELECT
		 MeasureID
		,ElementCD
		,'PeriodChng' AS ExpressionCD
		,'NUM(@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodCurrent}>}%ExpressionSTR))-@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodPrior}>}%ExpressionSTR)),chr(9650)&'+CHAR(39)+MeasureFormatCD+';'+CHAR(39)+'&chr(9660)&'+CHAR(39)+MeasureFormatCD+CHAR(39)+')' AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION ALL
	SELECT
		 MeasureID
		,ElementCD
		,'PeriodPcntChng' AS ExpressionCD
		,'NUM((@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodCurrent}>}%ExpressionSTR))-@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodPrior}>}%ExpressionSTR)))/(@(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodCurrent}>}%ExpressionSTR))),'+CHAR(39)+'#,##0%'+CHAR(39)+')' AS ExpressionSTR
	FROM LinkMeasureElementCTE
),

/** add additional elements for informational purposes **/
InfoExpressionCTE AS (
	SELECT
		 MeasureID
		,'LABEL' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+MeasureNM+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'UNIT' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+MeasureUnitDSC+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'UNIT_NUMER' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+NumUnitDSC+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'UNIT_DENOM' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+DenomUnitDSC+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'ICON' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+'qmem://ImageID/'+ISNULL(MeasureIconNM,'')+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'ICON_GHOST' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+'qmem://ImageID/'+ISNULL(MeasureIconNM+'_ghost','')+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'ICON_TYPE' AS ElementCD
		,'Base' AS ExpressionCD
		,CASE MeasureTypeDSC
			WHEN 'Outcome' THEN CHAR(39)+'qmem://ImageID/outcome_rounded'+CHAR(39)
			WHEN 'Process' THEN CHAR(39)+'qmem://ImageID/process_rounded'+CHAR(39)
		 END AS ExpressionSTR
	FROM LinkMeasureElementCTE
	UNION
	SELECT
		 MeasureID
		,'TYPE' AS ElementCD
		,'Base' AS ExpressionCD
		,CHAR(39)+MeasureTypeDSC+CHAR(39) AS ExpressionSTR
	FROM LinkMeasureElementCTE

	UNION
	SELECT
		 MeasureID
		,ElementCD
		,'BaseChng_Color' AS ExpressionCD
		,CASE
			WHEN MeasureDirectionCD = 'n/a'
			THEN 'none'
			WHEN MeasureDirectionCD = 'Up'
			THEN 'IF( @(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={BaseChng}>}%ExpressionSTR)) >= 0,''green'',''red'')'
			WHEN MeasureDirectionCD = 'Down'
			THEN 'IF( @(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={BaseChng}>}%ExpressionSTR)) > 0,''red'',''green'')'
		 END AS ExpressionSTR
	FROM LinkMeasureElementCTE

	UNION
	SELECT
		 MeasureID
		,ElementCD
		,'PeriodChng_Color' AS ExpressionCD
		,CASE
			WHEN MeasureDirectionCD = 'n/a'
			THEN 'none'
			WHEN MeasureDirectionCD = 'Up'
			THEN 'IF( @(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodChng}>}%ExpressionSTR)) >= 0,''green'',''red'')'
			WHEN MeasureDirectionCD = 'Down'
			THEN 'IF( @(=ONLY({<%MeasureID={'+CAST(MeasureID AS VARCHAR)+'},%ElementCD={'+ElementCD+'},%ExpressionCD={PeriodChng}>}%ExpressionSTR)) > 0,''red'',''green'')'
		 END AS ExpressionSTR
	FROM LinkMeasureElementCTE
)

/** union all the magic together **/
SELECT *
FROM StandardExpressionCTE
UNION ALL
SELECT *
FROM InfoExpressionCTE";
			var parser = new Parser(false, true, true);
			var results = parser.GetTables(sql);

			Assert.AreEqual(14, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void Query2()
		{
			var sql = @"
WITH
CurrentUnitCTE AS (
	SELECT
		a.PatientID
		,a.LocationNM
		,a.DepartmentNM
		,NULL AS DepartmentGrouper1NM
	FROM SAM.InfectiousDisease.CAUTI26MetricCensusPatientDay a
	INNER JOIN (
		SELECT DISTINCT PatientID
		FROM SAM.InfectiousDisease.CAUTI26PopulationEncounter
		WHERE InHospitalFLG = 1
		) b ON b.PatientID = a.PatientID
	WHERE a.CensusDT = DATEADD(D,-1,CAST('2011-09-19 16:52:00.000' AS DATE))
)

SELECT
	p.PatientDateKEY
	,p.PatientID
	,e.MRN
	,p.DateDT
	,e.FacilityAccountID
	,e.PatientEncounterID
	,e.AdmitDTS
	,e.DischargeDTS
	,e.DischargeOrCurrentDT
	,e.EncounterTypeCD
	,e.InHospitalFLG
	,e.PatientNM
	,e.GenderCD
	,e.BirthDTS
	,e.AgeAtAdmitNBR
	,e.AgeAtAdmitCohortTXT
	,e.AgeAtAdmitDisplayTXT
	,e.AdmitICDDiagnosisCD
	,e.FinancialClassID
	,e.FinancialClassNM
	,e.TotalChargeAMT
	,e.TotalCostAMT
	,e.PrimaryServiceDSC
	,e.PrimaryICDDiagnosisCD
	,e.PrimaryICDDiagnosisDSC
	,e.AdmitTypeCD
	,e.AdmitTypeDSC
	,pd.CensusDT
	,pd.CensusDTS
	,pd.LocationID AS CensusLocationID
	,pd.LocationNM AS CensusLocationNM
	,pd.DepartmentID AS CensusDepartmentID
	,ISNULL(pd.DepartmentGrouper1NM,pd.DepartmentNM) AS CensusDepartmentNM
	,pd.RoomID AS CensusRoomID
	,pd.RoomNM AS CensusRoomNM
	,pd.BedID AS CensusBedID
	,pd.BedNM AS CensusBedNM
	,cu.LocationNM AS CurrentLocationNM
	,ISNULL(cu.DepartmentGrouper1NM,cu.DepartmentNM) AS CurrentDepartmentNM
	,CurrentDepartmentFLG = CASE WHEN ISNULL(cu.DepartmentGrouper1NM,cu.DepartmentNM) = ISNULL(pd.DepartmentGrouper1NM,pd.DepartmentNM) THEN 1 ELSE 0 END
	,ISNULL(pd.PatientDayCNT,0) AS PatientDayCNT
	,CASE WHEN ISNULL(cl.LineCNT,0) > 5 THEN 5 ELSE ISNULL(cl.LineCNT,0) END AS LineCNT
	,ISNULL(cl.LineDayCNT,0) AS LineDayCNT
	,CASE WHEN cl.LineDayStatusDSC = 'Permanent' THEN ISNULL(cl.LineDayCNT,0) ELSE 0 END AS PermLineDayCNT /*Perm Line Count*/
	,CASE WHEN cl.LineDayStatusDSC = 'Temporary' THEN ISNULL(cl.LineDayCNT,0) ELSE 0 END AS TempLineDayCNT /*Temp Line Count*/
	,CASE WHEN cl.LineDayCNT IS NULL THEN 'No' ELSE 'Yes' END AS LineFLG
	,CONVERT(INT,ISNULL(FemoralLineCNT,0)) FemoralLineCNT
	,CASE WHEN chg.PatientID IS NULL THEN 0 ELSE 1 END AS MedicatedBathCNT /*CHG Bathing Compliance*/
	,CASE WHEN st.CAUTIFLG = 'Y' THEN 1 ELSE 0 END AS CAUTIFLG
FROM SAM.InfectiousDisease.CAUTI26MetricPatientDay p
INNER JOIN SAM.InfectiousDisease.CAUTI26PopulationEncounter e ON e.PatientEncounterID = p.PatientEncounterID
LEFT OUTER JOIN SAM.InfectiousDisease.CAUTI26MetricCensusPatientDay pd ON pd.PatientDateKEY = p.PatientDateKEY
LEFT OUTER JOIN SAM.InfectiousDisease.CAUTI26MetricCensusLineDay cl ON cl.PatientDateKEY = p.PatientDateKEY
LEFT OUTER JOIN SAM.InfectiousDisease.CAUTI26MetricRiskEventDay chg ON chg.PatientDateKEY = p.PatientDateKEY AND chg.RiskFactorDSC = 'Sterile Barrier (CHG Bathing)'
LEFT OUTER JOIN CurrentUnitCTE cu ON cu.PatientID = p.PatientID and e.inhospitalFLG=1
LEFT OUTER JOIN SAM.InfectiousDisease.CAUTI26MetricReviewStatus s ON s.PatientID = p.PatientID AND s.CollectDT = p.DateDT
LEFT OUTER JOIN SAM.InfectiousDisease.CAUTI26MetricReviewStatusReference st ON st.ReviewConclusionDSC = s.ReviewConclusionDSC";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(55, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void CircularCteShouldNotBeListedAsTableDependency()
		{
			var sql = @"
WITH RecurCTE AS
(
	SELECT * FROM
	(
		SELECT pop.CohortCD, r.Id
		FROM SAM.Readmissions.ReadmissionPopulation pop
		JOIN RecurCTE r ON pop.CohortCD = r.CohortCD
	) i
)
SELECT * FROM RecurCTE";
			var parser = new Parser(true, true, true);
			var results = parser.GetTables(sql);

			Assert.AreEqual(1, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void Between()
		{
			var sql = @"
SELECT c.PatientID
FROM SAM.InfectiousDisease.CLABSIHIMSSPopulationCulture c
LEFT OUTER JOIN SAM.InfectiousDisease.CLABSIHIMSSMetricLocalityDepartment d2 ON d2.PatientID = c.PatientID
	AND c.SpecimenTakenTimeDTS BETWEEN d2.DepartmentBeginDTS AND d2.DepartmentEndDTS;";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(5, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void ShouldNotShareScopeOutsideOfCte()
		{
			var sql = @"
WITH PartAExclusions AS
(
	SELECT A.QualifyingDRGSetID
	FROM SharedRiskSM.Reference.BPQualifyingDRGCrosswalkBASE A
)

SELECT A.SetID, A.AnchorEncounterFLG
FROM SAM.BundledPayments.BPClaimDetailBASE A";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(3, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void Nested()
		{
			var sql = @"
SELECT CAST(CaseDT AS DATETIME) AS CaseDT
FROM (SELECT * FROM SAM.AllinaScorecard.GlycemicControlDiabetes WHERE Dimension3VAL = 'Y') z";
			var parser = new Parser(false, true, true);
			var results = parser.GetTables(sql);

			Assert.AreEqual(3, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void NestedAlias()
		{
			var sql = @"
SELECT CAST(z.CaseDT AS DATETIME) AS CaseDT
FROM (SELECT * FROM SAM.AllinaScorecard.GlycemicControlDiabetes WHERE Dimension3VAL = 'Y') z";
			var parser = new Parser(false, true, true);
			var results = parser.GetTables(sql);

			Assert.AreEqual(3, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void NamedColumnShouldNotBeIncluded()
		{
			var sql = @"SELECT [Key] = CAST(Bar AS VARCHAR) FROM Foo";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(1, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void TablesWithSameAliasPreferClosest()
		{
			var sql = @"SELECT u.PatientID FROM Foo u UNION ALL SELECT u.DoctorID FROM Bar u";
			var parser = new Parser(false, true, true);
			var results = parser.GetTables(sql);

			Assert.AreEqual(2, results.SelectMany(r => r.Columns).Count());
			Assert.AreEqual("[Foo].[PatientID]", results.SelectMany(r => r.Columns).First().FullyQualifiedNM);
			Assert.AreEqual("[Bar].[DoctorID]", results.SelectMany(r => r.Columns).Last().FullyQualifiedNM);
		}

		[Test]
		public void IifStatement()
		{
			var sql = @"SELECT Result = IIF(A >= .5, p.B, C) FROM Foo p";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(3, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void IncorrectAlias()
		{
			var sql = @"SELECT pa.A FROM Foo p";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(0, results.SelectMany(r => r.Columns).Count());
			Assert.AreEqual(0, results.SelectMany(r => r.PossibleColumns).Count());
		}

		[Test]
		public void InputExpressionOnCaseStatement()
		{
			var sql = @"SELECT CASE e.MemberGenderCD
	    WHEN 'M' THEN CAST('Male' AS varchar(1000))
	    WHEN 'F' THEN CAST('Female' AS varchar(1000))
    ELSE NULL
    END AS GenderDSC
FROM AetnaClaim.Claim.Enrollment e";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(1, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void Unpivot()
		{
			var sql = @"SELECT x.ICDDiagnosisCodes
    FROM AetnaClaim.Claim.ClaimDetail cd
    UNPIVOT 
    (
    ICDDiagnosisCD FOR ICDDiagnosisCodes IN (
        [PrimaryICD9DiagnosisCD]
        ,[ICD9Diagnosis02CD]
        ,[ICD9Diagnosis03CD]
        ,[ICD9Diagnosis04CD]
        ,[ICD9Diagnosis05CD]
        ,[ICD9Diagnosis06CD]
        ,[ICD9Diagnosis07CD]
        ,[ICD9Diagnosis08CD])
    ) AS x";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(2, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void Pivot()
		{
			var sql = @"
    SELECT 
        year(invoiceDate) as [year],left(datename(month,invoicedate),3)as [month], 
        InvoiceAmount as Amount 
    FROM Invoice
PIVOT
(
    SUM(Amount)
    FOR [month] IN (jan, feb, mar, apr, 
    may, jun, jul, aug, sep, oct, nov, dec)
)AS pvt";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(4, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void PivotWithNestedSelect()
		{
			var sql = @"SELECT *
FROM (
    SELECT 
        year(invoiceDate) as [year],left(datename(month,invoicedate),3)as [month], 
        InvoiceAmount as Amount 
    FROM Invoice
) as s
PIVOT
(
    SUM(Amount)
    FOR [month] IN (jan, feb, mar, apr, 
    may, jun, jul, aug, sep, oct, nov, dec)
)AS pvt";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(4, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void InValues()
		{
			var sql = @"SELECT PatientID FROM CatalystDEV.Patient.SocialHistoryBASE social WHERE 'Y' in (CigarettesFLG,PipesFLG,CigarsFLG)";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(4, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void JoinedCteHasColumnAndSoDoesAnotherJoin()
		{
			var sql = @"WITH PatientFilterCount AS (
	SELECT AccountID, p.EventID
	FROM [EpisodeOfCareEventFilterStagingBASE] p
	INNER JOIN Foo f ON f.bID = af.EventID
)
SELECT AccountID, pf.EventID
FROM PatientFilterCount pf
INNER JOIN Bar ON aID = af.EventID";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(3, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void JoinedCteHasColumn()
		{
			var sql = @"WITH PatientFilterCount AS (
	SELECT AccountID, p.EventID
	FROM [EpisodeOfCareEventFilterStagingBASE] p
)
SELECT AccountID, pf.EventID
FROM PatientFilterCount pf";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(2, results.SelectMany(r => r.Columns).Count());
		}
		
		[Test]
		public void JoinedCteDoesntHaveColumn()
		{
			var sql = @"WITH PatientFilterCount AS (
	SELECT p.EventID
	FROM [EpisodeOfCareEventFilterStagingBASE] p
	INNER JOIN Foo f ON f.bID = af.EventID
)
SELECT AccountID, pf.EventID
FROM PatientFilterCount pf
INNER JOIN Bar ON aID = af.EventID";
			var parser = new Parser();
			var results = parser.GetTables(sql);

			Assert.AreEqual(4, results.SelectMany(r => r.Columns).Count());
		}

		[Test]
		public void DatePartUnitIgnored()
		{
			var sql = @"SELECT DATEPART(dw, DateDTS) AS TestDate FROM foo.bar";
			var parser = new Parser();
			var results = parser.GetTables(sql);
			
			Assert.AreEqual(1, results.SelectMany(r => r.Columns).Count());
		}
	}
}
