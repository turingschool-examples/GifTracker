using System.ComponentModel.DataAnnotations;

namespace GifTrackerAPI.Models
{
    public class Gif
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Url is required")]
        public string Url { get; set; }
        public int Rating { get; set; }
    }
}
