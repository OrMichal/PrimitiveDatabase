namespace ConsoleApp17
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataService service = new DataService();
            DataServiceResult result = service.ReadAll();

            Table table = new Table();
            table.Headers = result.Headers;
            table.Data = result.Rows;

            Console.CursorVisible = false;

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                table.Draw();

                ConsoleKeyInfo info = Console.ReadKey();
                table.HandleKey(info);
            }

        }
    }
}
