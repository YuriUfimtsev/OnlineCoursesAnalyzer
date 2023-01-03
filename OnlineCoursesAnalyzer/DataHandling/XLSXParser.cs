using OfficeOpenXml;
using OnlineCoursesAnalyzer.Data;

namespace OnlineCoursesAnalyzer.DataHandling;

/// <summary>
/// Implements .xlsx files processing.
/// </summary>
public static class XLSXParser
{
    /// <summary>
    /// Converts .xlsx data to a list of values without first row data.
    /// </summary>
    /// <param name="stream">.xlsx file stream.</param>
    /// <param name="requiredColumnNames">Columns defining the resulting data set.</param>
    /// <param name="significantColumnNames">Columns that must contain data for each row.
    /// The row is treated as containing error otherwise.</param>
    /// <param name="allowedNumberOfErrorRows">The number above which processing will be interrupted and an exception thrown.</param>
    /// <returns>List of values and list of row numbers not containing values in significant columns.</returns>
    public static (List<string[]> DataWithRowsNumbers, List<string> NullRowsNumbers) GetDataWithoutFirstRow(
        Stream stream,
        string[] requiredColumnNames,
        string[] significantColumnNames,
        int allowedNumberOfErrorRows) => GetDataByConditionWithoutFirstRowBaseFunction(
            stream,
            requiredColumnNames,
            significantColumnNames,
            false,
            "-1",
            (parameter) => true,
            allowedNumberOfErrorRows);

    /// <summary>
    /// Converts .xlsx data to a list of values without first row data. Selects rows based on the presence of requiredDataInConditionColumn value in conditionColumnName column.
    /// </summary>
    /// <param name="stream">.xlsx file stream.</param>
    /// <param name="requiredColumnNames">Columns defining the resulting data set.</param>
    /// <param name="significantColumnNames">Columns that must contain data for each row.
    /// The row is treated as containing error otherwise.</param>
    /// <param name="conditionColumnName">The column by which the selection will take place.</param>
    /// <param name="cellCondition">Condition for the cell value.</param>
    /// <param name="allowedNumberOfErrorRows">The number above which processing will be interrupted and an exception thrown.</param>
    /// <returns>List of values and list of row numbers not containing values in significant columns.</returns>
    public static (List<string[]> DataWithRowsNumbers, List<string> NullRowsNumbers) GetDataByTheConditionWithoutFirstRow(
        Stream stream,
        string[] requiredColumnNames,
        string[] significantColumnNames,
        string conditionColumnName,
        Func<string, bool> cellCondition,
        int allowedNumberOfErrorRows) => GetDataByConditionWithoutFirstRowBaseFunction(
            stream,
            requiredColumnNames,
            significantColumnNames,
            true,
            conditionColumnName,
            cellCondition,
            allowedNumberOfErrorRows);

    private static (List<string[]> DataWithRowsNumbers, List<string> NullRowsNumbers) GetDataByConditionWithoutFirstRowBaseFunction(
        Stream stream,
        string[] requiredColumnNames,
        string[] significantColumnNames,
        bool isConditionSet,
        string conditionColumnName,
        Func<string, bool> cellCondition,
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
            var significantColumnNumbers = GetColumnNumbersFromNames(worksheet, significantColumnNames);
            var conditionColumnNumber = isConditionSet ? GetColumnNumbersFromNames(worksheet, new[] { conditionColumnName })[0] : -1;
            var completelyNullRowSubsequenceLength = 0;
            var firstRowNumberInCompletelyNullRowSubsequence = 0;
            for (var i = 2; i <= worksheet.Dimension.Rows; ++i)
            {
                var conditionColumnValue = isConditionSet ? worksheet.Cells[i, conditionColumnNumber].Value : null;
                if (!isConditionSet || (isConditionSet && cellCondition((conditionColumnValue ?? string.Empty).ToString()!)))
                {
                    var (rowDataAndNumber, doesRowContainNull, isCompletelyNullRow)
                    = GetRowData(worksheet, i, requiredColumnNumbers, significantColumnNumbers);
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

    private static (string[] RowData, bool IsSignificantValueNull, bool IsCompletelyNullRow) GetRowData(
        ExcelWorksheet sheet,
        int rowNumber,
        int[] requiredColumnNumbers,
        int[] notNullRequiredColumnsNumbers)
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
            else if (notNullRequiredColumnsNumbers.Contains(requiredColumnNumbers[j]))
            {
                doesRowContainNull = true;
                rowData[j] = string.Empty;
            }
            else
            {
                rowData[j] = string.Empty;
            }
        }

        rowData[rowData.Length - 1] = rowNumber.ToString();
        return (rowData, doesRowContainNull, isCompletelyNullRow);
    }
}
