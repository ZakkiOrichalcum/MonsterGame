using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace TCG.Utility
{
    public class CSVReader
    {
        public static Table<string> ReadInCSVIntoTable(string path, char delimiter, bool firstRowIsTitles = false)
        {
            var output = new List<string[]>();

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    output.Add(ScrubOutsideQuotesFromItems(Regex.Split(line, delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"), delimiter));
                }
            }

            return new Table<string>(output.ToArray(), firstRowIsTitles);
        }

        public static Table<string> ReadStringCSVIntoTable(string csvString, char delimiter, bool firstRowIsTitles = false)
        {
            var output = new List<string[]>();

            var reader = csvString.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (var line in reader)
            {
                output.Add(ScrubOutsideQuotesFromItems(Regex.Split(line, delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"), delimiter));
            }

            return new Table<string>(output.ToArray(), firstRowIsTitles);
        }

        public static Table<string> ReadInNumberOfRowsOfCSVIntoTable(string path, char delimiter, int numberOfLines, bool firstRowIsTitles = false)
        {
            var output = new List<string[]>();
            var index = 0;

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                while (!reader.EndOfStream && index < numberOfLines)
                {
                    var line = reader.ReadLine();
                    output.Add(ScrubOutsideQuotesFromItems(Regex.Split(line, delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"), delimiter));
                    index++;
                }
            }

            return new Table<string>(output.ToArray(), firstRowIsTitles);
        }

        public static Table<string> ReadNumberOfRowsOfStringCSVIntoTable(string csvString, char delimiter, int numberOfLines, bool firstRowIsTitles = false)
        {
            var output = new List<string[]>();

            var reader = csvString.Split(new string[] { "\n" }, StringSplitOptions.None);
            for (var i = 0; i < numberOfLines; i++)
            {
                output.Add(ScrubOutsideQuotesFromItems(Regex.Split(reader[i], delimiter + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"), delimiter));
            }

            return new Table<string>(output.ToArray(), firstRowIsTitles);
        }

        private static string[] ScrubOutsideQuotesFromItems(string[] stringArr, char delimiter)
        {
            var output = new string[stringArr.Length];

            for (int i = 0; i < stringArr.Length; i++)
            {
                var item = stringArr[i];
                if (item.Contains(delimiter) && item.Substring(0, 1) == "\"" && item.Substring(item.Length - 1, 1) == "\"")
                    item = item.Substring(1, item.Length - 2);
                output[i] = item.Replace("\r", "").Replace("\n", "");
            }

            return output;
        }
    }
}