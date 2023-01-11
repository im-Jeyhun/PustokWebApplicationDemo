using DemoApplication.Database;

namespace DemoApplication.BackgroundService
{
    public class DeleteExpiredUpUsers : IHostedService, IDisposable
    {
        private ILogger<DeleteExpiredUpUsers> _logger;
        public IServiceProvider Services { get; }

        public DeleteExpiredUpUsers(IServiceProvider service, ILogger<DeleteExpiredUpUsers> logger)
        {
            Services = service;
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            Console.WriteLine($"{nameof(DeleteExpiredUpUsers)} is startet");
            while (!cancellationToken.IsCancellationRequested)
            {
                DeleteUsers();
            }
            return Task.CompletedTask;
        }

        private void DeleteUsers()
        {
            using var dataContext = GetDataBase();

            try
            {

                var tokens =  dataContext.UserActivations.OrderBy(t => t.TokenExpireDate < DateTime.Now).Select(t => t.User).ToList();

                foreach (var deletedUser in tokens)
                {
                    dataContext.Users.Remove(deletedUser);
                }

                dataContext.SaveChanges();

            }
            catch (Exception e)
            {
                _logger.LogWarning($"Something went wrong while {nameof(DeleteExpiredUpUsers)} working");
                throw e;
            }

        }

        public DataContext GetDataBase()
        {
            DataContext dataContext;
            using (IServiceScope scope = Services.CreateScope())
            {
                dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            }
            return dataContext;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(DeleteExpiredUpUsers)} is stoped");

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GetDataBase();
        }
    }
}
