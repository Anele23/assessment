using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ValidateCSV
    {
        public static string CSVFileCheck(byte[] fileBytes)
        {
            try
            {
                // Check if the CSV file is valid
                if (FileValidator.IsValidCSV(fileBytes))
                {
                    // CSV file is valid, call other validation functions
                    if (FileValidator.CheckLineEnding(fileBytes))
                    {
                        // Check if each line contains at least 5 fields
                        if (FileValidator.CheckFileFields(fileBytes))
                        {
                            // Check if the first line contains required fields
                            if (FileValidator.CheckFileHeader(fileBytes))
                            {
                                // Get columns with empty values
                                List<string> columnsWithEmptyValues = FileValidator.GetColumnsWithEmptyValues(fileBytes);

                                // Check if there was an error
                                if (columnsWithEmptyValues.Count > 0)
                                {
                                    string error = "The value in the" + string.Join(", ", columnsWithEmptyValues) + "column on row 2 is empty. Update the CSV and\r\ntry to import again.: ";

                                    return error;
                                }

                                // Get columns with values longer than 100 characters
                                List<string> columnsWithLongValues = FileValidator.GetColumnsWithLongValues(fileBytes);

                                if (columnsWithEmptyValues.Count > 0)
                                {
                                    string error = "The field name cannot exceed 100 characters";

                                    return error;
                                }

                                // Get invalid columns based on field types and properties
                                List<string> invalidColumns = FileValidator.GetInvalidColumns(fileBytes);

                                // Check if there was an error
                                if (invalidColumns.Count > 0)
                                {
                                    string error = "Invalid columns: " + string.Join(", ", invalidColumns);

                                    return error;
                                }

                                // Check if there are at least 2 lines in the file
                                if (FileValidator.HasAtLeastTwoLines(fileBytes))
                                {
                                    // At least one field must be of type Text and marked as Searchable
                                    if (FileValidator.HasTextFieldMarkedAsSearchable(fileBytes))
                                    {
                                        // At least one field must be of type Date
                                        if (FileValidator.HasDateField(fileBytes))
                                        {
                                            // All validations passed, return success response
                                            return "SUCCESS";
                                        }
                                        else
                                        {
                                            // Handle the case where there is no Date field
                                            return "At least one Date field is required.";
                                        }
                                    }
                                    else
                                    {
                                        // Handle the case where there is no Text field marked as Searchable
                                        return "At least one Text field must be marked as Searchable.";
                                    }
                                }
                                else
                                {
                                    // Handle the case where there are not enough lines in the file
                                    return "CSV file must have at least 2 lines.";
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
                        return "Incorrect line endings. Lines must be delimited by CR or CRLF.";
                    }
                }
                else
                {
                    // Handle the case where the CSV file is not valid
                    return "Invalid CSV file.";
                }
            }
            catch (Exception ex)
            {
                // Handle and rethrow the exception if any error occurs during the process
                throw new CustomValidationException("Error occurred while validating the CSV file.", ex);
            }
        }
    }
}
