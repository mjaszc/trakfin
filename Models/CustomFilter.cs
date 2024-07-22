namespace Trakfin.Models
{
    public class CustomFilter
    {
        public int Id { get; set; }
        public string? Bank { get; set; }
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Title { get; set; }
    }
}
