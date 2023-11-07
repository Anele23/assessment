using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class ValidateCSV
    {
        public static string ValidateCsv(byte[] fileBytes)
        {
            try
            {
                if (!FileValidator.IsValidCsv(fileBytes))
                {
                    return "Invalid CSV file.";
                }

                if (!FileValidator.CheckLineEnding(fileBytes))
                {
                    return "Incorrect line endings. Lines must be delimited by CR or CRLF.";
                }

                if (!FileValidator.CheckFileFields(fileBytes))
                {
                    return "Each line in the CSV file must contain at least 5 fields.";
                }

                if (!FileValidator.CheckFileHeader(fileBytes))
                {
                    return "The first line must contain the following fields: Name, Type, Search, Library Filter, Visible.";
                }

                var columnsWithEmptyValues = FileValidator.GetColumnsWithEmptyValues(fileBytes);
                if (columnsWithEmptyValues.Count > 0)
                {
                    return $"The value in the {string.Join(", ", columnsWithEmptyValues)} column on row 2 is empty. Update the CSV and try to import again.";
                }

                var columnsWithLongValues = FileValidator.GetColumnsWithLongValues(fileBytes);
                if (columnsWithLongValues.Count > 0)
                {
                    return "The field name cannot exceed 100 characters";
                }

                var invalidColumns = FileValidator.GetInvalidColumns(fileBytes);
                if (invalidColumns.Count > 0)
                {
                    return $"Invalid columns: {string.Join(", ", invalidColumns)}";
                }

                if (!FileValidator.HasAtLeastTwoLines(fileBytes))
                {
                    return "CSV file must have at least 2 lines.";
                }

                if (!FileValidator.HasTextFieldMarkedAsSearchable(fileBytes))
                {
                    return "At least one Text field must be marked as Searchable.";
                }

                if (!FileValidator.HasDateField(fileBytes))
                {
                    return "At least one Date field is required.";
                }

                return "SUCCESS";
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while validating the CSV file.", ex);
            }
        }
    }
}
