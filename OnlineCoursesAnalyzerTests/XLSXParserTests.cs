using OnlineCoursesAnalyzer.DataHandling;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OnlineCoursesAnalyzerTests;

public class XLSXParserTests
{
    private static string GetPathToFile(string fileName)
        => $"Data/{fileName}";

    [Test]
    public void StandartTest()
    {
        var stream = File.OpenRead(GetPathToFile("StandartData.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        var (dataWithRowNumbers, nullRows) = XLSXParser.GetDataWithoutFirstRow(stream, requiredColumnNames, 0);
        var expectedDataWithRowNumbers = new List<string[]>
        {
            new [] { "Анонимов", "Аноним", "Санкт-Петербург", "2" },
            new [] { "Петров", "Петр", "Москва", "3" },
            new [] { "Иванов", "Иван", "Казань", "4" },
        };
        Assert.That(nullRows.Count, Is.EqualTo(0));
        CollectionAssert.AreEquivalent(expectedDataWithRowNumbers, dataWithRowNumbers);
    }

    [Test]
    public void SomeNullRowsInsideTest()
    {
        var stream = File.OpenRead(GetPathToFile("DataWithNullRowsInside.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        var (dataWithRowNumbers, nullRows) = XLSXParser.GetDataWithoutFirstRow(stream, requiredColumnNames, 12);
        var expectedNullRowNumbers = new List<string> { "2", "4", "5", "6", "8", "11" };
        Assert.That(dataWithRowNumbers.Count, Is.EqualTo(5));
        CollectionAssert.AreEquivalent(expectedNullRowNumbers, nullRows);
    }

    [Test]
    public void CompletelyNullRowsAtTheEndOfTheTableTest()
    {
        var stream = File.OpenRead(GetPathToFile("DataWithNullRowsAtTheEndOfTheTable.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        var (dataWithRowNumbers, nullRows) = XLSXParser.GetDataWithoutFirstRow(stream, requiredColumnNames, 12);
        Assert.That(nullRows.Count, Is.EqualTo(0));
        Assert.That(dataWithRowNumbers.Count, Is.EqualTo(11));
    }

    [Test]
    public void NotCompletelyNullRowsAtTheEndOfTheTableTest()
    {
        var stream = File.OpenRead(GetPathToFile("DataWithNotCompletelyNullRowsAtTheEndOfTheTable.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        var (dataWithRowNumbers, nullRows) = XLSXParser.GetDataWithoutFirstRow(stream, requiredColumnNames, 12);
        var expectedNullRowNumbers = new List<string> { "9", "10", "11", "12" };
        CollectionAssert.AreEquivalent(expectedNullRowNumbers, nullRows);
        Assert.That(dataWithRowNumbers.Count, Is.EqualTo(7));
    }

    [Test]
    public void MoreNullRowsThanAllowedTest()
    {
        var stream = File.OpenRead(GetPathToFile("DataWithFiveNullRows.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        Assert.Throws<InvalidInputDataException>(() => XLSXParser.GetDataWithoutFirstRow(
            stream,
            requiredColumnNames,
            4));
    }

    [Test]
    public void NotXLSXFileTest()
    {
        var stream = File.OpenRead(GetPathToFile("Empty.pdf"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        Assert.Throws<InvalidInputDataException>(() => XLSXParser.GetDataWithoutFirstRow(
            stream,
            requiredColumnNames,
            12));
    }

    [Test]
    public void AllowedNumberOfErrorRowsParameterLessThanZeroTest()
    {
        var stream = File.OpenRead(GetPathToFile("StandartData.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город" };
        var (dataWithRowNumbers, nullRows) = XLSXParser.GetDataWithoutFirstRow(stream, requiredColumnNames, -5);
    }

    [Test]
    public void EmptyRequiredColumnNamesParameterTest()
    {
        var stream = File.OpenRead(GetPathToFile("StandartData.xlsx"));
        var requiredColumnNames = new string[] { };
        var (dataWithRowNumbers, nullRows) = XLSXParser.GetDataWithoutFirstRow(stream, requiredColumnNames, 1);
        var expectedData = new List<string[]> { new [] { "2" }, new [] { "3" }, new [] { "4" } };
        CollectionAssert.AreEquivalent(expectedData, dataWithRowNumbers);
        Assert.That(nullRows.Count, Is.EqualTo(0));
    }

    [Test]
    public void RequiredColumnNameIsMissingInTheTableTest()
    {
        var stream = File.OpenRead(GetPathToFile("StandartData.xlsx"));
        var requiredColumnNames = new [] { "Фамилия", "Имя", "Город", "Регион" };
        Assert.Throws<InvalidInputDataException>(() => XLSXParser.GetDataWithoutFirstRow(
            stream,
            requiredColumnNames,
            3));
    }
}