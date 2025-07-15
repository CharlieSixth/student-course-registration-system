# 🎓 Student Course Registration System

**Student course registration system in C#**

Academic project for **Object-Oriented Programming** course demonstrating the application of inheritance, polymorphism, custom exceptions, interfaces, and file I/O operations in C#.

## 📋 Project Description

The system allows for managing students and courses, enrolling students in courses, withdrawing them, and logging all operations and errors to a file. The program implements a complete registration system with data persistence and comprehensive error handling.

## 🏗️ Architecture

### Main Classes

#### `Student` (implements `IComparable<Student>`)
Represents a student with personal information and enrolled courses:
```csharp
public class Student : IComparable<Student>
{
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime BirthDate { get; }
    public string StudentId { get; }
    private List<Course> EnrolledCourses;
    
    public void EnrollInCourse(Course course);
    public void WithdrawFromCourse(Course course);
    public void ListCourses();
    public int CompareTo(Student other); // Sort by LastName, then FirstName
}
```

#### `Course` (implements `IComparable<Course>`)
Represents a course with capacity management and enrolled students:
```csharp
public class Course : IComparable<Course>
{
    public string CourseName { get; }
    public int Capacity { get; }
    public DateTime StartDate { get; }
    private List<Student> EnrolledStudents;
    
    public void AddStudent(Student student);
    public void RemoveStudent(Student student);
    public void DisplayStudents();
    public int CompareTo(Course other); // Sort by StartDate
}
```

#### `RegistrationSystem` (implements `IDisposable`)
Main system class managing student and course registration:
- Implements `IDisposable` to properly close streams and save data
- All operations are logged to a text file
- Handles data persistence through file I/O
- Comprehensive exception handling

### Custom Exception Classes

The system implements custom exception classes inheriting from `Exception`:

- `CourseNotFoundException`
- `CourseFullException`
- `StudentAlreadyEnrolledException`
- `StudentNotEnrolledException`
- `CannotRemoveCourseWithEnrolledStudentsException`

Each exception class contains readable messages and additional error information when needed.

## 🎯 Features

### Interactive Menu System
The program offers a comprehensive console interface:

```
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
Wybierz opcję:
```

### Core Functionality

1. **Add New Student** - Enter new student data and add to database
2. **Add New Course** - Create new course with name, capacity limit, and start date
3. **Enroll Student in Course** - Select student and course for registration
4. **Withdraw Student from Course** - Remove student from selected course
5. **Display Students List (A-Z)** - Show students sorted alphabetically by surname
6. **Display Courses List (by Date)** - Show courses ordered from earliest to latest
7. **Display Available Courses** - Show only courses with available spots
8. **Remove Student** - Delete student from database (if not enrolled in any course)
9. **Remove Course** - Delete course (if no students are enrolled)
10. **Save and Exit** - Save all data and terminate application

## 🔧 Technologies and Concepts

- **Language:** C# (.NET 9.0)
- **Paradigm:** Object-oriented programming
- **Patterns:** Inheritance, polymorphism, composition
- **Concepts:** 
  - Custom exceptions
  - Interface implementation (`IComparable`, `IDisposable`)
  - File I/O operations
  - Data persistence
  - Logging system
  - Exception handling

## 📁 Project Structure

```
PS4-OP/
├── Program.cs              # Main application logic
├── Student.cs              # Student class implementation
├── Course.cs               # Course class implementation
├── RegistrationSystem.cs   # System management class
├── Exceptions/             # Custom exception classes
│   ├── CourseNotFoundException.cs
│   ├── CourseFullException.cs
│   └── ...
├── Data/                   # Data files
│   ├── students.txt
│   ├── courses.txt
│   └── log.txt
├── PS4-OP.csproj          # Project configuration
└── README.md              # Project documentation
```

## 🚀 Getting Started

1. Clone the repository:
```bash
git clone https://github.com/your-username/student-registration-system.git
```

2. Navigate to the project directory:
```bash
cd student-registration-system
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

## 🎓 Educational Goals

The project demonstrates:
- Implementation of custom exception classes
- Proper use of interfaces (`IComparable`, `IDisposable`)
- File I/O operations and data persistence
- Comprehensive error handling and logging
- Object composition and relationships
- Sorting and data management
- Clean separation of concerns

## 📚 Assignment Requirements

- [x] Implementation of `Student` class with `IComparable`
- [x] Implementation of `Course` class with `IComparable`
- [x] `RegistrationSystem` class with `IDisposable`
- [x] Custom exception classes
- [x] File I/O for data persistence
- [x] Comprehensive logging system
- [x] Interactive console menu
- [x] Student and course management
- [x] Registration and withdrawal functionality
- [x] Data sorting and display options

## 💾 Data Management

- **Students** and **courses** data is loaded from text files
- Missing files are handled gracefully without terminating the program
- All operations (successful and failed) are logged to file
- Data is automatically saved when exiting the application
- Proper resource cleanup through `IDisposable` implementation

## 🛡️ Error Handling

The system implements comprehensive error handling:
- Custom exceptions for specific error scenarios
- Graceful handling of file I/O errors
- Input validation and user-friendly error messages
- Complete operation logging for debugging

## 👨‍💻 Author

Project completed as part of **Object-Oriented Programming** coursework.

## 📄 License

Educational project - for academic use.
