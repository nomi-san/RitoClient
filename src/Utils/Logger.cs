using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using ZLogger;

namespace RitoClient;

static partial class Logger
{
    private static readonly ILoggerFactory s_factory;
    private static readonly ILogger s_logger;

    static Logger()
    {
#if WINDOWS && DEBUG
        WindowsConsole.OpenConsole();
#endif

        int pid = Environment.ProcessId;
        var dir = Path.Combine(Module.ThisModuleDir, "logs");
        var path = Path.Combine(dir, string.Format("{0:yyyy-MM-ddTHH-mm-ss}_p{1}.log", DateTime.Now, pid));

        s_factory = LoggerFactory.Create(builder =>
        {
            builder
                .ClearProviders()
                .AddZLoggerFile(path, options =>
                {
                    options.UsePlainTextFormatter(fmt =>
                    {
                        fmt.SetPrefixFormatter($"{0:yyyy-MM-dd HH:mm:ss.fff} [{1:short}] ",
                            (in MessageTemplate template, in LogInfo info) =>
                            {
                                template.Format(info.Timestamp, info.LogLevel);
                            });
                    });
                })
#if DEBUG
                .AddZLoggerConsole(options =>
                {
                    options.UsePlainTextFormatter(fmt =>
                    {
                        fmt.SetPrefixFormatter($"\u001b[49;7m {2:HH:mm:ss.fff} \u001b[0m{0} {3:short} {1} ",
                            (in MessageTemplate template, in LogInfo info) =>
                            {
                                var cc = info.LogLevel switch
                                {
                                    LogLevel.Error => "\x1b[101;30m",
                                    LogLevel.Information => "\x1b[102;30m",
                                    LogLevel.Warning => "\x1b[103;30m",
                                    LogLevel.Debug => "\x1b[104;30m",
                                    _ => ""
                                };

                                template.Format(cc, cc == "" ? "" : "\x1b[0m", info.Timestamp, info.LogLevel);
                            });
                    });
                })
#endif
                .SetMinimumLevel(LogLevel.Debug);
        });

        s_logger = s_factory.CreateLogger("RITO");
    }

    public static void Info(string fmt, params object[] args)
    {
        s_logger.LogInformation(fmt, args);
    }

    public static void Debug(string fmt, params object[] args)
    {
        s_logger.LogDebug(fmt, args);
    }

    public static void Warn(string fmt, params object[] args)
    {
        s_logger.LogWarning(fmt, args);
    }

    public static void Error(string message, Exception? ex = null)
    {
        if (ex is null)
            s_logger.LogError(message);
        else
            s_logger.LogError(ex, message);
    }

#if WINDOWS && DEBUG
    static partial class WindowsConsole
    {
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;
        const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [LibraryImport("kernel32")]
        private static partial int AllocConsole();

        [LibraryImport("kernel32")]
        private static partial IntPtr GetStdHandle(int nStdHandle);

        [LibraryImport("kernel32")]
        private static partial void SetStdHandle(int nStdHandle, IntPtr handle);

        [LibraryImport("kernel32")]
        private static partial int GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [LibraryImport("kernel32")]
        private static partial int SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        public static void OpenConsole()
        {
            AllocConsole();

            var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            var safeOutHandle = new SafeFileHandle(stdOutHandle, ownsHandle: false);
            var fileStreamOut = new FileStream(safeOutHandle, FileAccess.Write);
            var writerOut = new StreamWriter(fileStreamOut) { AutoFlush = true };
            Console.SetOut(writerOut);

            var stdErrHandle = GetStdHandle(STD_ERROR_HANDLE);
            var safeErrHandle = new SafeFileHandle(stdErrHandle, ownsHandle: false);
            var fileStreamErr = new FileStream(safeErrHandle, FileAccess.Write);
            var writerErr = new StreamWriter(fileStreamErr) { AutoFlush = true };
            Console.SetError(writerErr);

            GetConsoleMode(stdOutHandle, out int mode);
            SetConsoleMode(stdOutHandle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
    }
#endif
}