using ApartmentNotifier.Services;

class Program
{
    static async Task Main(string[] args)
    {
        SearchManager searchManager = new SearchManager();

        await searchManager.RunAsync();

        Console.ReadKey();
    }

    static async Task Test()
    {

    }
}