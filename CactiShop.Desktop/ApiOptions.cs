using System.ComponentModel.DataAnnotations;

namespace CactiShop.Desktop;

public class ApiOptions
{
    public const string SectionName = "Api";

    [Required, Url]
    public string BaseUrl { get; set; } = "";
    public bool UseFake { get; set; }
}
