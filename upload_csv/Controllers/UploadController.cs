using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using upload_csv.Models;

namespace upload_csv.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly DataContext _context;

        public UploadController(DataContext context)
        {
            _context = context;
        }

        // GET: api/csv/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CSV>> GetCSV(int id)
        {
            var csv = await _context.CSV_data.FindAsync(id);

            if (csv == null)
            {
                return NotFound();
            }

            return Ok(csv);
        }


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
                    BulkInsertFromCsv(fileBytes);

                    return Ok(result.ToLower());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private void BulkInsertFromCsv(byte[] csvData)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Search", typeof(string));
            dataTable.Columns.Add("Library Filter", typeof(string));
            dataTable.Columns.Add("Visible", typeof(string));

            // Read CSV data from byte array and populate the DataTable
            using (MemoryStream stream = new MemoryStream(csvData))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                int counter = 1;
                bool isFirstRow = true; // Flag to skip the first row
                while ((line = reader.ReadLine()) != null)
                {
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue; // Skip the first row
                    }
                        
            
                    string[] values = line.Split(',');
                    counter++;
                    if (values.Length >= 5)
                    {
                        dataTable.Rows.Add(counter,values[0], values[1], values[2], values[3], values[4]);
                    }
                }
            }

            // Perform bulk insert using SqlBulkCopy
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_context.Database.GetDbConnection().ConnectionString))
            {
                bulkCopy.DestinationTableName = "CSV_data";
                foreach (DataColumn column in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
