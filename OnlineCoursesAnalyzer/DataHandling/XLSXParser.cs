using OfficeOpenXml;
using OnlineCoursesAnalyzer.Data;

namespace OnlineCoursesAnalyzer.DataHandling;

public static class XLSXParser
{
    public static (List<string[]> DataWithRowsNumbers, List<string> NullRowsNumbers) GetDataWithoutFirstRow(
        Stream stream,
        string[] requiredColumnNames,
        int allowedNumberOfErrorRows)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        allowedNumberOfErrorRows = allowedNumberOfErrorRows >= 0 ? allowedNumberOfErrorRows : 0;
        var nullRowsNumbers = new List<string>();
        var dataWithRowsNumbers = new List<string[]>();
        try
        {
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            var requiredColumnNumbers = GetColumnNumbersFromNames(worksheet, requiredColumnNames);
            var completelyNullRowSubsequenceLength = 0;
            var firstRowNumberInCompletelyNullRowSubsequence = 0;
            for (var i = 2; i <= worksheet.Dimension.Rows; ++i)
            {
                var (rowDataAndNumber, doesRowContainNull, isCompletelyNullRow)
                    = GetRowData(worksheet, i, requiredColumnNumbers);
                if (!doesRowContainNull)
                {
                    dataWithRowsNumbers.Add(rowDataAndNumber!);
                }
                else if (!isCompletelyNullRow)
                {
                    nullRowsNumbers.Add(i.ToString());
                    CheckNullRowNumbersListCount(allowedNumberOfErrorRows, nullRowsNumbers);
                }
                else
                {
                    if ((firstRowNumberInCompletelyNullRowSubsequence + completelyNullRowSubsequenceLength) == i)
                    {
                        completelyNullRowSubsequenceLength++;
                    }
                    else
                    {
                        for (var j = firstRowNumberInCompletelyNullRowSubsequence;
                            j < firstRowNumberInCompletelyNullRowSubsequence + completelyNullRowSubsequenceLength;
                            ++j)
                        {
                            nullRowsNumbers.Add(j.ToString());
                            CheckNullRowNumbersListCount(allowedNumberOfErrorRows, nullRowsNumbers);
                        }

                        firstRowNumberInCompletelyNullRowSubsequence = i;
                        completelyNullRowSubsequenceLength = 1;
                    }
                }
            }

            if ((firstRowNumberInCompletelyNullRowSubsequence + completelyNullRowSubsequenceLength - 1)
                != worksheet.Dimension.Rows)
            {
                for (var j = firstRowNumberInCompletelyNullRowSubsequence;
                    j < firstRowNumberInCompletelyNullRowSubsequence + completelyNullRowSubsequenceLength;
                    ++j)
                {
                    nullRowsNumbers.Add(j.ToString());
                    CheckNullRowNumbersListCount(allowedNumberOfErrorRows, nullRowsNumbers);
                }
            }

            return (dataWithRowsNumbers, nullRowsNumbers);
        }
        catch (InvalidDataException)
        {
            throw new InvalidInputDataException(Messages.IncorrectFileType);
        }
    }

    private static void CheckNullRowNumbersListCount(int allowedNumberOfErrorRows, List<string> nullRowsNumbers)
    {
        if (nullRowsNumbers.Count > allowedNumberOfErrorRows)
        {
            throw new InvalidInputDataException(
                Messages.GenerateFileUploadErrorMessage(nullRowsNumbers.Count),
                Messages.GenerateFileUploadErrorMessageWithInvalidRows(nullRowsNumbers));
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
                throw new InvalidInputDataException(Messages.GenerateRequiredColumnNameNotFoundErrorMessage(
                    requiredColumnNames[i]));
            }
        }

        return requiredColumnNumbers;
    }

    private static (string[] RowData, bool DoesRowContainNull, bool IsCompletelyNullRow) GetRowData(
        ExcelWorksheet sheet,
        int rowNumber,
        int[] requiredColumnNumbers)
    {
        var doesRowContainNull = false;
        var isCompletelyNullRow = true;
        var rowData = new string[requiredColumnNumbers.Length + 1];
        for (var j = 0; j < requiredColumnNumbers.Length; ++j)
        {
            var value = sheet.Cells[rowNumber, requiredColumnNumbers[j]].Value;
            if (value != null)
            {
                rowData[j] = value.ToString()!;
                isCompletelyNullRow = false;
            }
            else
            {
                doesRowContainNull = true;
            }
        }

        rowData[rowData.Length - 1] = rowNumber.ToString();

        return (rowData, doesRowContainNull, isCompletelyNullRow);
    }
}
