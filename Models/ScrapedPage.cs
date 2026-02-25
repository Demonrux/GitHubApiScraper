using System.ComponentModel.DataAnnotations;

namespace InternetTechLab1.Models
{
    public class ScrapedPage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? H1 { get; set; }

        public string? Description { get; set; }

        public string? LinksJson { get; set; }  

        public string? ContentPreview { get; set; }

        public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;

    }
}
