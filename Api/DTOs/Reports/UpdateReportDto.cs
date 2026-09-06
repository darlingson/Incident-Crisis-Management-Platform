namespace Api.DTOs.Reports
{
    public class ReportUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Narrative { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class UserSelectionDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}