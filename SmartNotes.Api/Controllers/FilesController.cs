using Microsoft.AspNetCore.Mvc;

namespace SmartNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var url = $"https://mock-s3.com/{file.FileName}";

        return Ok(new
        {
            FileName = file.FileName,
            Url = url
        });
    }
}