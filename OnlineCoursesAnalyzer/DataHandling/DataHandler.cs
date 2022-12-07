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

    private List<(Student, bool)>? studentsDataWithExplicitProctoringStatus;
    private bool isNotNullDataActual;

    public Dictionary<string, Student>? EducationalAchievementData { get; private set; }

    public Dictionary<string, bool>? ProctoringStatusData { get; private set; }

    public bool IsNotNullProctoringDataMoreThanNotNullEducationalAchievmentData
    {
        get
        {
            return this.EducationalAchievementData == null || this.ProctoringStatusData == null
                ? false : this.ProctoringStatusData!.Count > this.EducationalAchievementData!.Count;
        }
    }

    public List<string> AddEducationalAchievementData(Stream fileReadStream)
    {
        var (educationalAchievmentDataList, errorRows) = XLSXParser.GetDataWithoutFirstRow(
            fileReadStream,
            RequiredDataFromEducationalAchievementFile,
            EducationalAchievementFile.AllowedNumberOfErrorRows);
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
                throw new InvalidInputDataException(Messages.GenerateRepeatStudentErrorMessage(rowData[0]));
            }

            if (errorRows.Count > EducationalAchievementFile.AllowedNumberOfErrorRows)
            {
                throw new InvalidInputDataException(
                    Messages.GenerateFileUploadErrorMessage(errorRows.Count),
                    Messages.GenerateFileUploadErrorMessageWithInvalidRows(errorRows));
            }
        }

        this.EducationalAchievementData = educationalAchievmentDataDictionary;
        this.isNotNullDataActual = false;
        return errorRows;
    }

    public List<string> AddProctoringStatusData(Stream fileReadStream)
    {
        var (proctoringStatusDataList, errorRows) = XLSXParser.GetDataWithoutFirstRow(
            fileReadStream,
            RequiredDataFromProctoringStatusFile,
            ProctoringStatusFile.AllowedNumberOfErrorRows);
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
                        Messages.GenerateFileUploadErrorMessage(errorRows.Count),
                        Messages.GenerateFileUploadErrorMessageWithInvalidRows(errorRows));
                }
            }
            else
            {
                if (proctoringStatusDataDictionary.ContainsKey(studentData[0]))
                {
                    throw new InvalidInputDataException(Messages.GenerateRepeatStudentErrorMessage(studentData[0]));
                }

                proctoringStatusDataDictionary.Add(studentData[0], proctoringData);
            }
        }

        this.ProctoringStatusData = proctoringStatusDataDictionary;
        this.isNotNullDataActual = false;
        return errorRows;
    }

    public (List<(Student Student, bool ProctoringStatus)> StudentsData,
        List<string> StudentsWithoutProctoringEmails) GetResultWithExplicitProctoringStatus()
    {
        var studentWithoutProctoringEmails = new List<string>();
        if (this.isNotNullDataActual)
        {
            return (this.studentsDataWithExplicitProctoringStatus!, studentWithoutProctoringEmails);
        }

        if (this.EducationalAchievementData == null || this.ProctoringStatusData == null)
        {
            throw new InvalidInputDataException(Messages.NotEnoughData);
        }

        var studentsData = new List<(Student, bool)>();
        foreach (var student in this.EducationalAchievementData)
        {
            if (this.ProctoringStatusData.ContainsKey(student.Key))
            {
                student.Value.ProctoringStatus = this.ProctoringStatusData[student.Key]
                    ? ProctoringStatusFile.ProctoringStatusIsTrue : ProctoringStatusFile.ProctoringStatusIsFalse;
                studentsData.Add((student.Value, this.ProctoringStatusData[student.Key]));
            }
            else
            {
                studentWithoutProctoringEmails.Add(student.Key);
                if (studentWithoutProctoringEmails.Count
                    > EducationalAchievementFile.AllowedNumberOfStudentsWithoutProctoringStatus)
                {
                    throw new InvalidInputDataException(
                        Messages.GenerateFilesProcessingErrorMessage(studentWithoutProctoringEmails.Count),
                        Messages.GenerateFilesProcessingErrorMessageWithStudentEmails(studentWithoutProctoringEmails));
                }
            }
        }

        studentsData.Sort((firstElement, secondElement)
            => firstElement.Item1.SecondName.CompareTo(secondElement.Item1.SecondName));
        this.studentsDataWithExplicitProctoringStatus = studentsData;
        this.isNotNullDataActual = true;
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