namespace OnlineCoursesAnalyzer.DataHandling;

public class InvalidInputDataException : Exception
{
    public InvalidInputDataException(string message)
        : this(message, message)
    {
    }

    public InvalidInputDataException(string message, string advancedMessage)
        : base(message)
    {
        this.ExternalMessage = advancedMessage;
    }

    public InvalidInputDataException(string message, string externalMessage, Exception innerException)
        : base(message, innerException)
    {
        this.ExternalMessage = externalMessage;
    }

    public string ExternalMessage { get; }
}
