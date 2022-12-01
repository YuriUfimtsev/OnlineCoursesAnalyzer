using System.Globalization;

namespace OnlineCoursesAnalyzer;

public class DataHandler
{
    private static readonly string[] RequiredColumnsFromEducationalAchievementDataFile =
    {
        EducationalAchievementDataFile.Email,
        EducationalAchievementDataFile.SecondName,
        EducationalAchievementDataFile.FirstName,
        EducationalAchievementDataFile.LastName,
        EducationalAchievementDataFile.GradePercent,
    };

    private static readonly string[] RequiredColumnsFromProctoringStatusDataFile =
    {
        ProctoringStatusDataFile.Email,
        ProctoringStatusDataFile.ProctoringStatus,
    };

    private Dictionary<string, Student>? educationalAchievementData;
    private Dictionary<string, string>? proctoringStatusData;
    private List<Student>? studentsData;
    private bool isNotNullDataActual;

    private enum Grades
    {
        A,
        B,
        C,
        D,
        E,
        F,
    }

    public void AddEducationalAchievementData(Stream fileReadStream)
    {
        var educationalAchievmentDataList = XLXSParser.GetDataFromColumnsWithoutFirstRow(
            fileReadStream, RequiredColumnsFromEducationalAchievementDataFile);
        var educationalAchievmentDataDictionary = new Dictionary<string, Student>();
        foreach (var studentData in educationalAchievmentDataList)
        {
            var grade = GetGrade(studentData[4]);
            var student = new Student(studentData[1], studentData[2], studentData[3], grade.ToString());
            educationalAchievmentDataDictionary.Add(studentData[0], student);
        }

        this.educationalAchievementData = educationalAchievmentDataDictionary;
        this.isNotNullDataActual = false;
    }

    public void AddProctoringStatusData(Stream fileReadStream)
    {
        var proctoringStatusDataList = XLXSParser.GetDataFromColumnsWithoutFirstRow(
            fileReadStream, RequiredColumnsFromProctoringStatusDataFile);
        var proctoringStatusDataDictionary = new Dictionary<string, string>();
        foreach (var studentData in proctoringStatusDataList)
        {
            proctoringStatusDataDictionary.Add(studentData[0], studentData[1]);
        }

        this.proctoringStatusData = proctoringStatusDataDictionary;
        this.isNotNullDataActual = false;
    }

    public List<Student>? GetResult()
    {
        if (this.isNotNullDataActual)
        {
            return this.studentsData;
        }

        if (this.educationalAchievementData == null || this.proctoringStatusData == null)
        {
            return null;
        }

        if (this.educationalAchievementData.Count != this.proctoringStatusData.Count)
        {
            // do smth
        }

        var studentsData = new List<Student>();
        foreach (var student in this.educationalAchievementData)
        {
            if (this.proctoringStatusData.ContainsKey(student.Key))
            {
                student.Value.ProctoringStatus = this.proctoringStatusData[student.Key];
            }

            studentsData.Add(student.Value);
        }

        studentsData.Sort((firstElement, secondElement)
            => firstElement.SecondName.CompareTo(secondElement.SecondName));
        this.studentsData = studentsData;
        this.isNotNullDataActual = true;
        return studentsData;
    }

    private static Grades GetGrade(string gradePercent)
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

    private static class EducationalAchievementDataFile
    {
        public static string Email => "Email";

        public static string SecondName => "Second Name";

        public static string FirstName => "First Name";

        public static string LastName => "Last Name";

        public static string GradePercent => "Grade percent";
    }

    private static class ProctoringStatusDataFile
    {
        public static string Email => "User";

        public static string ProctoringStatus => "Status is correct";
    }
}
