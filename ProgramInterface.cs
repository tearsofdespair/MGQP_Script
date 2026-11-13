namespace MGQP_Script;

public class ProgramInterface
{
    private FarmManager _manager;

    public ProgramInterface()
    {
        _manager = new FarmManager();
    }
    
    public void Run()
    {
        Console.WriteLine("Choose your language: [0] English, [1] Russian");
    }
}