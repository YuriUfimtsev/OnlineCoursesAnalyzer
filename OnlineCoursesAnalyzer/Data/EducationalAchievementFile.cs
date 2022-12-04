namespace OnlineCoursesAnalyzer.Data;

public static class EducationalAchievementFile
{
    public static string EmailColumn { get; } = "Email";

    public static string SecondNameColumn { get; } = "Second Name";

    public static string FirstNameColumn { get; } = "First Name";

    public static string LastNameColumn { get; } = "Last Name";

    public static string GradePercentColumn { get; } = "Grade percent";

    public static int AllowedNumberOfErrorRows { get; } = 50;
}
