using System.Globalization;

namespace OnlineCoursesAnalyzer.Data;

public class Grade
{
    public enum Grades
    {
        A,
        B,
        C,
        D,
        E,
        F,
    }

    public static Grades GetGrade(string gradePercent)
    {
        var percent = (int)Math.Truncate(float.Parse(gradePercent, CultureInfo.InvariantCulture));
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
