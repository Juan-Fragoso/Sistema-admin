namespace ControlCampus.Models.ViewModels
{
    public class GroupSubjectDetailViewModel
    {
        public CourseAssignment CourseAssignment { get; set; } = null!;
        public Student? Student { get; set; }
        public Enrollment? Enrollment { get; set; }
        public Grade? Grade { get; set; }
        public string? Boleta { get; set; }
    }
}
