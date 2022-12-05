namespace OnlineCoursesAnalyzer.Data;

public static class ProctoringStatusFile
{
    // 2 megabytes
    public static int MaxAllowedSizeInBytes { get; } = 2097152;

    public static string EmailColumn { get; } = "User";

    public static string ProctoringStatusColumn { get; } = "Status is correct";

    public static string ProctoringStatusIsTrue { get; } = "yes";

    public static string ProctoringStatusIsFalse { get; } = "no";

    public static int AllowedNumberOfErrorRows { get; } = 50;
}
