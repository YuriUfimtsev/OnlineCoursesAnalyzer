using System.Text;

namespace OnlineCoursesAnalyzer.Data;

public static class Messages
{
    private static int numberOfErrorRowNumbersDisplayed = 20;

    private static int numberOfErrorStudentEmailsDisplayed = 5;

    public static string ProcessingCompletedSuccessfully { get; } = "Обработка данных завершена корректно.";

    public static string InProcess { get; } = "Обработка данных.";

    public static string IncorrectFileType { get; } = "Тип файла не соответствует '.xlxs'.";

    public static string NotEnoughData { get; } = "Недостаточно данных. Загрузите файлы.";

    public static string MoreProctoringDataThanEducationalAchievmentData { get; }
        = "Не для всех студентов с данными о прохождении прокторинга найдена дополнительная информация.";

    public static string GenerateFileInProcessUploadMessage(string fileName) => $"Файл '{fileName}' загружается";

    public static string GenerateFileSuccessfullyUploadMessage(string fileName)
        => $"Файл '{fileName}' успешно загружен";

    public static string GenerateComplexMessage(string messageBeginning, string messageEnd)
    {
        var message = new StringBuilder(messageBeginning);
        message.Append(' ');
        message.Append(messageEnd);
        return message.ToString();
    }

    public static string GenerateFileUploadWarningMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл загружен. {errorRowNumbers.Count} строк содержат ошибки. Проверьте строки ",
            errorRowNumbers,
            numberOfErrorRowNumbersDisplayed);
        return errorMessage;
    }

    public static string GenerateFileUploadErrorMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл не загружен. Больше {errorRowNumbers.Count - 1} строк содержат ошибки. Проверьте строки ",
            errorRowNumbers,
            numberOfErrorRowNumbersDisplayed);
        return errorMessage;
    }

    public static string GenerateFilesProcessingErrorMessageWithStudentEmails(List<string> studentEmails)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Не найдены данные прохождения прокторинга для более чем {studentEmails.Count - 1} студентов. А именно ",
            studentEmails,
            numberOfErrorStudentEmailsDisplayed);
        return errorMessage;
    }

    public static string GenerateFilesProcessingWarningMessageWithStudentEmails(List<string> studentEmails)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Не найдены данные прохождения прокторинга для {studentEmails.Count} студентов. А именно ",
            studentEmails,
            numberOfErrorStudentEmailsDisplayed);
        return errorMessage;
    }

    public static string GenerateRepeatStudentErrorMessage(string repeatStudentEmail)
        => $"Студент с email-адресом {repeatStudentEmail} найден дважды.";

    public static string GenerateRequiredColumnNameNotFoundErrorMessage(string requiredColumnName)
        => $"Не найден столбец с требуемым названием: {requiredColumnName}.";

    private static string GenerateErrorMessageWithInvalidElements(
        string errorMessageStart,
        List<string> errorRowIdentifiers,
        int allowedNumberToDisplay)
    {
        var errorMessage = new StringBuilder();
        errorMessage.Append(errorMessageStart);
        var displayedRowNumbers = Math.Min(errorRowIdentifiers.Count, allowedNumberToDisplay);
        for (var i = 0; i < displayedRowNumbers; ++i)
        {
            errorMessage.Append(errorRowIdentifiers[i]);
            if (i != displayedRowNumbers - 1)
            {
                errorMessage.Append(", ");
            }
        }

        errorMessage.Append(errorRowIdentifiers.Count > allowedNumberToDisplay ? "..." : '.');
        var stringMessage = errorMessage.ToString();
        return stringMessage;
    }
}
