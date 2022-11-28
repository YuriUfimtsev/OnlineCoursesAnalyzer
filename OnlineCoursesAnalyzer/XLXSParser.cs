using OfficeOpenXml;

namespace OnlineCoursesAnalyzer;
public static class XLXSParser
{
    public static List<string[]> GetDataFromColumns(Stream stream, int[] requiredColumnNumbers)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var allRowsData = new List<string[]>();
        using var package = new ExcelPackage(stream);

        var worksheet = package.Workbook.Worksheets[0];
        for (var i = 2 /* Считаем первую строчку - названиями колонок, поэтому парсим со второй.*/;
            i < worksheet.Dimension.Rows; ++i)
        {
            var (rowData, isNullRow) = GetRowData(worksheet, i, requiredColumnNumbers);
            if (!isNullRow)
            {
                allRowsData.Add(rowData!);
            }
        }

        return allRowsData;
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
            rowData[j] = sheet.Cells[rowNumber, requiredColumnNumbers[j]].Value.ToString();
            if (rowData[j] == null)
            {
                isNullRow = true;
            }
        }

        return (rowData, isNullRow);
    }
}
