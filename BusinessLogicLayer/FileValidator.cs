using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public  class FileValidator
    {
        /// <summary>
        /// Check if the content is a test string and line delimited by CR or CRLF
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CheckLineEnding(string text)
        {
          return text.Contains("r\n") || (text.Contains('\r') && !text.Contains("\n"));
        }

        /// <summary>
        /// Check if each line contains at least 5 fields
        /// </summary>
        /// <param name="filepath"></param>
        public static bool CheckFileFields(string filepath)
        {
            try
            {
                // Initialize the result variable to false
                bool result = false;


                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Loop through each line in the CSV file
                    while (!parser.EndOfData)
                    {
                        // Read fields from the current line
                        string[] fields = parser.ReadFields();

                        // Check if the line contains at least 5 fields
                        if (fields != null && fields.Length >= 5)
                        {
                            // Set result to true and return immediately if a line with at least 5 fields is found
                            result = true;
                            return result;
                        }
                    }
                }

                // Return the final result after checking all lines in the CSV file
                return result;
            }
            catch (Exception ex)
            {
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// Check if the first line contains at least the following fields: Name, Type, Search, Library Filter,
        /// Visible
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool CheckFileHeader(string filepath)
        {
            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    // Read the first line as headers
                    string[] headers = parser.ReadFields();

                    // Check if all required fields are present in the first line
                    string[] requiredFields = { "Name", "Type", "Search", "Library Filter", "Visible" };
                    return requiredFields.All(field => headers.Contains(field, StringComparer.OrdinalIgnoreCase));
                }
            }
            catch (Exception ex)
            {
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// Check for empty values. All 5 fields must have a value.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<string> GetColumnsWithEmptyValues(string filepath)
        {
            List<string> columnsWithEmptyValues = new List<string>();

            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

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
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }


        /// <summary>
        /// Check field name lengths <= 100 characters
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<string> GetColumnsWithLongValues(string filepath)
        {
            List<string> columnsWithLongValues = new List<string>();

            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
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
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// Check for valid field types AND type property values:
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<string> GetInvalidColumns(string filepath)
        {
            List<string> invalidColumns = new List<string>();

            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
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
                        if (!IsValidFieldType(fields[typeIndex]))
                            invalidColumns.Add("Type");

                        // Check Search, Library Filter, and Visible columns
                        if (!IsValidYesNoValue(fields[searchIndex]))
                            invalidColumns.Add("Search");

                        if (!IsValidYesNoValue(fields[libraryFilterIndex]))
                            invalidColumns.Add("Library Filter");

                        if (!IsValidYesNoValue(fields[visibleIndex]))
                            invalidColumns.Add("Visible");
                    }
                }

                return invalidColumns;
            }
            catch (Exception ex)
            {
                // Handle and rethrow the exception if any error occurs during the process
                throw;
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
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static List<string> CheckFieldPropertyRules(string filepath)
        {
            List<string> invalidColumns = new List<string>();

            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

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
                }

                return invalidColumns;
            }
            catch (Exception ex)
            {
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// Check if there are at least 2 lines in the file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>True or False</returns>
        public static bool HasAtLeastTwoLines(string filepath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filepath))
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
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// At least one field must be of type Text and marked as Searchable
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool HasTextFieldMarkedAsSearchable(string filepath)
        {
            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

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
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// At least one field must be of type Date
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool HasDateField(string filepath)
        {
            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

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
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool IsValidCSV(string filepath)
        {
            try
            {
                // Using TextFieldParser to read CSV file with semicolon (;) delimiter
                using (TextFieldParser parser = new TextFieldParser(filepath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

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
                // Handle and rethrow the exception if any error occurs during the process
                throw;
            }
        }
    }
}
