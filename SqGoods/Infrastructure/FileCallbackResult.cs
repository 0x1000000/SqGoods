using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SqGoods.Infrastructure
{
    public class FileCallbackResult : FileResult
    {
        private readonly Func<Stream, Task> _streamWriter;

        public FileCallbackResult(string contentType, Func<Stream, Task> streamWriter) : base(contentType)
        {
            this._streamWriter = streamWriter;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var executor = new FileCallbackResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
            return executor.ExecuteAsync(context, this);
        }

        private sealed class FileCallbackResultExecutor : FileResultExecutorBase
        {
            public FileCallbackResultExecutor(ILoggerFactory loggerFactory)
                : base(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
            {
            }

            public Task ExecuteAsync(ActionContext context, FileCallbackResult result)
            {
                SetHeadersAndLog(context, result, null, false);
                return result._streamWriter(context.HttpContext.Response.Body);
            }
        }
    }
}