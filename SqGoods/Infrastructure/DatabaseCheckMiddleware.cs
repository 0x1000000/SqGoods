using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SqGoods.DomainLogic.DataAccess;

namespace SqGoods.Infrastructure
{
    public class DatabaseCheckMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var m =  (IDatabaseManager?)context.RequestServices.GetService(typeof(IDatabaseManager)) 
                     ?? throw new Exception($"'{nameof(IDatabaseManager)}' should be injected");

            if (!m.IsInitialized)
            {
                var error = m.LastError;

                if (error != null && error.ErrorCode == InitializationError.InitializationErrorCode.Connection)
                {
                    var cfg = (IConfiguration?)context.RequestServices.GetService(typeof(IConfiguration))
                              ?? throw new Exception($"'{nameof(IConfiguration)}' should be injected");

                    await m.Initialize(cfg.GetValue<string>("ConnectionString"));
                }

                if (!m.IsInitialized)
                {
                    if (!context.Request.Path.StartsWithSegments("/DbError", StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.Response.Redirect("/DbError");
                        return;
                    }
                }
            }
            await next(context);
        }
    }
}