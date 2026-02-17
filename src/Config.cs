using CsToml;

namespace RitoClient;

[TomlSerializedObject(NamingConvention = TomlNamingConvention.None)]
sealed partial class Config
{
    public static Config I { get; } = new();
    static string _path = Path.Join(BaseDir, "config.toml");

    public static string BaseDir => Module.ThisModuleDir;

    public static string UserDir => Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RitoClientData");

    public static string PreloadDir
        => !string.IsNullOrEmpty(I.preload_dir)
        ? Path.GetFullPath(I.preload_dir, BaseDir)
        : Path.Join(UserDir, "preload");

    static Config()
    {
        if (File.Exists(_path))
        {
            try
            {
                using var fs = File.OpenRead(_path);
                I = CsTomlSerializer.Deserialize<Config>(fs);

                return;
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to load config, using defaults. {0}", ex.Message);
            }
        }
    }

    public static void Load()
    {
    }

    public static void Save()
    {
        var userDir = UserDir;
        if (!Directory.Exists(userDir))
            Directory.CreateDirectory(userDir);

        try
        {
            using var fs = File.OpenWrite(_path);
            CsTomlSerializer.Serialize(fs, I);
        }
        catch (Exception ex)
        {
            Logger.Debug("Failed to save config. Exception: {0}", ex);
        }
    }

    [TomlValueOnSerialized(NullHandling = TomlNullHandling.Ignore)]
    public string? preload_dir { get; set; }

    [TomlValueOnSerialized]
    public bool potato_mode { get; set; } = false;

    [TomlValueOnSerialized]
    public bool disable_sentry { get; set; } = false;

    [TomlValueOnSerialized(NullHandling = TomlNullHandling.Ignore)]
    public string? theme_name { get; set; }
}