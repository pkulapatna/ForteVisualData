using Serilog.Events;
using Serilog;
using System;

namespace AppServices
{
    public  class ClsSerilog
    {
        public const int INFO = 0;
        public const int WARNING = 1;
        public const int ERROR = 2;
        public const int FATEL = 3;
        public const int DEBUG = 4;

        public ClsSerilog()
        {

            Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                  .Enrich.FromLogContext()
                  .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                  .Enrich.FromLogContext()
                  .WriteTo.File($"C:\\ForteLog\\ASCIILog\\ForteVisualData_.Log", rollingInterval: RollingInterval.Month)
                  .CreateLogger();
        }

        public static void LogMessage(int logidx, string strMessage)
        {
            try
            {
                switch (logidx)
                {
                    case INFO:
                        Log.Information(strMessage);
                        break;
                    case WARNING:
                        Log.Warning(strMessage);
                        break;
                    case ERROR:
                        Log.Error(strMessage);
                        break;
                    case FATEL:
                        Log.Fatal(strMessage);
                        break;
                    case DEBUG:
                        Log.Debug(strMessage);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "LOG ERROR");
            }
        }

        public static void CloseLogger()
        {
            Log.Information("Closed Serilog - Flush Log");
            Log.CloseAndFlush();
        }
    }
}
