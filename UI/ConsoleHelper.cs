namespace GitHubApiScraper.UI
{
    public static class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            Console.Clear();
            Console.WriteLine($"=== {title} ===\n");
        }

        public static void Pause(string? message = null)
        {
            if (message != null) Console.WriteLine(message);
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        public static bool AskYesNo(string prompt)
        {
            Console.Write($"{prompt} (y/n): ");
            return Console.ReadLine()?.Trim().ToLower() == "y";
        }
    }
}