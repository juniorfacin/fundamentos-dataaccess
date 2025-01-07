namespace BaltaDataAccess.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string Phone { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}