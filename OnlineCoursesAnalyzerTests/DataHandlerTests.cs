using OnlineCoursesAnalyzer.Data;
using OnlineCoursesAnalyzer.DataHandling;
using System.Collections.Specialized;

namespace OnlineCoursesAnalyzerTests;

public class DataHandlerTests
{
    private static string GetPathToFile(string fileName)
        => $"../../../Data/{fileName}";

    private static bool CompareStudents(Student firstStudent, Student secondStudent)
        => firstStudent.SecondName == secondStudent.SecondName
            && firstStudent.FirstName == secondStudent.FirstName
            && firstStudent.LastName == secondStudent.LastName
            && firstStudent.Grade == secondStudent.Grade
            && firstStudent.ProctoringStatus == secondStudent.ProctoringStatus;

    private static bool CompareStudentDataDictionaries(Dictionary<string, Student>? firstDictionary,
        Dictionary<string, Student>? secondDictionary)
    {
        if (firstDictionary == null || secondDictionary == null)
        {
            return firstDictionary == secondDictionary;
        }

        if (firstDictionary.Count != secondDictionary.Count)
        {
            return false;
        }

        foreach (var keyValuePair in firstDictionary)
        {
            if (!secondDictionary.ContainsKey(keyValuePair.Key) || !CompareStudents(secondDictionary[keyValuePair.Key], keyValuePair.Value))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CompareStudentDataLists(List<(Student, bool)> firstData, List<(Student, bool)> secondData)
    {
        if (firstData.Count != secondData.Count)
        {
            return false;
        }

        foreach (var firstItem in firstData)
        {
            var isItemFound = false;
            foreach (var secondItem in secondData)
            {
                if (CompareStudents(firstItem.Item1, secondItem.Item1) && firstItem.Item2 == secondItem.Item2)
                {
                    isItemFound = true;
                }
            }

            if (!isItemFound)
            {
                return false;
            }
        }

        return true;
    }

    [Test]
    public void GetGradeTest()
    {
        var gradePercentArray = new[] { "90.0", "50", "87.9", "61", "70.3" };
        var gradeArray = new string[5];
        for (var i = 0; i < gradePercentArray.Length; ++i)
        {
            gradeArray[i] = Grade.GetGrade(gradePercentArray[i]).ToString();
        }

        var expectedGradeArray = new[] { "A", "E", "B", "D", "C" };
        Assert.That(gradeArray, Is.EqualTo(expectedGradeArray));
    }

    [Test]
    public void AddEducationalAchievmentDataFileStandartTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentStandartData.xlsx"));
        var expectedData = new Dictionary<string, Student>
        {
            { "st000000@student.spbu.ru", new Student("Анонимов", "Аноним", "Анонимович", "F") },
            { "st000001@student.spbu.ru", new Student("Иванов", "Иван", "Иванович", "A") },
            { "st000002@student.spbu.ru", new Student("Петров", "Петр", "Петрович", "A") },
            { "st000003@student.spbu.ru", new Student("Александров", "Александр", "Александрович", "E") },
            { "st000004@student.spbu.ru", new Student("Сергеев", "Сергей", "Сергеевич", "C") },
        };
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        Assert.That(errorRows.Count, Is.EqualTo(0));
        Assert.That(CompareStudentDataDictionaries(expectedData, dataHandler.EducationalAchievementData));
    }

    [Test]
    public void AddEducationalAchievmentDataWithNullRowsAndIncorrectGradePercentTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithSomeErrors.xlsx"));
        if (EducationalAchievementFile.AllowedNumberOfErrorRows < 5)
        {
            Assert.Throws<InvalidInputDataException>(() => dataHandler.AddEducationalAchievementData(fileStream));
            return;
        }

        var expectedErrorRowNumbers = new List<string> { "2", "4", "7", "9", "10" };
        var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
        CollectionAssert.AreEquivalent(expectedErrorRowNumbers, errorRows);
        Assert.That(dataHandler.EducationalAchievementData!.Count, Is.EqualTo(5));
    }

    [Test]
    public void AddEducationalAchievmentDataWithTwoEqualsStudentIdentifiersTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithTwoEqualsStudentEmails.xlsx"));
        Assert.Throws<InvalidInputDataException>(() => dataHandler.AddEducationalAchievementData(fileStream));
    }

    [Test]
    public void AddEducationalAchievmentDataWithManyErrorRowsTest()
    {
        var dataHandler = new DataHandler();
        var fileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithManyErrorRows.xlsx"));
        if (EducationalAchievementFile.AllowedNumberOfErrorRows >= 100)
        {
            var errorRows = dataHandler.AddEducationalAchievementData(fileStream);
            Assert.That(errorRows.Count, Is.EqualTo(100));
            return;
        }

        Assert.Throws<InvalidInputDataException>(() => dataHandler.AddEducationalAchievementData(fileStream));
        Assert.That(dataHandler.EducationalAchievementData, Is.EqualTo(null));
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
        if (ProctoringStatusFile.AllowedNumberOfErrorRows < 6)
        {
            Assert.Throws<InvalidInputDataException>(() => dataHandler.AddProctoringStatusData(fileStream));
            return;
        }

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
        if (ProctoringStatusFile.AllowedNumberOfErrorRows >= 100)
        {
            var errorRows = dataHandler.AddProctoringStatusData(fileStream);
            Assert.That(errorRows.Count, Is.EqualTo(100));
            return;
        }

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
            ( new Student("Анонимов", "Аноним", "Анонимович", "F", "yes"), true ),
            ( new Student("Иванов", "Иван", "Иванович", "A", "no"), false ),
            ( new Student("Петров", "Петр", "Петрович", "A", "no"), false ),
            ( new Student("Александров", "Александр", "Александрович", "E", "no"), false ),
            ( new Student("Сергеев", "Сергей", "Сергеевич", "C", "yes"), true ),
        };
        var (studentsData, errorStudents) = dataHandler.GetResultWithExplicitProctoringStatus();
        Assert.That(errorStudents.Count, Is.EqualTo(0));
        Assert.That(CompareStudentDataLists(expectedResult, studentsData));
    }

    [Test]
    public void GetResultWithSomeErrorsTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentDataWithSomeErrors.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusDataWithSomeErrors.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        if (EducationalAchievementFile.AllowedNumberOfStudentsWithoutProctoringStatus
            < dataHandler.EducationalAchievementData!.Count - dataHandler.ProctoringStatusData!.Count)
        {
            Assert.Throws<InvalidInputDataException>(() => dataHandler.GetResultWithExplicitProctoringStatus());
            return;
        }

        var (studentsData, errorStudents) = dataHandler.GetResultWithExplicitProctoringStatus();
        Assert.That(studentsData.Count, Is.EqualTo(2));
        Assert.That(errorStudents.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetResultWithManyErrorsTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentBigData.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusRow.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        if (EducationalAchievementFile.AllowedNumberOfStudentsWithoutProctoringStatus
            >= dataHandler.EducationalAchievementData!.Count - dataHandler.ProctoringStatusData!.Count)
        {
            var (studentsData, errorStudents) = dataHandler.GetResultWithExplicitProctoringStatus();
            Assert.That(studentsData.Count, Is.EqualTo(1));
            Assert.That(errorStudents.Count, Is.EqualTo(100));
            return;
        }

        Assert.Throws<InvalidInputDataException>(() => dataHandler.GetResultWithExplicitProctoringStatus());
    }

    [Test]
    public void IsNotNullProctoringDataMoreThanNotNullEducationalAchievmentDataTest()
    {
        var dataHandler = new DataHandler();
        var educationalAchievementFileStream = File.OpenRead(GetPathToFile("EducationalAchievmentStandartData.xlsx"));
        var proctoringStatusFileStream = File.OpenRead(GetPathToFile("ProctoringStatusBigData.xlsx"));
        dataHandler.AddEducationalAchievementData(educationalAchievementFileStream);
        dataHandler.AddProctoringStatusData(proctoringStatusFileStream);
        Assert.That(dataHandler.IsNotNullProctoringDataMoreThanNotNullEducationalAchievmentData);
    }
}
