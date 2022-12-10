using OnlineCoursesAnalyzer.Data;

namespace OnlineCoursesAnalyzer.DataHandling;

/// <summary>
/// Implements the aducational achievement data and proctoring status data handling.
/// </summary>
public class DataHandler
{
    /// <summary>
    /// Array of educational achievement file column names from which data should be obtained.
    /// </summary>
    private static readonly string[] RequiredDataFromEducationalAchievementFile =
    {
        EducationalAchievementFile.EmailColumn,
        EducationalAchievementFile.SecondNameColumn,
        EducationalAchievementFile.FirstNameColumn,
        EducationalAchievementFile.LastNameColumn,
        EducationalAchievementFile.GradePercentColumn,
    };

    /// <summary>
    /// Columns in which a missing value will be treated as an error.
    /// </summary>
    private static readonly string[] EducationalAchievementFileSignificantColumns =
    {
        EducationalAchievementFile.EmailColumn,
        EducationalAchievementFile.GradePercentColumn,
    };

    /// <summary>
    /// Educational achievement file column name by which the data will be selected.
    /// </summary>
    private static readonly string ConditionEducationalAchievementFileColumn = EducationalAchievementFile.FacultyNamesColumn;

    /// <summary>
    /// Condition for data selection.
    /// </summary>
    private static readonly string EducationalAchievementFileCondition = EducationalAchievementFile.MathMechFacultyName;

    /// <summary>
    /// Array of proctoring status file column names from which data should be obtained.
    /// </summary>
    private static readonly string[] RequiredDataFromProctoringStatusFile =
    {
        ProctoringStatusFile.EmailColumn,
        ProctoringStatusFile.ProctoringStatusColumn,
    };

    private List<(Student, bool)>? studentsDataWithExplicitProctoringStatus;
    private bool isNotNullDataActual;

    /// <summary>
    /// Gets educational achievement data where keys - student email addresses, values - students data.
    /// </summary>
    public Dictionary<string, Student>? EducationalAchievementData { get; private set; }

    /// <summary>
    /// Gets proctoring status data where keys - student email addresses, values - student proctoring statuses.
    /// </summary>
    public Dictionary<string, bool>? ProctoringStatusData { get; private set; }

    /// <summary>
    /// Adds educational achievement data to the storage.
    /// </summary>
    /// <param name="fileReadStream">Stream with the educational achievement data.</param>
    /// <returns>List of row numbers containing errors.</returns>
    /// <exception cref="InvalidInputDataException">Throws if the input data was received in an incorrect format.
    /// View EducationalAchievementFile class as a sample.</exception>
    public List<string> AddEducationalAchievementData(Stream fileReadStream)
    {
        var (educationalAchievmentDataList, errorRows) = XLSXParser.GetDataByTheColumnContainsConditionWithoutFirstRow(
            fileReadStream,
            RequiredDataFromEducationalAchievementFile,
            EducationalAchievementFileSignificantColumns,
            ConditionEducationalAchievementFileColumn,
            EducationalAchievementFileCondition,
            EducationalAchievementFile.AllowedNumberOfErrorRows);
        var educationalAchievmentDataDictionary = new Dictionary<string, Student>();
        foreach (var rowData in educationalAchievmentDataList)
        {
            try
            {
                var grade = Grade.GetGrade(rowData[4]);
                var student = new Student(rowData[0], rowData[1], rowData[2], rowData[3], grade.ToString());
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

    /// <summary>
    /// Adds proctoring status data to the storage.
    /// </summary>
    /// <param name="fileReadStream">Stream with the proctoring status data.</param>
    /// <returns>List of row numbers containing errors.</returns>
    /// <exception cref="InvalidInputDataException">Throws if the input data was received in an incorrect format.
    /// View ProctoringStatusFile class as a sample.</exception>
    public List<string> AddProctoringStatusData(Stream fileReadStream)
    {
        var (proctoringStatusDataList, errorRows) = XLSXParser.GetDataWithoutFirstRow(
            fileReadStream,
            RequiredDataFromProctoringStatusFile,
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

    /// <summary>
    /// Performs processing of the saved educational achievement and proctoring status data.
    /// </summary>
    /// <returns>List of students with explicit proctoring status, list with email addresses of students without proctoring status.</returns>
    /// <exception cref="InvalidInputDataException">Throws if the input data was received in an incorrect format.
    /// View ProctoringStatusFile class and EducationalAchievementFile class as a sample.</exception>
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

        try
        {
            studentsData.Sort((firstElement, secondElement)
                    => EmailComparerWithExpectedFormat(firstElement.Item1.Email, secondElement.Item1.Email));
        }
        catch (InvalidOperationException)
        {
            {
                throw new InvalidInputDataException(
                    Messages.GenerateComplexMessage(Messages.UnexpectedEmailFormat, Messages.GetMoreInformation),
                    Messages.AdvancedUnexpectedEmail);
            }
        }

        this.studentsDataWithExplicitProctoringStatus = studentsData;
        this.isNotNullDataActual = true;
        return (studentsData, studentWithoutProctoringEmails);
    }

    private static int EmailComparerWithExpectedFormat(string firstEmail, string secondEmail)
    {
        var firstEmailNumber = int.Parse(firstEmail.Substring(2, 6));
        var secondEmailNumber = int.Parse(secondEmail.Substring(2, 6));
        return firstEmailNumber.CompareTo(secondEmailNumber);
    }

    private static (bool IsInterpretationCorrect, bool InterpretationResult) InterpretProctoringStatus(
        string proctoringData)
    {
        var interpretationResult = proctoringData == ProctoringStatusFile.ProctoringStatusIsTrue;
        return (
            interpretationResult || (!interpretationResult && proctoringData == ProctoringStatusFile.ProctoringStatusIsFalse),
            interpretationResult);
    }
}