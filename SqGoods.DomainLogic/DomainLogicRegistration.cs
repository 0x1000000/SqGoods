using System;
using Microsoft.Extensions.DependencyInjection;
using SqGoods.DomainLogic.DataAccess;

namespace SqGoods.DomainLogic
{
    public static class DomainLogicRegistration
    {
        public static void AddSqGoodsDomainLogic(this IServiceCollection services, Action<DomainLogicOptions> configuration)
        {
            var options = new DomainLogicOptions();
            configuration(options);

            services.AddSingleton(new SqlConnectionStorageFactory(options));
            services.AddSingleton<IDatabaseManager, DatabaseManager>();
            services.AddScoped(r => r.GetService<SqlConnectionStorageFactory>()!.CreateStorage());
            services.AddScoped(r => r.GetService<ISqlConnectionStorage>()!.CreateDatabase());

            //Repositories
            services.AddScoped<IDomainLogic, DomainLogic>();
        }
    }
}