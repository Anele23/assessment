using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

class Program
{
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

    static void Main()
    {
        string filePath = "C:\\Users\\TheBoss\\source\\repos\\BulkUploader\\tester1.csv";

        try
        {
            // Call the GetColumnsWithLongValues method with the file path
            List<string> columnsWithLongValues = GetColumnsWithLongValues(filePath);

            // Print the result
            if (columnsWithLongValues.Count > 0)
            {
                Console.WriteLine("Columns with values longer than 100 characters: " + string.Join(", ", columnsWithLongValues));
            }
            else
            {
                Console.WriteLine("No columns have values longer than 100 characters.");
            }
        }
        catch (Exception ex)
        {
            // Handle and display any exceptions that occur during the process
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}
