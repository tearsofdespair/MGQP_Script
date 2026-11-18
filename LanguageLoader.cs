using System.Reflection;
using System.Text.Json;

namespace MGQP_Script;

public class LanguageLoader
{
    public LanguageLoader(){ }

    public string[] LoadLanguage(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        // Имя ресурса обычно имеет формат: <RootNamespace>.<Folder>.<FileName>
        string resourceName = "MGQP_Script.Data.languages.json";

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new FileNotFoundException($"Внедрённый ресурс '{resourceName}' не найден.");
        }

        using var reader = new StreamReader(stream);
        string jsonString = reader.ReadToEnd();

        var allLanguages = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonString);
        return allLanguages?[name] ?? Array.Empty<string>();
    }
}