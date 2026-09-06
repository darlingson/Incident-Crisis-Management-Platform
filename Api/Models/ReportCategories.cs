namespace Api.Models
{
    public class ReportCategories
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public int CategoryId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Report Report { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
}
