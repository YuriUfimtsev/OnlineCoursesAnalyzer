using System.Text;

namespace OnlineCoursesAnalyzer.Data;

public static class ErrorMessages
{
    private static int numberOfRowNumbersDisplayed = 20;

    private static int numberOfStudentEmailsDisplayed = 5;

    public static string IncorrectFileType { get; } = "Тип файла не соответствует .xlxs";

    public static string NotEnoughData { get; } = "Недостаточно данных. Загрузите файлы.";

    public static string GenerateFileUploadErrorMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл не загружен. Больше {errorRowNumbers.Count - 1} строк содержат ошибки. Проверьте строки ",
            errorRowNumbers,
            numberOfStudentEmailsDisplayed);
        return errorMessage;
    }

    public static string GenerateFilesProcessingErrorMessageWithStudentEmails(List<string> studentEmails)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Не найдены данные прохождения прокторинга для более чем {studentEmails.Count - 1} студентов. А именно ",
            studentEmails,
            numberOfRowNumbersDisplayed);
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
        var displayedRowNumbers = Math.Min(errorRowIdentifiers.Count - 1, allowedNumberToDisplay);
        for (var i = 0; i < displayedRowNumbers; ++i)
        {
            errorMessage.Append(errorRowIdentifiers[i]);
            errorMessage.Append(", ");
        }

        errorMessage.Append('.');
        var stringMessage = errorMessage.ToString();
        return stringMessage;
    }
}
