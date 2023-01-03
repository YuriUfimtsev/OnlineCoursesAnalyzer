namespace OnlineCoursesAnalyzer.DataHandling;

/// <summary>
/// Throws if the user's file doesn't meet the conditions
/// described in the EducationalAchievementFile and ProctoringStatusFile classes.
/// </summary>
public class InvalidInputDataException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidInputDataException"/> class.
    /// </summary>
    /// <param name="message">The message that will be shown to the user.</param>
    public InvalidInputDataException(string message)
        : this(message, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidInputDataException"/> class.
    /// </summary>
    /// <param name="message">The message that will be shown to the user.</param>
    /// <param name="advancedMessage">The message with additional information that will be shown to the user.</param>
    public InvalidInputDataException(string message, string advancedMessage)
        : base(message)
    {
        this.ExternalMessage = advancedMessage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidInputDataException"/> class.
    /// </summary>
    /// <param name="message">The message that will be shown to the user.</param>
    /// <param name="externalMessage">The message with additional information that will be shown to the user.</param>
    /// <param name="innerException">Inner exception.</param>
    public InvalidInputDataException(string message, string externalMessage, Exception innerException)
        : base(message, innerException)
    {
        this.ExternalMessage = externalMessage;
    }

    /// <summary>
    /// Gets the message with additional information.
    /// </summary>
    public string ExternalMessage { get; }
}
