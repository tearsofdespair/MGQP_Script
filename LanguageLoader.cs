using System.Text.Json;

namespace MGQP_Script;

public class LanguageLoader
{
    public LanguageLoader(){ }

    public string[] LoadLanguage(string name)
    {
        string jsonString = File.ReadAllText(@"D:\myprogs\C#\MGQP_Script\MGQP_Script\Data\languages.json");
        
        var allLanguages = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonString);
        
        return allLanguages[name];
    }
}