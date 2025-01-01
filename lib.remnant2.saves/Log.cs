using Serilog;

namespace lib.remnant2.saves;

// Rudimentary logging facility for debugging and troubleshooting
// We do not need anything more advanced
public class Log
{
    public const string Category = "RemnantLogCategory";
    public const string Compression = "Compression";
    public const string Parser = "Parser";

    private static ILogger? _logger;

    public static ILogger Logger
    {
        get => (_logger ?? Serilog.Log.Logger).ForContext("RemnantLogLibrary", "lib.remnant2.saves");
        set => _logger = value;
    }
}
