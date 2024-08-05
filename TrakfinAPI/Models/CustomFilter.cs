using System.ComponentModel.DataAnnotations;

namespace TrakfinAPI.Models
{
    public class CustomFilter
    {
        public int Id { get; set; }
        public string? Bank { get; set; }
        public string? Category { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string? Title { get; set; }
    }
}
