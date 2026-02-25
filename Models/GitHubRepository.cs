using System.ComponentModel.DataAnnotations;

namespace InternetTechLab1.Models
{
    public class GitHubRepository
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } 

        [Required]
        public string Url { get; set; } 

        public string? Description { get; set; }

        [Required]
        public string OwnerLogin { get; set; } 

        public string? Language { get; set; }

        public int Stars { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
