using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Api.DTOs.Reports
{
    public class CreateReportDto
    { 
        [Required]
        public string Title { get; set; } = String.Empty;
        [Required]
        public string Type { get; set; } = String.Empty;
        [Required]
        public string Location { get; set; } = String.Empty;
        [Required]
        public string Narrative { get; set; } = String.Empty;
        [Required]
        public string Impact { get; set; } = String.Empty;
        [Required]
        public string Description { get; set; } = String.Empty;
    }
}