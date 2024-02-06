using Serilog;
namespace WorkerServiceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ��Ϊ Windows Service ����ʱ��Ĭ�ϵĵ�ǰ����Ŀ¼�� C:\WINDOWS\system32���ᵼ���Ҳ��������ļ���
            // ������Ҫ�������һ�У�ָ����ǰ����Ŀ¼ΪӦ�ó������ڵ�ʵ��Ŀ¼��
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();

            // ȫ�ֹ������־��¼��
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