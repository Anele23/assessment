using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer;

namespace CSVUploadAPI.Controllers
{
    [ApiController]
    [Route("api/csv")]
    public class CSVController : Controller
    {
        [HttpPost]
        [Route("upload")]
        public IActionResult UploadCsv([FromForm] IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Invalid file");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();

                    string result = ValidateCSV.CSVFileCheck(fileBytes);

                    if (result != "SUCCESS") 
                    {
                       return Ok(result);
                    }

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
