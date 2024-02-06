namespace WorkerServiceDemo
{
    public class Worker : BackgroundService
    {
        /// <summary>
        /// ״̬��0-Ĭ��״̬��1-������ɹر�ǰ�ı�Ҫ������2-����ִ�� StopAsync
        /// </summary>
        private volatile int _status = 0; //״̬
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<Worker> _logger;

        public Worker(IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // ����ʵ��ʵ�ʵ�ҵ���߼�
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                        await SomeMethodThatDoesTheWork(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Global exception occurred. Will resume in a moment.");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            finally
            {
                _logger.LogWarning("My worker service shut down.");
            }
        }
        private async Task SomeMethodThatDoesTheWork(CancellationToken cancellationToken)
        {
            string msg = _status switch
            {
                1 => "������ɹر�ǰ�ı�Ҫ��������",
                2 => "��װ������ͷ���ing���� ��ʵ��ȥϴ������",
                _ => "�Ұ���������ͷ���ing����"
            };

            _logger.LogInformation(msg);
            await Task.CompletedTask;
        }

        /// <summary>
        /// �ر�ǰ��Ҫ��ɵĹ���
        /// </summary>
        private void GetOffWork()
        {
            _status = 1;

            _logger.LogInformation("̫���ˣ��°�ʱ�䵽��output from ApplicationStopping.Register Action at: {time}", DateTimeOffset.Now);

            _logger.LogDebug("��ʼ����ر�ǰ������ɵĹ��� at: {time}", DateTimeOffset.Now);

            _logger.LogInformation("��⣬��һ������ bug ��Ҫ�°�ǰ��ɣ�����");

            _logger.LogInformation("���������Ұ��Ӱ࣬��Ҫ�ٸ� 20 �룬Wait 1 ");

            Task.Delay(TimeSpan.FromSeconds(20)).Wait();

            _logger.LogInformation("���������������Ұ��Ӱ࣬��Ҫ�ٸ� 1 ���ӣ�Wait 2 ");

            Task.Delay(TimeSpan.FromMinutes(1)).Wait();

            _logger.LogInformation("�����������������ں��ˣ������°��ˣ�");

            _logger.LogDebug("�ر�ǰ������ɵĹ���������� at: {time}", DateTimeOffset.Now);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // ע��Ӧ��ֹͣǰ��Ҫ��ɵĲ���
            _hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                GetOffWork();
            });
            _logger.LogInformation("�ϰ��ˣ����Ǿ����ӵ�һ�죬output from StartAsync");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _status = 2;

            _logger.LogInformation("׼���°��ˣ�output from StopAsync at: {time}", DateTimeOffset.Now);

            _logger.LogInformation("ȥϴϴ�豭�ȡ���", DateTimeOffset.Now);
            Task.Delay(30_000).Wait();
            _logger.LogInformation("�豭ϴ���ˡ�", DateTimeOffset.Now);

            _logger.LogInformation("�°�� ^_^", DateTimeOffset.Now);

            return base.StopAsync(cancellationToken);
        }
    }
}
