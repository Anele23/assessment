using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ValidateCSV
    {
        public static string inputFileV(string filePath)
        {
            try
            {
                // Check if the CSV file is valid
                if (FileValidator.IsValidCSV(filePath))
                {
                    // CSV file is valid, call other validation functions
                    if (FileValidator.CheckLineEnding(File.ReadAllText(filePath)))
                    {
                        // Check if each line contains at least 5 fields
                        if (FileValidator.CheckFileFields(filePath))
                        {
                            // Check if the first line contains required fields
                            if (FileValidator.CheckFileHeader(filePath))
                            {
                                // Get columns with empty values
                                List<string> columnsWithEmptyValues = FileValidator.GetColumnsWithEmptyValues(filePath);

                                // Get columns with values longer than 100 characters
                                List<string> columnsWithLongValues = FileValidator.GetColumnsWithLongValues(filePath);

                                // Get invalid columns based on field types and properties
                                List<string> invalidColumns = FileValidator.GetInvalidColumns(filePath);

                                // Check if there are at least 2 lines in the file
                                if (FileValidator.HasAtLeastTwoLines(filePath))
                                {
                                    // At least one field must be of type Text and marked as Searchable
                                    if (FileValidator.HasTextFieldMarkedAsSearchable(filePath))
                                    {
                                        // At least one field must be of type Date
                                        if (FileValidator.HasDateField(filePath))
                                        {
                                            // All validations passed, return success response
                                            return Ok("CSV file is valid.");
                                        }
                                        else
                                        {
                                            // Handle the case where there is no Date field
                                            return BadRequest("At least one Date field is required.");
                                        }
                                    }
                                    else
                                    {
                                        // Handle the case where there is no Text field marked as Searchable
                                        return BadRequest("At least one Text field must be marked as Searchable.");
                                    }
                                }
                                else
                                {
                                    // Handle the case where there are not enough lines in the file
                                    return BadRequest("CSV file must have at least 2 lines.");
                                }
                            }
                            else
                            {
                                // Handle the case where the first line does not contain required fields
                                return "The first line must contain the following fields: Name, Type, Search, Library Filter, Visible.";
                            }
                        }
                        else
                        {
                            // Handle the case where each line does not contain at least 5 fields
                            return "Each line in the CSV file must contain at least 5 fields.";
                        }
                    }
                    else
                    {
                        // Handle the case where the line endings are incorrect
                        return BadRequest("Incorrect line endings. Lines must be delimited by CR or CRLF.");
                    }
                }
                else
                {
                    // Handle the case where the CSV file is not valid
                    return BadRequest("Invalid CSV file.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the validation process
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
