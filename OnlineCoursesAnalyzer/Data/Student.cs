namespace OnlineCoursesAnalyzer.Data;

public class Student
{
    public Student(string secondName, string firstName, string lastName, string grade)
    {
        this.SecondName = secondName;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Grade = grade;
    }

    public string SecondName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Grade { get; set; }

    public string? ProctoringStatus { get; set; }
}