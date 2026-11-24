using CactiShop.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CactiShop.Api.Controllers;

[ApiController]
public class ImagesController : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route(ApiRoutes.Images.UploadImage)]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0) return BadRequest(new { message = "Файл не выбран" });

        var uploadsFolder = Path.Combine(AppContext.BaseDirectory, "wwwroot/image");
        if (!Directory.Exists(uploadsFolder)) return BadRequest(new { message = $"Мамка не найдена: {uploadsFolder}" });

        var uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(uploadsFolder);
        var filepath = Path.Combine(uploadsFolder, uniqueFilename);

        using (var stream = new FileStream(filepath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return Ok(new { fileName = uniqueFilename });
    }
}
