using Serilog;
namespace WorkerServiceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 作为 Windows Service 运行时，默认的当前工作目录是 C:\WINDOWS\system32，会导致找不到配置文件，
            // 所以需要添加下面一行，指定当前工作目录为应用程序所在的实际目录。
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();

            // 全局共享的日志记录器
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                var separator = new string('-', 30);
                Log.Information($"{separator} Starting host {separator} ");
                IHost host = Host.CreateDefaultBuilder(args)
                    .UseWindowsService()
                    .ConfigureServices(services =>
                    {
                        services.AddHostedService<Worker>();
                    })
                    .UseSerilog()
                    .Build();

                host.Run();
                Log.Information($"{separator} Exit host {separator} ");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");

            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}