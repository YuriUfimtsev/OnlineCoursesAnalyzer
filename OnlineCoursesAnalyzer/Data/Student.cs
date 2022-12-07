﻿namespace OnlineCoursesAnalyzer.Data;

public class Student
{
    public Student(string secondName, string firstName, string lastName, string grade, string proctoringStatus)
    {
        this.SecondName = secondName;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Grade = grade;
        this.ProctoringStatus = proctoringStatus;
    }

    public Student(string secondName, string firstName, string lastName, string grade)
        : this(secondName, firstName, lastName, grade, string.Empty)
    {
    }

    public string SecondName { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Grade { get; }

    public string ProctoringStatus { get; set; }
}