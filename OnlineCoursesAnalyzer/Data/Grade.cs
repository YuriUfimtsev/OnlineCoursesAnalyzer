using System.Globalization;

namespace OnlineCoursesAnalyzer.Data;

/// <summary>
/// Implements the storage of actual data of the rating system.
/// </summary>
public static class Grade
{
    /// <summary>
    /// Contains an up-to-date rating system.
    /// </summary>
    public enum Grades
    {
        /// <summary>
        /// ECTS grade A.
        /// </summary>
        A,

        /// <summary>
        /// ECTS grade B.
        /// </summary>
        B,

        /// <summary>
        /// ECTS grade C.
        /// </summary>
        C,

        /// <summary>
        /// ECTS grade D.
        /// </summary>
        D,

        /// <summary>
        /// ECTS grade E.
        /// </summary>
        E,

        /// <summary>
        /// ECTS grade F.
        /// </summary>
        F,
    }

    /// <summary>
    /// Convert grade percent to the grade in up-to-date rating system.
    /// </summary>
    /// <param name="gradePercent">Student grade percent.</param>
    /// <returns>Student grade.</returns>
    public static Grades GetGrade(string gradePercent)
    {
        var value = float.Parse(gradePercent, CultureInfo.InvariantCulture) * 100;
        var percent = (int)Math.Truncate(value);
        return percent switch
        {
            >= 90 => Grades.A,
            >= 80 => Grades.B,
            >= 70 => Grades.C,
            >= 61 => Grades.D,
            >= 50 => Grades.E,
            _ => Grades.F,
        };
    }
}
