namespace RitoClient;

static class RiotService
{
    public static int AppPort { get; }
    public static string? AuthToken { get; }

    static RiotService()
    {
        // --app-port=12345 --remoting-auth-token=abcdefg
        const string prefixPort = "--app-port=";
        const string prefixToken = "--remoting-auth-token=";

        var env = Environment.CommandLine!;
        var parts = env.Split(' ');

        foreach (var part in parts)
        {
            if (part.StartsWith(prefixPort))
            {
                var portStr = part.Substring(prefixPort.Length);
                if (int.TryParse(portStr, out var port))
                {
                    AppPort = port;
                }
            }
            else if (part.StartsWith(prefixToken))
            {
                AuthToken = part.Substring(prefixToken.Length);
            }

            if (AppPort != 0 && AuthToken != null)
                break;
        }
    }
}
