using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class FileValidator
    {
        /// <summary>
        /// Check if the content is a test string and line delimited by CR or CRLF
        /// </summary>
        /// <param name=fileBytes"></param>
        /// <returns></returns>
        public static bool CheckLineEnding(byte[] fileBytes)
        {
            try
            {
                string content = Encoding.UTF8.GetString(fileBytes);
                bool isCommaSeparated = IsCommaSeparated(content);
                bool isCRorCRLF = IsCRorCRLF(content);

                if (isCommaSeparated && isCRorCRLF)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while checking line endings in the CSV file.", ex);
            }
        }

        // Check if the CSV file is comma-separated
        static bool IsCommaSeparated(string content)
        {
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (!line.Contains(","))
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsCRorCRLF(string content)
        {
            if (content.Contains("\r\n"))
            {
                return true; // File uses CRLF line endings
            }
            else if (content.Contains("\r"))
            {
                return true; // File uses CR line endings
            }
            else
            {
                return false; // File uses LF line endings
            }
        }

        /// <summary>
        /// Check if each line contains at least 5 fields
        /// </summary>
        /// <param name="fileBytes"></param>
        public static bool CheckFileFields(byte[] fileBytes)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(fileBytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Loop through each line in the CSV file
                    while (!parser.EndOfData)
                    {
                        // Read fields from the current line
                        string[] fields = parser.ReadFields();

                        // Check if the line contains at least 5 fields
                        if (fields != null && fields.Length >= 5)
                        {
                            // If a line with at least 5 fields is found, return true immediately
                            return true;
                        }
                    }
                }

                // If no line with at least 5 fields is found, return false
                return false;
            }
            catch (Exception ex)
            {
                
                throw new CustomValidationException("Error occurred while checking file fields in the CSV file.", ex);
            }
        }


        /// <summary>
        /// Check if the first line contains at least the following fields: Name, Type, Search, Library Filter,
        /// Visible
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static bool CheckFileHeader(byte[] fileBytes)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(fileBytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the first line as headers
                    string[] headers = parser.ReadFields();

                    // Check if all required fields are present in the first line
                    string[] requiredFields = { "Name", "Type", "Search", "Library Filter", "Visible" };
                    return requiredFields.All(field => headers.Contains(field, StringComparer.OrdinalIgnoreCase));
                }
            }
            catch (Exception ex)
            {
                
                throw new CustomValidationException("Error occurred while checking file header in the CSV file.", ex);
            }
        }


        /// <summary>
        /// Check for empty values. All 5 fields must have a value.
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static List<string> GetColumnsWithEmptyValues(byte[] fileBytes)
        {
            List<string> columnsWithEmptyValues = new List<string>();

            try
            {
                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(fileBytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the first line as headers
                    string[] headers = parser.ReadFields();

                    if (headers != null)
                    {
                        // Initialize arrays to track empty values in columns
                        bool[] isEmptyColumn = new bool[headers.Length];

                        // Loop through each line in the CSV file
                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields();

                            // Check for empty values in each column
                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (string.IsNullOrWhiteSpace(fields[i]))
                                {
                                    // Mark the column as having empty values
                                    isEmptyColumn[i] = true;
                                }
                            }
                        }

                        // Add headers of columns with empty values to the list
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (isEmptyColumn[i])
                            {
                                columnsWithEmptyValues.Add(headers[i]);
                            }
                        }
                    }
                }

                return columnsWithEmptyValues;
            }
            catch (Exception ex)
            {
                
                throw new CustomValidationException("Error occurred while checking columns with empty values in the CSV file.", ex);
            }
        }




        /// <summary>
        /// Check field name lengths <= 100 characters
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static List<string> GetColumnsWithLongValues(byte[] fileBytes)
        {
            List<string> columnsWithLongValues = new List<string>();

            try
            {
                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(fileBytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Read the first line as headers
                    string[] headers = parser.ReadFields();

                    if (headers != null)
                    {
                        // Loop through each column and check the length of values
                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields();

                            // Check if any value in this column has length greater than 100 characters
                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(fields[i]) && fields[i].Length > 100)
                                {
                                    // Value in this column exceeds 100 characters, add header to the list
                                    columnsWithLongValues.Add(headers[i]);
                                    break; // Move to the next column
                                }
                            }
                        }
                    }

                    return columnsWithLongValues;
                }
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while checking columns with long values in the CSV file.", ex);
            }
        }

        /// <summary>
        /// Check for valid field types AND type property values:
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static List<string> GetInvalidColumns(byte[] fileBytes)
        {
            List<string> invalidColumns = new List<string>();

            try
            {
                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(fileBytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Read the header line to get column indexes
                    string[] headers = parser.ReadFields();
                    int typeIndex = Array.IndexOf(headers, "Type");
                    int searchIndex = Array.IndexOf(headers, "Search");
                    int libraryFilterIndex = Array.IndexOf(headers, "Library Filter");
                    int visibleIndex = Array.IndexOf(headers, "Visible");

                    // Validate values in each row
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        // Check Type column
                        if (typeIndex >= 0 && !IsValidFieldType(fields[typeIndex]))
                            invalidColumns.Add("Type");

                        // Check Search, Library Filter, and Visible columns
                        if (searchIndex >= 0 && !IsValidYesNoValue(fields[searchIndex]))
                            invalidColumns.Add("Search");

                        if (libraryFilterIndex >= 0 && !IsValidYesNoValue(fields[libraryFilterIndex]))
                            invalidColumns.Add("Library Filter");

                        if (visibleIndex >= 0 && !IsValidYesNoValue(fields[visibleIndex]))
                            invalidColumns.Add("Visible");
                    }
                }

                return invalidColumns;
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while validating columns in the CSV file.", ex);
            }
        }



        public static bool IsValidFieldType(string type)
        {
            // Valid field types: Text, Number, Yes/No, Date, Image
            string[] validFieldTypes = { "Text", "Number", "Yes/No", "Date", "Image" };
            return Array.Exists(validFieldTypes, fieldType => fieldType.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsValidYesNoValue(string value)
        {
            // Valid values: Yes, No
            return value.Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("No", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check field type property rules
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static List<string> CheckFieldPropertyRules(byte[] fileBytes)
        {
            List<string> invalidColumns = new List<string>();

            try
            {
                string content = Encoding.UTF8.GetString(fileBytes);

                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(fileBytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the header line to get column indexes
                    string[] headers = parser.ReadFields();
                    int typeIndex = Array.IndexOf(headers, "Type");
                    int searchIndex = Array.IndexOf(headers, "Search");
                    int libraryFilterIndex = Array.IndexOf(headers, "Library Filter");
                    int visibleIndex = Array.IndexOf(headers, "Visible");

                    int rowIndex = 1; // Variable to keep track of the current row number

                    // Validate values in each row
                    while (!parser.EndOfData)
                    {
                        rowIndex++;
                        string[] fields = parser.ReadFields();

                        // Check Type column and corresponding properties
                        string fieldType = fields[typeIndex];
                        string searchProperty = fields[searchIndex];
                        string libraryFilterProperty = fields[libraryFilterIndex];
                        string visibleProperty = fields[visibleIndex];

                        // Validate based on field type and properties
                        if (fieldType.Equals("Text", StringComparison.OrdinalIgnoreCase))
                        {
                            // Text fields can be searchable, filterable, and visible
                            if (!IsValidYesNoValue(searchProperty) || !IsValidYesNoValue(libraryFilterProperty))
                            {
                                invalidColumns.Add($"The value in the 'Search' or 'Library Filter' column on row {rowIndex} is invalid. Text fields can be searched or filtered.");
                            }
                        }
                        else if (fieldType.Equals("Number", StringComparison.OrdinalIgnoreCase) ||
                                 fieldType.Equals("Date", StringComparison.OrdinalIgnoreCase) ||
                                 fieldType.Equals("Yes/No", StringComparison.OrdinalIgnoreCase))
                        {
                            // Number, Date, and Yes/No fields can only be visible
                            if (!IsValidYesNoValue(searchProperty) || !IsValidYesNoValue(libraryFilterProperty))
                            {
                                invalidColumns.Add($"The value in the 'Search' or 'Library Filter' column on row {rowIndex} is invalid. {fieldType} fields cannot be searched or filtered.");
                            }
                        }
                        else if (fieldType.Equals("Image", StringComparison.OrdinalIgnoreCase))
                        {
                            // Image fields can only be visible
                            if (!IsValidYesNoValue(searchProperty) || !IsValidYesNoValue(libraryFilterProperty) || !IsValidYesNoValue(visibleProperty))
                            {
                                invalidColumns.Add($"Invalid properties in columns on row {rowIndex}. An Image field cannot be searched, filtered, or invisible.");
                            }
                        }
                    }

                    return invalidColumns;
                }
            }
            catch (Exception ex)
            {
                // Handle and rethrow the exception if any error occurs during the process
                throw new CustomValidationException("Error occurred while checking field properties in the CSV file.", ex);
            }
        }



        /// <summary>
        /// Check if there are at least 2 lines in the file
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns>True or False</returns>
        public static bool HasAtLeastTwoLines(byte[] fileBytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(fileBytes))
                using (StreamReader reader = new StreamReader(stream))
                {
                    int lineCount = 0;

                    // Read each line in the file
                    while (reader.ReadLine() != null)
                    {
                        lineCount++;

                        // If there are at least 2 lines, return true
                        if (lineCount >= 2)
                        {
                            return true;
                        }
                    }

                    // If the loop completes and there are fewer than 2 lines, return false
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while checking if there are at least two lines.", ex);
            }
        }


        /// <summary>
        /// At least one field must be of type Text and marked as Searchable
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool HasTextFieldMarkedAsSearchable(byte[] fileBytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(fileBytes))
                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the header line to get column indexes
                    string[] headers = parser.ReadFields();
                    int typeIndex = Array.IndexOf(headers, "Type");
                    int searchIndex = Array.IndexOf(headers, "Search");

                    // Validate values in each row
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        // Check if the field is of type Text and marked as Searchable
                        if (typeIndex >= 0 && searchIndex >= 0 && fields[typeIndex].Equals("Text", StringComparison.OrdinalIgnoreCase) && fields[searchIndex].Equals("Yes", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    // No Text field marked as Searchable found
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while checking if there is a Text field marked as Searchable.", ex);
            }
        }


        /// <summary>
        /// At least one field must be of type Date
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static bool HasDateField(byte[] fileBytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(fileBytes))
                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the header line to get column indexes
                    string[] headers = parser.ReadFields();
                    int typeIndex = Array.IndexOf(headers, "Type");

                    // Validate values in each row
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        // Check if the field is of type Date
                        if (typeIndex >= 0 && fields[typeIndex].Equals("Date", StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                    // No Date field found
                    return false;
                }
            }
            catch (Exception ex)
            {
                
                throw new CustomValidationException("Error occurred while checking if there is a Date field.", ex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static bool IsValidCSV(byte[] fileBytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(fileBytes))
                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the header line to get column count
                    string[] headers = parser.ReadFields();
                    int columnCount = headers.Length;

                    // Check if the number of fields in each row does not exceed 100
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        // Check if the number of fields exceeds 100
                        if (fields.Length > 100)
                        {
                            return false;
                        }
                    }

                    // Check if the number of columns (fields in the header) also exceeds 100
                    if (columnCount > 100)
                    {
                        return false;
                    }

                    // CSV is valid if the number of fields and columns are within the limit
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new CustomValidationException("Error occurred while validating the CSV file.", ex);
            }
        }
    }
}
