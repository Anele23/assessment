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
        public static void CheckFileFields(string filepath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] fields = line.Split(',');
                        if (fields.Length > 5)
                        {
                            return;
                        }
                        else
                        {
                            // Send out err message
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool CheckFileHeader(string filepath)
        {

            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                  string firstLine = sr.ReadLine();
                    if (firstLine != null)
                    {
                        string[] headers = firstLine.Split(",");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
