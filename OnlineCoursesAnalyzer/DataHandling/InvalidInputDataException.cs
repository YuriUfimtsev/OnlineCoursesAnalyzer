namespace OnlineCoursesAnalyzer.DataHandling;

public class InvalidInputDataException : Exception
{
    public InvalidInputDataException(string message)
        : base(message)
    {
    }

    public InvalidInputDataException(string message, Exception innerException)
    : base(message, innerException)
    {
    }
}
