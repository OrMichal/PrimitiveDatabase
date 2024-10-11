using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp17
{
    public class DataService
    {
        const string FILE = @"C:\users\misao\desktop\people.txt";

        public DataServiceResult ReadAll()
        {
            string[] headers = null;
            List<Row> rows = new List<Row>();

            using (StreamReader reader = new StreamReader(FILE))
            {
                while (!reader.EndOfStream)
                {
                    string[] parts = reader.ReadLine().Split(';');

                    if (headers == null)
                    {
                        headers = parts;
                    }
                    else
                    {
                        rows.Add(new Row(parts));
                    }
                }
            }

            return new DataServiceResult()
            {
                Headers = headers,
                Rows = rows
            };
        }

        public void SaveToTxt(List<Row> rows, string[] headers)
        {
            using(StreamWriter writer = new StreamWriter(FILE))
            {
                writer.WriteLine($"{headers[0]};{headers[1]};{headers[2]};{headers[3]}");
                foreach (Row row in rows)
                {
                    writer.WriteLine($"{row.Values[0]};{row.Values[1]};{row.Values[2]};{row.Values[3]}");
                }
            }
        }
    }
}
