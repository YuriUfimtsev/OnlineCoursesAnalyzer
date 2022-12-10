using System.Diagnostics;
using System.Text;

namespace OnlineCoursesAnalyzer.Data;

public static class Messages
{
    public static string GetMoreInformation { get; } = "Нажмите, чтобы узнать подробности";

    public static string ProcessingCompletedSuccessfully { get; } = "Обработка данных завершена корректно";

    public static string InProcess { get; } = "Обработка данных";

    public static string IncorrectFileType { get; } = "Тип файла не соответствует '.xlxs'";

    public static string NotEnoughData { get; } = "Недостаточно данных. Загрузите файлы";

    public static string UnexpectedEmail { get; } = "Неожиданный формат почтового адреса.";

    public static string AdvancedUnexpectedEmail { get; } = $"Неожиданный формат почтового адреса." +
        $" Проверьте, что все адреса формата {EducationalAchievementFile.ExpectedEmailFormat}.";

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

    public static string GenerateFileUploadWarningMessage(int errorRowsCount)
        => GenerateComplexMessage($"Файл загружен. {errorRowsCount} строк содержат ошибки.", GetMoreInformation);

    public static string GenerateFileUploadWarningMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл загружен. {errorRowNumbers.Count} строк содержат ошибки. Проверьте строки: ",
            errorRowNumbers);
        return errorMessage;
    }

    public static string GenerateFileUploadErrorMessage(int errorRowsCount)
        => GenerateComplexMessage($"Файл не загружен. {errorRowsCount} строк содержат ошибки.", GetMoreInformation);

    public static string GenerateFileUploadErrorMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл не загружен. Больше {errorRowNumbers.Count - 1} строк содержат ошибки. Проверьте строки: ",
            errorRowNumbers);
        return errorMessage;
    }

    public static string GenerateFilesProcessingErrorMessage(int errorStudentsEmails)
        => GenerateComplexMessage(
            $"Обработка прервана. Не найдены данные прохождения прокторинга для {errorStudentsEmails} студентов.",
            GetMoreInformation);

    public static string GenerateFilesProcessingErrorMessageWithStudentEmails(List<string> studentEmails)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Обработка прервана. Не найдены данные прохождения прокторинга для более чем {studentEmails.Count - 1} студентов. А именно: ",
            studentEmails);
        return errorMessage;
    }

    public static string GenerateFilesProcessingWarningMessage(int errorStudentsCount)
        => GenerateComplexMessage(
            $"Не найдены данные прохождения прокторинга для {errorStudentsCount} студентов.",
            GetMoreInformation);

    public static string GenerateFilesProcessingWarningMessageWithStudentEmails(List<string> studentEmails)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Не найдены данные прохождения прокторинга для {studentEmails.Count} студентов. А именно: ",
            studentEmails);
        return errorMessage;
    }

    public static string GenerateRepeatStudentErrorMessage(string repeatStudentEmail)
        => $"Студент с email-адресом {repeatStudentEmail} найден дважды";

    public static string GenerateRequiredColumnNameNotFoundErrorMessage(string requiredColumnName)
        => $"Не найден столбец с требуемым названием: {requiredColumnName}";

    private static string GenerateErrorMessageWithInvalidElements(
        string errorMessageStart,
        List<string> errorRowIdentifiers)
    {
        var errorMessage = new StringBuilder();
        errorMessage.Append(errorMessageStart);
        for (var i = 0; i < errorRowIdentifiers.Count; ++i)
        {
            errorMessage.Append(errorRowIdentifiers[i]);
            if (i != errorRowIdentifiers.Count - 1)
            {
                errorMessage.Append(", ");
            }
        }

        errorMessage.Append('.');
        var stringMessage = errorMessage.ToString();
        return stringMessage;
    }
}
