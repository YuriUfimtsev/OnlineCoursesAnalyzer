namespace OnlineCoursesAnalyzer.Data;

/// <summary>
/// Implements the storage of key data of the educational achievement file.
/// </summary>
public static class EducationalAchievementFile
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
    /// Gets allowed number of students for which proctoring status may not be found.
    /// </summary>
    public static int AllowedNumberOfStudentsWithoutProctoringStatus => 150;

    /// <summary>
    /// Gets the name of the column that contains student email addresses.
    /// </summary>
    public static string EmailColumn => "Email";

    /// <summary>
    /// Gets the name of the column that contains student surnames.
    /// </summary>
    public static string LastNameColumn => "Last Name";

    /// <summary>
    /// Gets the name of the column that contains student names.
    /// </summary>
    public static string FirstNameColumn => "First Name";

    /// <summary>
    /// Gets the name of the column that contains student patronymics.
    /// </summary>
    public static string SecondNameColumn => "Second Name";

    /// <summary>
    /// Gets the name of the column that contains student grade percents.
    /// </summary>
    public static string GradeColumn => "Итоговая аттестация (Avg)";

    /// <summary>
    /// Gets the name of the column that contains student control tasks achievement percents.
    /// </summary>
    public static string ControlTasksAchievementColumn => "Контрольные задания (Avg)";

    /// <summary>
    /// Gets the name of the column that contains names of the faculties.
    /// </summary>
    public static string FacultyNamesColumn => "Cohort Name";

    /// <summary>
    /// Gets the MathMech faculty title in file.
    /// </summary>
    public static string MathMechFacultyName => "матмех";

    /// <summary>
    /// Gets the sample for student email addresses.
    /// </summary>
    public static string ExpectedEmailFormat => "st000000@student.spbu.ru";
}
