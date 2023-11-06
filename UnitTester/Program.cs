using System;
using Microsoft.VisualBasic.FileIO;

class Program
{
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

    static void Main()
    {
        string filePath = "C:\\Users\\TheBoss\\source\\repos\\BulkUploader\\tester1.csv";

        try
        {
            // Call the HasTextFieldMarkedAsSearchable method with the file path
            bool hasTextFieldMarkedAsSearchable = HasTextFieldMarkedAsSearchable(filePath);

            // Print the result
            if (hasTextFieldMarkedAsSearchable)
            {
                Console.WriteLine("At least one Text field is marked as Searchable.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("No Text field is marked as Searchable.");
                Console.ReadLine();
            }
        }
        catch (Exception ex)
        {
            // Handle and display any exceptions that occur during the process
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.ReadLine();
        }
    }
}
