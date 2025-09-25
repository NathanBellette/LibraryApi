using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Contracts.Requests;

public class CreateBookRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Owner { get; set; } = string.Empty;

    public bool Availability { get; set; }
}


