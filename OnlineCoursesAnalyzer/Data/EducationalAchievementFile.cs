namespace OnlineCoursesAnalyzer.Data;

public static class EducationalAchievementFile
{
    // 2 megabytes
    public static int MaxAllowedSizeInBytes { get; } = 2097152;

    public static int AllowedNumberOfErrorRows { get; } = 10;

    public static int AllowedNumberOfStudentsWithoutProctoringStatus { get; } = 10;

    public static string EmailColumn { get; } = "Email";

    public static string SecondNameColumn { get; } = "Second Name";

    public static string FirstNameColumn { get; } = "First Name";

    public static string LastNameColumn { get; } = "Last Name";

    public static string GradePercentColumn { get; } = "Grade percent";

    public static string FacultyNamesColumn { get; } = "Cohort Name";

    public static string MathmechFacultyName { get; } = "матмех";

    public static string ExpectedEmailFormat { get; } = "st000000@student.spbu.ru";
}
