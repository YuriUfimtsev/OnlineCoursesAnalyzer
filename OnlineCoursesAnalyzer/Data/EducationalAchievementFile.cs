using System.ComponentModel;

namespace OnlineCoursesAnalyzer.Data;

/// <summary>
/// Implements the storage of key data of the educational achievement file.
/// </summary>
public static class EducationalAchievementFile
{
    /// <summary>
    /// Gets maximum estimated file size. 2 megabytes by default.
    /// </summary>
    public static int MaxAllowedSizeInBytes { get; } = 2097152;

    /// <summary>
    /// Gets allowed number of rows containing errors.
    /// </summary>
    public static int AllowedNumberOfErrorRows { get; } = 10;

    /// <summary>
    /// Gets allowed number of students for which proctoring status may not be found.
    /// </summary>
    public static int AllowedNumberOfStudentsWithoutProctoringStatus { get; } = 10;

    /// <summary>
    /// Gets the name of the column that contains student email addresses.
    /// </summary>
    public static string EmailColumn { get; } = "Email";

    /// <summary>
    /// Gets the name of the column that contains student surnames.
    /// </summary>
    public static string SecondNameColumn { get; } = "Second Name";

    /// <summary>
    /// Gets the name of the column that contains student names.
    /// </summary>
    public static string FirstNameColumn { get; } = "First Name";

    /// <summary>
    /// Gets the name of the column that contains student patronymics.
    /// </summary>
    public static string LastNameColumn { get; } = "Last Name";

    /// <summary>
    /// Gets the name of the column that contains student grade percents.
    /// </summary>
    public static string GradePercentColumn { get; } = "Grade percent";

    /// <summary>
    /// Gets the name of the column that contains names of the faculties.
    /// </summary>
    public static string FacultyNamesColumn { get; } = "Cohort Name";

    /// <summary>
    /// Gets the MathMech faculty title in file.
    /// </summary>
    public static string MathMechFacultyName { get; } = "матмех";

    /// <summary>
    /// Gets the sample for student email addresses.
    /// </summary>
    public static string ExpectedEmailFormat { get; } = "st000000@student.spbu.ru";
}
