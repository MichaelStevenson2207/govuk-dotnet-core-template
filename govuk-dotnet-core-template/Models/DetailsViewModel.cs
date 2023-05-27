using System.ComponentModel.DataAnnotations;

namespace govuk_dotnet_core_template.Models
{
    public sealed class DetailsViewModel
    {
        [Required(ErrorMessage = "Enter your name")]
        [StringLength(100, ErrorMessage = "{0} must be a string with a maximum length of {1}")]
        public string? Name { get; set; }
    }
}