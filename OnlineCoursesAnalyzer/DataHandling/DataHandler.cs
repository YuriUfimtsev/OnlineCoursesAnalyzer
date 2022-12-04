using System.Globalization;
using OnlineCoursesAnalyzer.Data;

namespace OnlineCoursesAnalyzer.DataHandling;

public class DataHandler
{
    private static readonly string[] RequiredDataFromEducationalAchievementFile =
    {
        EducationalAchievementFile.EmailColumn,
        EducationalAchievementFile.SecondNameColumn,
        EducationalAchievementFile.FirstNameColumn,
        EducationalAchievementFile.LastNameColumn,
        EducationalAchievementFile.GradePercentColumn,
    };

    private static readonly string[] RequiredDataFromProctoringStatusFile =
    {
        ProctoringStatusFile.EmailColumn,
        ProctoringStatusFile.ProctoringStatusColumn,
    };

    private Dictionary<string, Student>? educationalAchievementData;
    private Dictionary<string, string>? proctoringStatusData;
    private List<(Student, bool)>? studentsDataWithExplicitProctoringStatus;
    private bool isNotNullDataActual;

    public void AddEducationalAchievementData(Stream fileReadStream)
    {
        var educationalAchievmentDataList = XLXSParser.GetDataFromColumnsWithoutFirstRow(
            fileReadStream, RequiredDataFromEducationalAchievementFile);
        var educationalAchievmentDataDictionary = new Dictionary<string, Student>();
        foreach (var studentData in educationalAchievmentDataList)
        {
            var grade = Grade.GetGrade(studentData[4]);
            var student = new Student(studentData[1], studentData[2], studentData[3], grade.ToString());
            educationalAchievmentDataDictionary.Add(studentData[0], student);
        }

        this.educationalAchievementData = educationalAchievmentDataDictionary;
        this.isNotNullDataActual = false;
    }

    public void AddProctoringStatusData(Stream fileReadStream)
    {
        var proctoringStatusDataList = XLXSParser.GetDataFromColumnsWithoutFirstRow(
            fileReadStream, RequiredDataFromProctoringStatusFile);
        var proctoringStatusDataDictionary = new Dictionary<string, string>();
        foreach (var studentData in proctoringStatusDataList)
        {
            proctoringStatusDataDictionary.Add(studentData[0], studentData[1]);
        }

        this.proctoringStatusData = proctoringStatusDataDictionary;
        this.isNotNullDataActual = false;
    }

    public List<(Student, bool)>? GetResultWithExplicitProctoringStatus()
    {
        if (this.isNotNullDataActual)
        {
            return this.studentsDataWithExplicitProctoringStatus;
        }

        if (this.educationalAchievementData == null || this.proctoringStatusData == null)
        {
            return null;
        }

        if (this.educationalAchievementData.Count != this.proctoringStatusData.Count)
        {
            // do smth
        }

        var studentsData = new List<(Student, bool)>();
        foreach (var student in this.educationalAchievementData)
        {
            if (this.proctoringStatusData.ContainsKey(student.Key))
            {
                student.Value.ProctoringStatus = this.proctoringStatusData[student.Key];
            }

            var proctoringData = InterpretProctoringStatus(student.Value, studentsData.Count);
            studentsData.Add((student.Value, proctoringData));
        }

        studentsData.Sort((firstElement, secondElement)
            => firstElement.Item1.SecondName.CompareTo(secondElement.Item1.SecondName));
        this.studentsDataWithExplicitProctoringStatus = studentsData;
        this.isNotNullDataActual = true;

        return studentsData;
    }

    private static bool InterpretProctoringStatus(Student studentData, int count)
    {
        return studentData.ProctoringStatus switch
        {
            ProctoringStatusFile.ProctoringStatusIsTrue => true,
            ProctoringStatusFile.ProctoringStatusIsFalse => false,
            _ => throw new Exception(), ////
        };
    }
}
