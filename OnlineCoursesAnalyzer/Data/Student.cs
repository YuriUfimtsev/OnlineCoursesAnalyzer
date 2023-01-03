namespace OnlineCoursesAnalyzer.Data;

/// <summary>
/// Implements the student representation.
/// </summary>
public record Student
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Student"/> class.
    /// </summary>
    /// <param name="email">Student email address.</param>
    /// <param name="lastName">Student surname.</param>
    /// <param name="firstName">Student name.</param>
    /// <param name="secondName">Student patronymic.</param>
    /// <param name="grade">Student grade for course.</param>
    /// <param name="proctoringStatus">Student proctoring status.</param>
    public Student(string email, string lastName, string firstName, string secondName, string grade, string proctoringStatus)
    {
        this.SecondName = secondName;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Grade = grade;
        this.ProctoringStatus = proctoringStatus;
        this.Email = email;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Student"/> class.
    /// </summary>
    /// <param name="email">Student email address.</param>
    /// <param name="lastName">Student surname.</param>
    /// <param name="firstName">Student name.</param>
    /// <param name="secondName">Student patronymic.</param>
    /// <param name="grade">Student grade for course.</param>
    public Student(string email, string lastName, string firstName, string secondName, string grade)
        : this(email, lastName, firstName, secondName, grade, string.Empty)
    {
    }

    /// <summary>
    /// Gets the student email address.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Gets the student surname.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// Gets the student name.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// Gets the student patronymic.
    /// </summary>
    public string SecondName { get; init; }

    /// <summary>
    /// Gets the student grade for course.
    /// </summary>
    public string Grade { get; init; }

    /// <summary>
    /// Gets or sets the student proctoring status.
    /// </summary>
    public string ProctoringStatus { get; set; }
}