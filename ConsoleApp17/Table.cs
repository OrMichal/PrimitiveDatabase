using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp17
{
    public class Table
    {
        private int selected = 0;
        private int offset = 0;


        public string[] Headers { get; set; }

        public List<Row> Data { get; set; }

        public int Limit { get; set; } = 10;

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.UpArrow && this.selected > 0)
            {
                this.selected--;
                if (this.selected < this.offset)
                {
                    this.offset--;
                }
            }
            else if (info.Key == ConsoleKey.DownArrow && this.selected < this.Data.Count - 1)
            {
                this.selected++;
                if (this.selected >= this.offset + this.Limit)
                {
                    this.offset++;
                }
            }
        }

        public void Draw()
        {
            int[] widths = this.Widths();

            this.DrawLine(widths);
            this.DrawRow(this.Headers, widths);
            this.DrawLine(widths);
            if(this.Data.Count > (offset + Limit))
            {
                for (int i = offset; i < offset + Limit; i++)
                {
                    Row row = this.Data[i];

                    if (i == this.selected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    this.DrawRow(row.Values, widths);

                    Console.ResetColor();
                }
            }
            

            this.DrawLine(widths);
        }

        private void DrawRow(string[] values, int[] widths)
        {
            int i = 0;
            foreach (string item in values)
            {
                Console.Write($"| {item.PadRight(widths[i++], ' ')} ");
            }
            Console.WriteLine("|");
        }

        private void DrawLine(int[] widths)
        {
            foreach (int length in widths)
            {
                string dashes = "".PadRight(length + 2, '-');
                Console.Write($"+{dashes}");
            }
            Console.WriteLine("+");
        }

        private int[] Widths()
        {
            int[] widths = new int[this.Headers.Length];

            int i = 0;
            foreach (string item in this.Headers)
            {
                widths[i++] = item.Length;
            }

            foreach (Row row in this.Data)
            {
                i = 0;
                foreach (string item in row.Values)
                {
                    if (item.Length > widths[i])
                    {
                        widths[i] = item.Length;
                    }

                    i++;
                }
            }

            return widths;
        }
    }
}
