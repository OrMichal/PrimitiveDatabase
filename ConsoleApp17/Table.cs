using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp17
{
    public class Table : DataService
    {
        private int selected = 0;
        private int offset = 0;
        private int choiceIndex = 0;
        private int changeItemIndex = 0;

        private bool activeChange = false; 
        private bool wantToExit = false;
        private bool wantToRewrite = false;
        private bool wantToSave = false;
        private bool onNumber = false;
        private bool onText = false;
        private bool onPrimaryKey = false;

        public string[] Headers { get; set; }

        public List<Row> Data { get; set; }

        public int Limit { get; set; } = 10;

        public void HandleKey(ConsoleKeyInfo info)
        {
            Console.CursorVisible = false;
            if (info.Key == ConsoleKey.UpArrow)
            {
                MoveUp();
            }
            else if (info.Key == ConsoleKey.DownArrow)
            {
                MoveDown();

            }
            
            if (info.Key == ConsoleKey.Tab)
            {
                ToggleChangeMenu();
            }
            
            if(info.Key == ConsoleKey.Escape)
            {
                ExitChangeMenu();
            }
            
            if(info.Key == ConsoleKey.Enter && wantToExit)
            {
                ExitChangeMenu();
            }
            
            if(info.Key == ConsoleKey.Backspace && activeChange)
            {
                DeleteByOne();
            }
            
            if(char.IsLetterOrDigit(info.KeyChar) && activeChange)
            {
                RewriteData(info);
            }
            
            if(info.Key == ConsoleKey.Enter && wantToSave)
            {
                SaveChanges();
            }
        }

        private void MoveUp()
        {
            if (!activeChange && this.selected > 0)
            {
                this.selected--;
            }
            else if (activeChange && this.selected < 6 && this.selected > 0)
            {
                this.selected--;
                this.changeItemIndex--;
            }

            if (this.selected < this.offset)
            {
                this.offset--;
            }
        }
        private void MoveDown()
        {
            if (!activeChange && this.selected < this.Data.Count - 2)
            {
                this.selected++;
            }
            else if (activeChange && this.selected < 5)
            {
                this.selected++;
                this.changeItemIndex++;
            }


            if (this.selected >= this.offset + this.Limit)
            {
                this.offset++;
            }
        }
        private void ToggleChangeMenu()
        {
            if (this.activeChange == false)
            {
                this.activeChange = true;
            }
            else if (this.activeChange)
            {
                this.activeChange = false;
            }
            choiceIndex = this.selected;
            this.selected = 0;
            Console.Clear();
        }
        private void ExitChangeMenu()
        {
            activeChange = false;
            this.selected = 0;
        }
        private void DeleteByOne()
        {
            this.Data[choiceIndex].Values[this.selected] = rewritten(this.Data[choiceIndex].Values[this.selected]);
            Console.SetCursorPosition(0, 0);
            Draw();
        }
        private void RewriteData(ConsoleKeyInfo info)
        {
            if (onText && char.IsLetter(info.KeyChar))
            {
                this.Data[choiceIndex].Values[this.selected] = ChangeData(this.Data[choiceIndex].Values[this.selected], info.KeyChar);
                Console.SetCursorPosition(0, 0);
                Draw();
            }
            else if (onNumber && !onPrimaryKey && char.IsLetterOrDigit(info.KeyChar))
            {
                this.Data[choiceIndex].Values[this.selected] = ChangeData(this.Data[choiceIndex].Values[this.selected], info.KeyChar);
                Console.SetCursorPosition(0, 0);
                Draw();
            }
        }
        private void SaveChanges()
        {
            base.SaveToTxt(this.Data, this.Headers);
            wantToSave = false;
            Console.WriteLine("Your changes has been successfully saved");
            Thread.Sleep(400);
            activeChange = false;
        }

        public void Draw()
        {
            if (!activeChange)
            {
                DrawTable();
            }
            else
            {
                DrawChangeMenu();
            }
        }

        private void DrawTable()
        {
            int[] widths = this.Widths();

            this.DrawLine(widths);
            this.DrawRow(this.Headers, widths);
            this.DrawLine(widths);
            if (this.Data.Count > (offset + Limit))
            {
                for (int i = offset; i < offset + Limit; i++)
                {
                    Row row = this.Data[i];

                    if (i == this.selected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        this.DrawRow(row.Values, widths);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ResetColor();
                        this.DrawRow(row.Values, widths);
                    }

                }
            }

            this.DrawLine(widths);
        }

        private void DrawChangeMenu()
        {
            if (this.activeChange == true)
            {
                Console.ResetColor();
                Console.SetCursorPosition(0, 0);

                string[] operations = { "cancel", "apply & exit" };

                for (int i = 0; i < (this.Data[choiceIndex].Values.Count() + operations.Count()); i++)
                {
                    if (i == this.selected)
                    {

                        if (i == this.Data[choiceIndex].Values.Count() + 0)
                        {
                            wantToExit = true;
                        }
                        else
                        {
                            wantToExit = false;
                        }

                        if (i == 0)
                        {
                            onPrimaryKey = true;
                        }
                        else if (i > 1 && i < this.Data[choiceIndex].Values.Count() && int.TryParse(this.Data[choiceIndex].Values[i], out int _))
                        {
                            onNumber = true;
                            onText = false;
                            onPrimaryKey = false;
                        }
                        else
                        {
                            onNumber = false;
                            onText = true;
                        }

                        if (i < this.Data[choiceIndex].Values.Count() && wantToRewrite)
                        {
                            wantToRewrite = false;
                        }
                        else
                        {
                            wantToRewrite = false;
                            onNumber = false;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            DrawItemForChange(this.Data[choiceIndex].Values, operations, i);
                        }

                        if (i == 5)
                        {
                            wantToSave = true;
                        }


                    }
                    else
                    {
                        Console.ResetColor();
                        DrawItemForChange(this.Data[choiceIndex].Values, operations, i);
                    }
                }
            }
        }

        private List<string> GetRowData(string[] values)
        {
            List<string> list = new List<string>();
            foreach(string item in values)
            {
                list.Add(item.ToString());
            }
            return list;
        }

        private void DrawItemForChange(string[] values,string[] operations, int index)
        {
            if(index < values.Count())
            {
                List<string> data = GetRowData(values);
                Console.Write($"_{data[index].PadRight(30, '_')}");
                Console.WriteLine("_");
            }
            else if(index >= values.Count() && index < (operations.Count() + values.Count()))
            {
                Console.Write($"[{operations[index - values.Count()]}");
                Console.WriteLine("]");

            }
        }

        private string ChangeData(string data, char c)
        {
            data += c.ToString();
            return data;
        }

        private string rewritten(string text)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach(char c in text)
            {
                if(i++ < text.Length - 1)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
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
