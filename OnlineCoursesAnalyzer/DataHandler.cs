using System.Globalization;

namespace OnlineCoursesAnalyzer;

public class DataHandler
{
    // Numbering columns from one.
    private static readonly int[] RequiredColumnNumbersFromEducationalAchievementDataFile =
    {
        (int)EducationalAchievementDataFileColumns.Email,
        (int)EducationalAchievementDataFileColumns.SecondName,
        (int)EducationalAchievementDataFileColumns.FirstName,
        (int)EducationalAchievementDataFileColumns.LastName,
        (int)EducationalAchievementDataFileColumns.GradePercent,
    };

    private static readonly int[] RequiredColumnNumbersFromProctoringStatusDataFile =
    {
        (int)ProctoringStatusDataFileColumns.Email,
        (int)ProctoringStatusDataFileColumns.ProctoringStatus,
    };

    private Dictionary<string, Student>? educationalAchievementData;
    private Dictionary<string, string>? proctoringStatusData;

    private enum EducationalAchievementDataFileColumns : int
    {
        Email = 2,
        SecondName = 6,
        FirstName = 5,
        LastName = 4,
        GradePercent = 8,
    }

    private enum ProctoringStatusDataFileColumns : int
    {
        Email = 2,
        ProctoringStatus = 6,
    }

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
        var educationalAchievmentDataList = XLXSParser.GetDataFromColumns(
            fileReadStream, RequiredColumnNumbersFromEducationalAchievementDataFile); /////
        var educationalAchievmentDataDictionary = new Dictionary<string, Student>();
        foreach (var studentData in educationalAchievmentDataList)
        {
            var grade = GetGrade(studentData[4]);
            var student = new Student(studentData[1], studentData[2], studentData[3], grade.ToString());
            educationalAchievmentDataDictionary.Add(studentData[0], student);
        }

        this.educationalAchievementData = educationalAchievmentDataDictionary;
    }

    public void AddProctoringStatusData(Stream fileReadStream)
    {
        var proctoringStatusDataList = XLXSParser.GetDataFromColumns(
            fileReadStream, RequiredColumnNumbersFromProctoringStatusDataFile);
        var proctoringStatusDataDictionary = new Dictionary<string, string>();
        foreach (var studentData in proctoringStatusDataList)
        {
            proctoringStatusDataDictionary.Add(studentData[0], studentData[1]);
        }

        this.proctoringStatusData = proctoringStatusDataDictionary;
    }

    public List<Student>? GetResult()
    {
        if (this.educationalAchievementData == null || this.proctoringStatusData == null)
        {
            return null;
        }

        if (this.educationalAchievementData.Count != this.proctoringStatusData.Count) // Сложность вычисления Count узнать
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
}
