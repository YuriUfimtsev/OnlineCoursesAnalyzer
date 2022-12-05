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
    private Dictionary<string, bool>? proctoringStatusData;
    private List<(Student, bool)>? studentsDataWithExplicitProctoringStatus;
    private bool isNotNullDataActual;

    public bool ISProctoringDataMoreThanEducationalAchievmentData
    {
        get
        {
            return this.educationalAchievementData == null || this.proctoringStatusData == null ? false : this.proctoringStatusData!.Count > this.educationalAchievementData!.Count;
        }
    }

    public List<string> AddEducationalAchievementData(Stream fileReadStream)
    {
        var (educationalAchievmentDataList, errorRows) = XLXSParser.GetDataWithoutFirstRow(
            fileReadStream, RequiredDataFromEducationalAchievementFile);
        var educationalAchievmentDataDictionary = new Dictionary<string, Student>();
        foreach (var rowData in educationalAchievmentDataList)
        {
            try
            {
                var grade = Grade.GetGrade(rowData[4]);
                var student = new Student(rowData[1], rowData[2], rowData[3], grade.ToString());
                educationalAchievmentDataDictionary.Add(rowData[0], student);
            }
            catch (FormatException)
            {
                errorRows.Add(rowData[rowData.Length - 1]);
            }
            catch (ArgumentException)
            {
                throw new InvalidInputDataException(
                    Messages.GenerateRepeatStudentErrorMessage(rowData[0]));
            }

            if (errorRows.Count > EducationalAchievementFile.AllowedNumberOfErrorRows)
            {
                throw new InvalidInputDataException(
                    Messages.GenerateFileUploadErrorMessageWithInvalidRows(errorRows));
            }
        }

        this.educationalAchievementData = educationalAchievmentDataDictionary;
        this.isNotNullDataActual = false;

        // errorRows.Sort();
        return errorRows;
    }

    public List<string> AddProctoringStatusData(Stream fileReadStream)
    {
        var (proctoringStatusDataList, errorRows) = XLXSParser.GetDataWithoutFirstRow(
            fileReadStream, RequiredDataFromProctoringStatusFile);
        var proctoringStatusDataDictionary = new Dictionary<string, bool>();
        foreach (var studentData in proctoringStatusDataList)
        {
            var (isProctoringDataCorrect, proctoringData) = InterpretProctoringStatus(studentData[1]);
            if (!isProctoringDataCorrect)
            {
                errorRows.Add(studentData[studentData.Length - 1]);
                if (errorRows.Count > ProctoringStatusFile.AllowedNumberOfErrorRows)
                {
                    throw new InvalidInputDataException(
                        Messages.GenerateFileUploadErrorMessageWithInvalidRows(errorRows));
                }
            }
            else
            {
                proctoringStatusDataDictionary.Add(studentData[0], proctoringData);
            }
        }

        this.proctoringStatusData = proctoringStatusDataDictionary;
        this.isNotNullDataActual = false;

        // errorRows.Sort();
        return errorRows;
    }

    public (List<(Student Student, bool IsProctoringCorrect)> StudentsData,
        List<string> StudentsWithoutProctoringEmails) GetResultWithExplicitProctoringStatus()
    {
        var studentWithoutProctoringEmails = new List<string>();
        if (this.isNotNullDataActual)
        {
            return (this.studentsDataWithExplicitProctoringStatus!, studentWithoutProctoringEmails);
        }

        if (this.educationalAchievementData == null || this.proctoringStatusData == null)
        {
            throw new InvalidInputDataException(Messages.NotEnoughData);
        }

        var studentsData = new List<(Student, bool)>();
        foreach (var student in this.educationalAchievementData)
        {
            if (this.proctoringStatusData.ContainsKey(student.Key))
            {
                student.Value.ProctoringStatus = this.proctoringStatusData[student.Key]
                    ? ProctoringStatusFile.ProctoringStatusIsTrue : ProctoringStatusFile.ProctoringStatusIsFalse;
                studentsData.Add((student.Value, this.proctoringStatusData[student.Key]));
            }
            else
            {
                studentWithoutProctoringEmails.Add(student.Key);
                if (studentWithoutProctoringEmails.Count
                    > EducationalAchievementFile.AllowedNumberOfStudentsWithoutProctoringStatus)
                {
                    throw new InvalidInputDataException(
                        Messages.GenerateFilesProcessingErrorMessageWithStudentEmails(
                            studentWithoutProctoringEmails));
                }
            }
        }

        studentsData.Sort((firstElement, secondElement)
            => firstElement.Item1.SecondName.CompareTo(secondElement.Item1.SecondName));
        this.studentsDataWithExplicitProctoringStatus = studentsData;
        this.isNotNullDataActual = true;

        // if (this.educationalAchievementData.Count < this.proctoringStatusData.Count)
        // {
        //    ErrorMessages.MoreProctoringDataThanEducationalAchievmentData;
        // }

        // studentWithoutProctoringEmails.Sort();
        return (studentsData, studentWithoutProctoringEmails);
    }

    private static (bool IsInterpretationCorrect, bool InterpretationResult) InterpretProctoringStatus(
        string proctoringData)
    {
        var isInterpretationCorrect = true;
        var interpretationResult = proctoringData == ProctoringStatusFile.ProctoringStatusIsTrue;
        if (interpretationResult || (!interpretationResult && proctoringData == ProctoringStatusFile.ProctoringStatusIsFalse))
        {
            return (isInterpretationCorrect, interpretationResult);
        }
        else
        {
            return (!isInterpretationCorrect, interpretationResult);
        }
    }
}