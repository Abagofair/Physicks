using Microsoft.Extensions.Configuration;

namespace GameUtilities.Options;

public class GameOptions
{
    public Graphics Graphics { get; set; }

    public static GameOptions Load(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(fileName).Build();

        return config.Get<GameOptions>();
    }
}