using OfficeOpenXml;

namespace OnlineCoursesAnalyzer.DataHandling;
public static class XLXSParser
{
    public static List<string[]> GetDataFromColumnsWithoutFirstRow(
        Stream stream,
        string[] requiredColumnNames)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var allRowsData = new List<string[]>();
        using var package = new ExcelPackage(stream);

        var worksheet = package.Workbook.Worksheets[0];
        var requiredColumnNumbers = GetColumnNumbersFromNames(worksheet, requiredColumnNames);
        for (var i = 2; i < worksheet.Dimension.Rows; ++i)
        {
            var (rowData, isNullRow) = GetRowData(worksheet, i, requiredColumnNumbers);
            if (!isNullRow)
            {
                allRowsData.Add(rowData!);
            }
        }

        return allRowsData;
    }

    private static int[] GetColumnNumbersFromNames(ExcelWorksheet sheet, string[] requiredColumnNames)
    {
        var requiredColumnNumbers = new int[requiredColumnNames.Length];
        for (var i = 0; i < requiredColumnNames.Length; ++i)
        {
            var columnNumber = sheet
                .Cells["1:1"]
                .First(c => Equals(c.Value.ToString(), requiredColumnNames[i]))
                .Start
                .Column;
            requiredColumnNumbers[i] = columnNumber;
        }

        return requiredColumnNumbers;
    }

    private static (string?[] RowData, bool IsNullRow) GetRowData(
        ExcelWorksheet sheet,
        int rowNumber,
        int[] requiredColumnNumbers)
    {
        var isNullRow = false;
        var rowData = new string?[requiredColumnNumbers.Length];
        for (var j = 0; j < requiredColumnNumbers.Length; ++j)
        {
            var value = sheet.Cells[rowNumber, requiredColumnNumbers[j]].Value;
            if (value == null)
            {
                isNullRow = true;
                return (rowData, isNullRow); // Сообщать, что есть null-строка
            }

            rowData[j] = value.ToString();
        }

        return (rowData, isNullRow);
    }
}
