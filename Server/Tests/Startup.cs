using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace xunittests;
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // configuration må ikke tilføjes som parameter, derfor oprettes det her lokalt.
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Program.ConfigureServices(services, configuration);
        services.RemoveAll(typeof(MyDbContext));

        services.AddScoped<MyDbContext>(factory =>
        {
            var postgreSqlContainer = new PostgreSqlBuilder().Build();
            postgreSqlContainer.StartAsync().GetAwaiter().GetResult();
            var connectionString = postgreSqlContainer.GetConnectionString();
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseNpgsql(connectionString)
                .Options;
            
            var ctx = new MyDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        });
    }
}