namespace OnlineCoursesAnalyzer.Data;

/// <summary>
/// Implements the storage of key data of the proctoring status file.
/// </summary>
public static class ProctoringStatusFile
{
    /// <summary>
    /// Gets maximum estimated file size. 3 megabytes by default.
    /// </summary>
    public static int MaxAllowedSizeInBytes => 3 * 1048576;

    /// <summary>
    /// Gets allowed number of rows containing errors.
    /// </summary>
    public static int AllowedNumberOfErrorRows => 150;

    /// <summary>
    /// Gets the name of the column that contains student email addresses.
    /// </summary>
    public static string EmailColumn => "User email";

    /// <summary>
    /// Gets the name of the column that contains student proctoring statuses.
    /// </summary>
    public static string ProctoringStatusColumn => "Status is correct";

    /// <summary>
    /// Gets the default value corresponding to the successful proctoring passing.
    /// </summary>
    public static string ProctoringStatusIsTrue => "yes";

    /// <summary>
    /// Gets the default value corresponding to the unsuccessful proctoring passing.
    /// </summary>
    public static string ProctoringStatusIsFalse => "no";
}
