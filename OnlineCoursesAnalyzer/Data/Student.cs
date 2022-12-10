namespace OnlineCoursesAnalyzer.Data;

public class Student
{
    public Student(string email, string secondName, string firstName, string lastName, string grade, string proctoringStatus)
    {
        this.SecondName = secondName;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Grade = grade;
        this.ProctoringStatus = proctoringStatus;
        this.Email = email;
    }

    public Student(string email, string secondName, string firstName, string lastName, string grade)
        : this(email, secondName, firstName, lastName, grade, string.Empty)
    {
    }

    public string Email { get; }

    public string SecondName { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Grade { get; }

    public string ProctoringStatus { get; set; }
}