using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ==============================================
// WŁASNE KLASY WYJĄTKÓW
// ==============================================

/// <summary>
/// Wyjątek rzucany, gdy kurs nie zostanie znaleziony.
/// </summary>
public class CourseNotFoundException : Exception
{
    public CourseNotFoundException(string message) : base(message) { }
}

/// <summary>
/// Wyjątek rzucany, gdy kurs osiągnął limit miejsc.
/// </summary>
public class CourseFullException : Exception
{
    public CourseFullException(string message) : base(message) { }
}

/// <summary>
/// Wyjątek rzucany, gdy student jest już zapisany na kurs.
/// </summary>
public class StudentAlreadyEnrolledException : Exception
{
    public StudentAlreadyEnrolledException(string message) : base(message) { }
}

/// <summary>
/// Wyjątek rzucany, gdy student nie jest zapisany na kurs.
/// </summary>
public class StudentNotEnrolledException : Exception
{
    public StudentNotEnrolledException(string message) : base(message) { }
}

/// <summary>
/// Wyjątek rzucany przy próbie usunięcia kursu z zapisanymi studentami.
/// </summary>
public class CannotRemoveCourseWithEnrolledStudentsException : Exception
{
    public CannotRemoveCourseWithEnrolledStudentsException(string message) : base(message) { }
}

/// <summary>
/// Wyjątek rzucany przy próbie usunięcia studenta zapisanego na kursy.
/// </summary>
public class CannotRemoveStudentWithEnrollmentsException : Exception
{
    public CannotRemoveStudentWithEnrollmentsException(string message) : base(message) { }
}

// ==============================================
// KLASA STUDENT
// ==============================================

/// <summary>
/// Reprezentuje studenta z możliwością sortowania po nazwisku i imieniu.
/// </summary>
public class Student : IComparable<Student>
{
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime BirthDate { get; }
    public string StudentId { get; }
    private List<Course> EnrolledCourses = new List<Course>();

    public Student(string firstName, string lastName, DateTime birthDate, string studentId)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        StudentId = studentId;
    }

    /// <summary>
    /// Zapisuje studenta na kurs.
    /// </summary>
    public void EnrollInCourse(Course course)
    {
        if (EnrolledCourses.Contains(course))
            throw new StudentAlreadyEnrolledException($"Student {StudentId} jest już zapisany na kurs {course.CourseName}.");
        
        EnrolledCourses.Add(course);
        course.AddStudent(this);
    }

    /// <summary>
    /// Wypisuje studenta z kursu.
    /// </summary>
    public void WithdrawFromCourse(Course course)
    {
        if (!EnrolledCourses.Contains(course))
            throw new StudentNotEnrolledException($"Student {StudentId} nie jest zapisany na kurs {course.CourseName}.");
        
        EnrolledCourses.Remove(course);
        course.RemoveStudent(this);
    }

    /// <summary>
    /// Wyświetla listę kursów, na które zapisany jest student.
    /// </summary>
    public void ListCourses()
    {
        Console.WriteLine($"Kursy studenta {LastName} {FirstName}:");
        foreach (var course in EnrolledCourses)
        {
            Console.WriteLine($"- {course.CourseName}");
        }
    }

    /// <summary>
    /// Porównuje studentów po nazwisku i imieniu.
    /// </summary>
    public int CompareTo(Student other)
    {
        int lastNameComparison = LastName.CompareTo(other.LastName);
        return lastNameComparison != 0 ? lastNameComparison : FirstName.CompareTo(other.FirstName);
    }

    /// <summary>
    /// Sprawdza, czy student jest zapisany na jakiekolwiek kursy.
    /// </summary>
    public bool IsEnrolledInAnyCourse() => EnrolledCourses.Any();
}

// ==============================================
// KLASA KURS
// ==============================================

/// <summary>
/// Reprezentuje kurs z możliwością sortowania po dacie rozpoczęcia.
/// </summary>
public class Course : IComparable<Course>
{
    public string CourseName { get; }
    public int Capacity { get; }
    public DateTime StartDate { get; }
    private List<Student> EnrolledStudents = new List<Student>();

    public Course(string courseName, int capacity, DateTime startDate)
    {
        CourseName = courseName;
        Capacity = capacity;
        StartDate = startDate;
    }

    /// <summary>
    /// Dodaje studenta do kursu.
    /// </summary>
    public void AddStudent(Student student)
    {
        if (EnrolledStudents.Count >= Capacity)
            throw new CourseFullException($"Kurs {CourseName} osiągnął limit miejsc ({Capacity}).");
        
        if (EnrolledStudents.Contains(student))
            throw new StudentAlreadyEnrolledException($"Student {student.StudentId} jest już zapisany na kurs {CourseName}.");
        
        EnrolledStudents.Add(student);
    }

    /// <summary>
    /// Usuwa studenta z kursu.
    /// </summary>
    public void RemoveStudent(Student student)
    {
        if (!EnrolledStudents.Contains(student))
            throw new StudentNotEnrolledException($"Student {student.StudentId} nie jest zapisany na kurs {CourseName}.");
        
        EnrolledStudents.Remove(student);
    }

    /// <summary>
    /// Wyświetla listę studentów zapisanych na kurs.
    /// </summary>
    public void DisplayStudents()
    {
        Console.WriteLine($"Studenci kursu {CourseName}:");
        foreach (var student in EnrolledStudents.OrderBy(s => s))
        {
            Console.WriteLine($"- {student.LastName} {student.FirstName}");
        }
    }

    /// <summary>
    /// Sprawdza dostępność miejsc na kursie.
    /// </summary>
    public bool HasAvailableSlots() => EnrolledStudents.Count < Capacity;

    /// <summary>
    /// Porównuje kursy po dacie rozpoczęcia.
    /// </summary>
    public int CompareTo(Course other) => StartDate.CompareTo(other.StartDate);
}

// ==============================================
// SYSTEM REJESTRACJI
// ==============================================

/// <summary>
/// Główny system zarządzający rejestracją z logowaniem operacji i obsługą plików.
/// </summary>
public class RegistrationSystem : IDisposable
{
    private List<Student> students = new List<Student>();
    private List<Course> courses = new List<Course>();
    private StreamWriter logWriter;

    public RegistrationSystem()
    {
        // Inicjalizacja logowania
        logWriter = new StreamWriter("log.txt", append: true);
        Log("System uruchomiony");
    }

    /// <summary>
    /// Zapisuje wiadomość do pliku logów z znacznikiem czasu.
    /// </summary>
    private void Log(string message, bool isError = false)
    {
        string logEntry = $"[{DateTime.Now}] {(isError ? "BŁĄD" : "INFO")}: {message}";
        logWriter.WriteLine(logEntry);
        logWriter.Flush(); // Natychmiastowy zapis
    }

    /// <summary>
    /// Dodaje nowego studenta do systemu.
    /// </summary>
    public void AddStudent(Student student)
    {
        if (students.Any(s => s.StudentId == student.StudentId))
        {
            Log($"Próba dodania studenta o istniejącym ID: {student.StudentId}", true);
            throw new ArgumentException($"Student o ID {student.StudentId} już istnieje.");
        }
        
        students.Add(student);
        Log($"Dodano studenta: {student.LastName} {student.FirstName} ({student.StudentId})");
    }

    /// <summary>
    /// Dodaje nowy kurs do systemu.
    /// </summary>
    public void AddCourse(Course course)
    {
        if (courses.Any(c => c.CourseName == course.CourseName))
        {
            Log($"Próba dodania kursu o istniejącej nazwie: {course.CourseName}", true);
            throw new ArgumentException($"Kurs o nazwie {course.CourseName} już istnieje.");
        }
        
        courses.Add(course);
        Log($"Dodano kurs: {course.CourseName} (Start: {course.StartDate:yyyy-MM-dd})");
    }

    /// <summary>
    /// Zapisuje studenta na kurs.
    /// </summary>
    public void EnrollStudent(string studentId, string courseName)
    {
        var student = students.FirstOrDefault(s => s.StudentId == studentId);
        var course = courses.FirstOrDefault(c => c.CourseName == courseName);

        if (student == null) throw new ArgumentException($"Nie znaleziono studenta o ID: {studentId}");
        if (course == null) throw new CourseNotFoundException($"Nie znaleziono kursu: {courseName}");

        try
        {
            student.EnrollInCourse(course);
            Log($"Zapisano studenta {studentId} na kurs {courseName}");
        }
        catch (Exception ex)
        {
            Log($"Błąd zapisu: {ex.Message}", true);
            throw;
        }
    }

    /// <summary>
    /// Wypisuje studenta z kursu.
    /// </summary>
    public void WithdrawStudent(string studentId, string courseName)
    {
        var student = students.FirstOrDefault(s => s.StudentId == studentId);
        var course = courses.FirstOrDefault(c => c.CourseName == courseName);

        if (student == null) throw new ArgumentException($"Nie znaleziono studenta o ID: {studentId}");
        if (course == null) throw new CourseNotFoundException($"Nie znaleziono kursu: {courseName}");

        try
        {
            student.WithdrawFromCourse(course);
            Log($"Wypisano studenta {studentId} z kursu {courseName}");
        }
        catch (Exception ex)
        {
            Log($"Błąd wypisywania: {ex.Message}", true);
            throw;
        }
    }

    /// <summary>
    /// Wyświetla listę studentów posortowaną alfabetycznie.
    /// </summary>
    public void ListStudents()
    {
        Console.WriteLine("Lista studentów (A-Z):");
        foreach (var student in students.OrderBy(s => s))
        {
            Console.WriteLine($"- {student.LastName} {student.FirstName} ({student.StudentId})");
        }
    }

    /// <summary>
    /// Wyświetla listę kursów posortowaną po dacie rozpoczęcia.
    /// </summary>
    public void ListCourses()
    {
        Console.WriteLine("Lista kursów (wg daty):");
        foreach (var course in courses.OrderBy(c => c))
        {
            Console.WriteLine($"- {course.CourseName} (Start: {course.StartDate:yyyy-MM-dd}, Miejsca: {course.Capacity})");
        }
    }

    /// <summary>
    /// Wyświetla kursy z dostępnymi miejscami.
    /// </summary>
    public void ListAvailableCourses()
    {
        Console.WriteLine("Dostępne kursy (z wolnymi miejscami):");
        foreach (var course in courses.Where(c => c.HasAvailableSlots()).OrderBy(c => c))
        {
            Console.WriteLine($"- {course.CourseName} (Wolne miejsca: {course.Capacity - course.EnrolledStudents.Count})");
        }
    }

    /// <summary>
    /// Usuwa studenta z systemu.
    /// </summary>
    public void RemoveStudent(string studentId)
    {
        var student = students.FirstOrDefault(s => s.StudentId == studentId);
        if (student == null) throw new ArgumentException($"Nie znaleziono studenta o ID: {studentId}");

        if (student.IsEnrolledInAnyCourse())
            throw new CannotRemoveStudentWithEnrollmentsException($"Nie można usunąć studenta {studentId} zapisanego na kursy.");

        students.Remove(student);
        Log($"Usunięto studenta: {student.LastName} {student.FirstName} ({studentId})");
    }

    /// <summary>
    /// Usuwa kurs z systemu.
    /// </summary>
    public void RemoveCourse(string courseName)
    {
        var course = courses.FirstOrDefault(c => c.CourseName == courseName);
        if (course == null) throw new CourseNotFoundException($"Nie znaleziono kursu: {courseName}");

        if (course.EnrolledStudents.Any())
            throw new CannotRemoveCourseWithEnrolledStudentsException($"Nie można usunąć kursu {courseName} z zapisanymi studentami.");

        courses.Remove(course);
        Log($"Usunięto kurs: {courseName}");
    }

    /// <summary>
    /// Zapisuje dane do plików i zwalnia zasoby.
    /// </summary>
    public void Dispose()
    {
        try
        {
            // Zapis danych do plików (do zaimplementowania)
            Log("Zamykanie systemu");
            logWriter?.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas zamykania systemu: {ex.Message}");
        }
    }
}

// ==============================================
// PROGRAM GŁÓWNY
// ==============================================

class Program
{
    static void Main()
    {
        using (var system = new RegistrationSystem())
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine(@"
===============================
     SYSTEM REJESTRACJI
===============================
1. Dodaj nowego studenta
2. Dodaj nowy kurs
3. Zapisz studenta na kurs
4. Wypisz studenta z kursu
5. Wyświetl listę studentów (A-Z)
6. Wyświetl listę kursów (wg daty)
7. Wyświetl dostępne kursy (z wolnymi miejscami)
8. Usuń studenta
9. Usuń kurs
10. Zapisz i zakończ program
-------------------------------
");
                Console.Write("Wybierz opcję: ");
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1": AddNewStudent(system); break;
                        case "2": AddNewCourse(system); break;
                        case "3": EnrollStudent(system); break;
                        case "4": WithdrawStudent(system); break;
                        case "5": system.ListStudents(); break;
                        case "6": system.ListCourses(); break;
                        case "7": system.ListAvailableCourses(); break;
                        case "8": RemoveStudent(system); break;
                        case "9": RemoveCourse(system); break;
                        case "10": running = false; break;
                        default: Console.WriteLine("Nieprawidłowa opcja!"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BŁĄD: {ex.Message}");
                }
            }
        }
        Console.WriteLine("Program zakończony. Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    static void AddNewStudent(RegistrationSystem system)
    {
        Console.Write("Imię: ");
        var firstName = Console.ReadLine();
        Console.Write("Nazwisko: ");
        var lastName = Console.ReadLine();
        Console.Write("Data urodzenia (yyyy-mm-dd): ");
        var birthDate = DateTime.Parse(Console.ReadLine());
        Console.Write("ID studenta: ");
        var id = Console.ReadLine();

        system.AddStudent(new Student(firstName, lastName, birthDate, id));
        Console.WriteLine("Student dodany pomyślnie!");
    }

    static void AddNewCourse(RegistrationSystem system)
    {
        Console.Write("Nazwa kursu: ");
        var name = Console.ReadLine();
        Console.Write("Limit miejsc: ");
        var capacity = int.Parse(Console.ReadLine());
        Console.Write("Data rozpoczęcia (yyyy-mm-dd): ");
        var startDate = DateTime.Parse(Console.ReadLine());

        system.AddCourse(new Course(name, capacity, startDate));
        Console.WriteLine("Kurs dodany pomyślnie!");
    }

    static void EnrollStudent(RegistrationSystem system)
    {
        Console.Write("ID studenta: ");
        var studentId = Console.ReadLine();
        Console.Write("Nazwa kursu: ");
        var courseName = Console.ReadLine();

        system.EnrollStudent(studentId, courseName);
        Console.WriteLine("Zapisano na kurs pomyślnie!");
    }

    static void WithdrawStudent(RegistrationSystem system)
    {
        Console.Write("ID studenta: ");
        var studentId = Console.ReadLine();
        Console.Write("Nazwa kursu: ");
        var courseName = Console.ReadLine();

        system.WithdrawStudent(studentId, courseName);
        Console.WriteLine("Wypisano z kursu pomyślnie!");
    }

    static void RemoveStudent(RegistrationSystem system)
    {
        Console.Write("ID studenta do usunięcia: ");
        var id = Console.ReadLine();
        system.RemoveStudent(id);
        Console.WriteLine("Student usunięty pomyślnie!");
    }

    static void RemoveCourse(RegistrationSystem system)
    {
        Console.Write("Nazwa kursu do usunięcia: ");
        var name = Console.ReadLine();
        system.RemoveCourse(name);
        Console.WriteLine("Kurs usunięty pomyślnie!");
    }
}
