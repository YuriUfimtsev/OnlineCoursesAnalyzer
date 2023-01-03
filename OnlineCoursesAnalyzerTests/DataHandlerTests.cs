using OnlineCoursesAnalyzer.Data;
using OnlineCoursesAnalyzer.DataHandling;

namespace OnlineCoursesAnalyzerTests;

public class DataHandlerTests
{
    private static string GetPathToFile(string fileName)
        => $"Data/DataHandlerTestsData/{fileName}";

    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
    }

    [Test]
    public void GetGradeTest()
    {
        var gradePercentArray = new[] { "0.9", "0.50", "0.879", "0.61", "0.703" };
        var gradeArray = new string[5];
        for (var i = 0; i < gradePercentArray.Length; ++i)
        {
            gradeArray[i] = Grade.GetGrade(gradePercentArray[i]).ToString();
        }

        var expectedGradeArray = new[] { "A", "E", "B", "D", "C" };
        Assert.That(gradeArray, Is.EqualTo(expectedGradeArray));
    }

    [Test]
    public void AddEducationalAchievementDataStandartTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentStandartData.xlsx"));
        var expectedData = new Dictionary<string, Student>
        {
            { "st000000@student.spbu.ru", new Student("st000000@student.spbu.ru", "Анонимов", "Аноним", "Анонимович", "F") },
            { "st000002@student.spbu.ru", new Student("st000002@student.spbu.ru", "Петров", "Петр", "Петрович", "A") },
            { "st000004@student.spbu.ru", new Student("st000004@student.spbu.ru", "Сергеев", "Сергей", "Сергеевич", "C") },
        };
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        Assert.That(errorRows.Count, Is.EqualTo(0));
        CollectionAssert.AreEquivalent(expectedData, dataHandler.EducationalAchievementData);
    }

    [Test]
    public void AddEducationalAchievementDataWithZerosInControlTaskColumnTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievementDataWithZerosInControlTaskColumn.xlsx"));
        var expectedData = new Dictionary<string, Student>
        {
            { "st000000@student.spbu.ru", new Student("st000000@student.spbu.ru", "Анонимов", "Аноним", "Анонимович", "E") },
            { "st000001@student.spbu.ru", new Student("st000001@student.spbu.ru", "Иванов", "Иван", "Иванович", "A") },
            { "st000002@student.spbu.ru", new Student("st000002@student.spbu.ru", "Петров", "Петр", "Петрович", "F") },
            { "st000003@student.spbu.ru", new Student("st000003@student.spbu.ru", "Александров", "Александр", "Александрович", "F") },
            { "st000004@student.spbu.ru", new Student("st000004@student.spbu.ru", "Сергеев", "Сергей", "Сергеевич", "F") },
        };
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        Assert.That(errorRows.Count, Is.EqualTo(0));
        CollectionAssert.AreEquivalent(expectedData, dataHandler.EducationalAchievementData);
    }

    [Test]
    public void AddEducationalAchievementDataCaseInsensitiveToFacultyNameTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievementDataCaseInsensitiveToFacultyName.xlsx"));
        var expectedData = new Dictionary<string, Student>
        {
            { "st000000@student.spbu.ru", new Student("st000000@student.spbu.ru", "Анонимов", "Аноним", "Анонимович", "E") },
            { "st000001@student.spbu.ru", new Student("st000001@student.spbu.ru", "Иванов", "Иван", "Иванович", "A") },
            { "st000004@student.spbu.ru", new Student("st000004@student.spbu.ru", "Сергеев", "Сергей", "Сергеевич", "F") },
        };
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        Assert.That(errorRows.Count, Is.EqualTo(0));
        CollectionAssert.AreEquivalent(expectedData, dataHandler.EducationalAchievementData);
    }

    [Test]
    public void AddEducationalAchievementDataWithNullRowsAndIncorrectGradePercentMathmechStudentsTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithSomeErrors.xlsx"));
        var expectedErrorRowNumbers = new List<string> { "4", "7", "9" };
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        CollectionAssert.AreEquivalent(expectedErrorRowNumbers, errorRows);
        Assert.That(dataHandler.EducationalAchievementData!.Count, Is.EqualTo(7));
    }

    [Test]
    public void AddEducationalAchievementDataWithTwoEqualsMathmechStudentIdentifiersTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithTwoEqualsStudentEmails.xlsx"));
        Assert.Throws<InvalidInputDataException>(() => dataHandler.AddEducationalAchievementData(fileStream));
    }

    [Test]
    public void AddEducationalAchievementDataWithManyMathmechStudentsErrorRowsTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithManyErrorRows.xlsx"));
        Assert.Throws<InvalidInputDataException>(() => dataHandler.AddEducationalAchievementData(fileStream));
        Assert.That(dataHandler.EducationalAchievementData, Is.EqualTo(null));
    }

    [Test]
    public void AddEducationalAchievementDataWithManyNotMathmechStudentsErrorRowsTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithManyNotMathmechStudentsErrorRows.xlsx"));
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        Assert.That(errorRows.Count, Is.EqualTo(0));
        Assert.That(dataHandler.EducationalAchievementData!.Count, Is.EqualTo(31));
    }

    [Test]
    public void AddProctoringStatusDataFileStandartTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("ProctoringStatusStandartData.xlsx"));
        var expectedData = new Dictionary<string, bool>
        {
            { "st000000@student.spbu.ru", true },
            { "st000001@student.spbu.ru", false },
            { "st000002@student.spbu.ru", false },
            { "st000003@student.spbu.ru", false },
            { "st000004@student.spbu.ru", true },
        };
        var errorRows = dataHandler.AddProctoringStatusData(fileStream);
        Assert.That(errorRows.Count, Is.EqualTo(0));
        CollectionAssert.AreEquivalent(expectedData, dataHandler.ProctoringStatusData);
    }

    [Test]
    public void AddProctoringStatusDataWithNullRowsAndIncorrectProctoringStatusTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("ProctoringStatusDataWithSomeErrors.xlsx"));
        var expectedErrorRowNumbers = new List<string> { "4", "5", "6", "8", "9", "10" };
        var errorRows = dataHandler.AddProctoringStatusData(fileStream);
        CollectionAssert.AreEquivalent(expectedErrorRowNumbers, errorRows);
        Assert.That(dataHandler.ProctoringStatusData!.Count, Is.EqualTo(4));
    }

    [Test]
    public void AddProctoringStatusDataWithTwoEqualsStudentIdentifiersTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("ProctoringStatusDataWithTwoEqualsStudentEmails.xlsx"));
        Assert.Throws<InvalidInputDataException>(() => dataHandler.AddProctoringStatusData(fileStream));
    }

    [Test]
    public void AddProctoringStatusDataWithManyErrorRowsTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("ProctoringStatusDataWithManyErrorRows.xlsx"));
        Assert.Throws<InvalidInputDataException>(() => dataHandler.AddProctoringStatusData(fileStream));
        Assert.That(dataHandler.ProctoringStatusData, Is.EqualTo(null));
    }

    [Test]
    public void GetResultStandartTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentStandartData.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusStandartData.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        var expectedResult = new List<(Student, bool)>
        {
            ( new Student("st000000@student.spbu.ru", "Анонимов", "Аноним", "Анонимович", "F", "yes"), true),
            ( new Student("st000002@student.spbu.ru", "Петров", "Петр", "Петрович", "A", "no"), false),
            ( new Student("st000004@student.spbu.ru", "Сергеев", "Сергей", "Сергеевич", "C", "yes"), true),
        };
        var (studentsData, errorStudents) = dataHandler.GetResultWithExplicitProctoringStatus();
        Assert.That(errorStudents.Count, Is.EqualTo(0));
        CollectionAssert.AreEquivalent(expectedResult, studentsData);
        //Assert.That(CompareStudentDataLists(expectedResult, studentsData));
    }

    [Test]
    public void GetResultWithSomeErrorsTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithSomeErrors.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusDataWithSomeErrors.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        var (studentsData, errorStudents) = dataHandler.GetResultWithExplicitProctoringStatus();
        Assert.That(studentsData.Count, Is.EqualTo(3));
        Assert.That(errorStudents.Count, Is.EqualTo(4));
    }

    [Test]
    public void GetResultWithManyErrorsTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentBigData.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusRow.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        Assert.Throws<InvalidInputDataException>(() => dataHandler.GetResultWithExplicitProctoringStatus());
    }

    [Test]
    public void GetResultLoadTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentBigData.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusBigData.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        var (result, errorStudents) = dataHandler.GetResultWithExplicitProctoringStatus();
        Assert.That(result.Count, Is.EqualTo(155));
        Assert.That(errorStudents.Count, Is.EqualTo(0));
    }
}
