using System.Text;

namespace OnlineCoursesAnalyzer.Data;

/// <summary>
/// Implements the storage of messages for the user.
/// </summary>
public static class Messages
{
    /// <summary>
    /// Gets "Get more information" message.
    /// </summary>
    public static string GetMoreInformation { get; } = "Нажмите, чтобы узнать подробности";

    /// <summary>
    /// Gets "Processing completed successfully" message.
    /// </summary>
    public static string ProcessingCompletedSuccessfully { get; } = "Обработка данных завершена корректно";

    /// <summary>
    /// Gets "Processing continues" message.
    /// </summary>
    public static string InProcess { get; } = "Обработка данных";

    /// <summary>
    /// Gets "Incorrect file type" message.
    /// </summary>
    public static string IncorrectFileType { get; } = "Тип файла не соответствует '.xlxs'";

    /// <summary>
    /// Gets "Insufficient data" message.
    /// </summary>
    public static string NotEnoughData { get; } = "Недостаточно данных. Загрузите файлы";

    /// <summary>
    /// Gets "Unexpected format of the email address" message.
    /// </summary>
    public static string UnexpectedEmailFormat { get; } = "Неожиданный формат почтового адреса.";

    /// <summary>
    /// Gets "Unexpected format of the email address" message with additional data.
    /// </summary>
    public static string AdvancedUnexpectedEmail { get; } = $"Неожиданный формат почтового адреса." +
        $" Проверьте, что все адреса формата {EducationalAchievementFile.ExpectedEmailFormat}.";

    /// <summary>
    /// Generates "The file is being uploaded" message based on the file name.
    /// </summary>
    /// <param name="fileName">File that is being uploaded name.</param>
    /// <returns>Message with the file name.</returns>
    public static string GenerateFileInProcessUploadMessage(string fileName) => $"Файл '{fileName}' загружается";

    /// <summary>
    /// Generates "The file has been uploaded successfully" message based on the file name.
    /// </summary>
    /// <param name="fileName">File that has been uploaded successfully.</param>
    /// <returns>Message with the file name.</returns>
    public static string GenerateFileSuccessfullyUploadMessage(string fileName)
        => $"Файл '{fileName}' успешно загружен";

    /// <summary>
    /// Generates "The file has been uploaded with a warning" message based on the number of rows with errors.
    /// </summary>
    /// <param name="errorRowsCount">Number of rows with errors.</param>
    /// <returns>Message with the number of error rows.</returns>
    public static string GenerateFileUploadWarningMessage(int errorRowsCount)
        => GenerateComplexMessage($"Файл загружен. {errorRowsCount} строк содержат ошибки.", GetMoreInformation);

    /// <summary>
    /// Generates "The file has been uploaded with a warning" message with a list of row numbers containing errors.
    /// </summary>
    /// <param name="errorRowNumbers">Number of rows containing errors.</param>
    /// <returns>Message with a list of row numbers containing errors.</returns>
    public static string GenerateFileUploadWarningMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл загружен. {errorRowNumbers.Count} строк содержат ошибки. Проверьте строки: ",
            errorRowNumbers);
        return errorMessage;
    }

    /// <summary>
    /// Generates "The file hasn't been uploaded" message based on the number of rows with errors.
    /// </summary>
    /// <param name="errorRowsCount">Number of rows with errors.</param>
    /// <returns>Message with the number of error rows.</returns>
    public static string GenerateFileUploadErrorMessage(int errorRowsCount)
        => GenerateComplexMessage($"Файл не загружен. {errorRowsCount} строк содержат ошибки.", GetMoreInformation);

    /// <summary>
    /// Generates "The file hasn't been uploaded" message with a list of row numbers containing errors.
    /// </summary>
    /// <param name="errorRowNumbers">Number of rows containing errors.</param>
    /// <returns>Message with a list of row numbers containing errors.</returns>
    public static string GenerateFileUploadErrorMessageWithInvalidRows(List<string> errorRowNumbers)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Файл не загружен. Больше {errorRowNumbers.Count - 1} строк содержат ошибки. Проверьте строки: ",
            errorRowNumbers);
        return errorMessage;
    }

    /// <summary>
    /// Generates the "Processing interrupted" message based on the number of students without proctoring status.
    /// </summary>
    /// <param name="errorStudentsCount">Number of students without proctoring ststus.</param>
    /// <returns>Message with the number of students without proctoring status.</returns>
    public static string GenerateFilesProcessingErrorMessage(int errorStudentsCount)
        => GenerateComplexMessage(
            $"Обработка прервана. Не найдены данные прохождения прокторинга для {errorStudentsCount} студентов.",
            GetMoreInformation);

    /// <summary>
    /// Generates the "Processing interrupted" message with a list of the email addresses of students without proctoring status.
    /// </summary>
    /// <param name="studentEmailAddresses">Email addresses of students without proctoring status.</param>
    /// <returns>Messsage with a list of the email addresses of students without proctoring status.</returns>
    public static string GenerateFilesProcessingErrorMessageWithStudentEmails(List<string> studentEmailAddresses)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Обработка прервана. Не найдены данные прохождения прокторинга для более чем {studentEmailAddresses.Count - 1} студентов. А именно: ",
            studentEmailAddresses);
        return errorMessage;
    }

    /// <summary>
    /// Generates "Processing has been completed with a warning" message based on the number of students without proctoring status.
    /// </summary>
    /// <param name="errorStudentsCount">Number of students without proctoring status.</param>
    /// <returns>Message with the number of students without proctoring status.</returns>
    public static string GenerateFilesProcessingWarningMessage(int errorStudentsCount)
        => GenerateComplexMessage(
            $"Обработка завершена. Не найдены данные прохождения прокторинга для {errorStudentsCount} студентов.",
            GetMoreInformation);

    /// <summary>
    /// Generates "Processing has been completed with a warning" message with a list of the email addresses of students without proctoring status.
    /// </summary>
    /// <param name="studentEmailAddresses">Email addresses of students without proctoring status.</param>
    /// <returns>Messsage with a list of the email addresses of students without proctoring status.</returns>
    public static string GenerateFilesProcessingWarningMessageWithStudentEmails(List<string> studentEmailAddresses)
    {
        var errorMessage = GenerateErrorMessageWithInvalidElements(
            $"Обработка завершена. Не найдены данные прохождения прокторинга для {studentEmailAddresses.Count} студентов. А именно: ",
            studentEmailAddresses);
        return errorMessage;
    }

    /// <summary>
    /// Generates "Error. Duplicate data for the student" message based on repeated student email address.
    /// </summary>
    /// <param name="repeatStudentEmail">Repeated student email address.</param>
    /// <returns>Message with the repeated student email address.</returns>
    public static string GenerateRepeatStudentErrorMessage(string repeatStudentEmail)
        => $"Ошибка. Студент с email-адресом {repeatStudentEmail} найден дважды";

    /// <summary>
    /// Generates "Error. A column with the required name hasn't been found" with the required name.
    /// </summary>
    /// <param name="requiredColumnName">Required column name.</param>
    /// <returns>Message with the required column name.</returns>
    public static string GenerateRequiredColumnNameNotFoundErrorMessage(string requiredColumnName)
        => $"Ошибка. Не найден столбец с требуемым названием: {requiredColumnName}";

    /// <summary>
    /// Generates a complex message based on two messages.
    /// </summary>
    /// <param name="messageBeginning">First message.</param>
    /// <param name="messageEnd">Second message.</param>
    /// <returns>The complex message.</returns>
    public static string GenerateComplexMessage(string messageBeginning, string messageEnd)
    {
        var message = new StringBuilder(messageBeginning);
        message.Append(' ');
        message.Append(messageEnd);
        return message.ToString();
    }

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
