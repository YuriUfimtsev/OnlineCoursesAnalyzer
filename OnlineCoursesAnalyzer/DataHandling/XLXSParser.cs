using OfficeOpenXml;
using OnlineCoursesAnalyzer.Data;

namespace OnlineCoursesAnalyzer.DataHandling;
public static class XLXSParser
{
    public static (List<string[]> DataWithRowsNumbers, List<string> NullRowsNumbers) GetDataWithoutFirstRow(
        Stream stream,
        string[] requiredColumnNames)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var nullRowsNumbers = new List<string>();
        var dataWithRowsNumbers = new List<string[]>();
        try
        {
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var requiredColumnNumbers = GetColumnNumbersFromNames(worksheet, requiredColumnNames);
            var nullRowsSubsequenceLength = 0;
            var firstRowNumberInNullRowSubsequence = 0;
            for (var i = 2; i < worksheet.Dimension.Rows; ++i)
            {
                var (rowDataAndNumber, isNullRow) = GetRowData(worksheet, i, requiredColumnNumbers);
                if (!isNullRow)
                {
                    dataWithRowsNumbers.Add(rowDataAndNumber!);
                }
                else
                {
                    if ((firstRowNumberInNullRowSubsequence + nullRowsSubsequenceLength) == i)
                    {
                        nullRowsSubsequenceLength++;
                    }
                    else
                    {
                        for (var j = firstRowNumberInNullRowSubsequence;
                            j < firstRowNumberInNullRowSubsequence + nullRowsSubsequenceLength;
                            ++j)
                        {
                            nullRowsNumbers.Add(i.ToString());
                            if (nullRowsNumbers.Count > EducationalAchievementFile.AllowedNumberOfErrorRows)
                            {
                                throw new InvalidInputDataException(
                                    Messages.GenerateFileUploadErrorMessageWithInvalidRows(nullRowsNumbers));
                            }
                        }

                        firstRowNumberInNullRowSubsequence = i;
                        nullRowsSubsequenceLength = 1;
                    }
                }
            }

            return (dataWithRowsNumbers, nullRowsNumbers);
        }
        catch (InvalidDataException)
        {
            throw new InvalidInputDataException(Messages.IncorrectFileType);
        }
    }

    private static int[] GetColumnNumbersFromNames(ExcelWorksheet sheet, string[] requiredColumnNames)
    {
        var requiredColumnNumbers = new int[requiredColumnNames.Length];
        for (var i = 0; i < requiredColumnNames.Length; ++i)
        {
            try
            {
                var columnNumber = sheet
                    .Cells["1:1"]
                    .First(c => Equals(c.Value.ToString(), requiredColumnNames[i]))
                    .Start
                    .Column;
                requiredColumnNumbers[i] = columnNumber;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidInputDataException(
                    Messages.GenerateRequiredColumnNameNotFoundErrorMessage(requiredColumnNames[i]));
            }
        }

        return requiredColumnNumbers;
    }

    private static (string?[] RowData, bool IsNullRow) GetRowData(
        ExcelWorksheet sheet,
        int rowNumber,
        int[] requiredColumnNumbers)
    {
        var isNullRow = false;
        var rowData = new string?[requiredColumnNumbers.Length + 1];
        for (var j = 0; j < requiredColumnNumbers.Length; ++j)
        {
            var value = sheet.Cells[rowNumber, requiredColumnNumbers[j]].Value;
            if (value == null)
            {
                isNullRow = true;
                return (rowData, isNullRow);
            }

            rowData[j] = value.ToString();
        }

        rowData[rowData.Length - 1] = rowNumber.ToString();

        return (rowData, isNullRow);
    }
}
