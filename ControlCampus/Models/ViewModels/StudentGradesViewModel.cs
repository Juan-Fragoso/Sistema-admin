namespace ControlCampus.Models.ViewModels
{
    public class StudentGradesViewModel
    {
        public List<Grade> Grades { get; set; } = new List<Grade>();
        public decimal Average { get; set; }
    }
}
