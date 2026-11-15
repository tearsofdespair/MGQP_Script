using System.Net.Mime;

namespace MGQP_Script;

public class ProgramInterface
{
    private FarmManager _manager;
    private string[] _strings;

    public ProgramInterface()
    {
        _manager = new FarmManager();
    }
    
    public void Run()
    {
        while (true)
        {
            LanguageLoader loader = new LanguageLoader();
            Console.WriteLine("Choose your language: [1] English, [2] Russian");
            int language = Convert.ToInt32(Console.ReadLine());
            if (language == 1)
            {
                _strings = loader.LoadLanguage("english");
            }
            else if (language == 2)
            {
                _strings = loader.LoadLanguage("russian");
            }
            else
            {
                Console.WriteLine("Wrong input. Please try again.");
                continue;
            }

            while (true)
            {
                Console.WriteLine(_strings[0]);
                int mode = Convert.ToInt32(Console.ReadLine());
                if (mode == 0)
                {
                    Environment.Exit(0);
                }
                else if (mode == 1)
                {
                    Console.WriteLine(_strings[1]);
                    int level = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine(_strings[2]);
                    int runs = Convert.ToInt32(Console.ReadLine());
                    _manager.Coliseum(level, runs);
                }
                else if (mode == 2)
                {
                    Console.WriteLine(_strings[3]);
                    int entitiesCount =  Convert.ToInt32(Console.ReadLine());
                    _manager.FarmEntities(entitiesCount);
                }
            }
            
        }
    }
}