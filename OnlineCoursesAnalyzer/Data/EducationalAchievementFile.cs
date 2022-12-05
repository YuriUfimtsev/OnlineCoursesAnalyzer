﻿namespace OnlineCoursesAnalyzer.Data;

public static class EducationalAchievementFile
{
    public static string EmailColumn { get; } = "Email";

    public static string SecondNameColumn { get; } = "Second Name";

    public static string FirstNameColumn { get; } = "First Name";

    public static string LastNameColumn { get; } = "Last Name";

    public static string GradePercentColumn { get; } = "Grade percent";

    public static int AllowedNumberOfErrorRows { get; } = 50;

    public static int AllowedNumberOfStudentsWithoutProctoringStatus { get; } = 50;

    // 2 megabytes
    public static int MaxAllowedSizeInBytes { get; } = 2097152;
}
