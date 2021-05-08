using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SqGoods.DomainLogic.DataAccess;

namespace SqGoods.Pages
{
    public class DbErrorModel : PageModel
    {
        private readonly IDatabaseManager _databaseManager;

        public DbErrorModel(IDatabaseManager databaseManager)
        {
            this._databaseManager = databaseManager;
            this.DataBaseType = databaseManager.Options.DatabaseType.ToString();
        }

        public string DataBaseType { get; }

        public string MainMessage { get; private set; } = string.Empty;

        public string? ExceptionText { get; private set; }

        public void OnGet()
        {
            var error = this._databaseManager.LastError;

            if (this._databaseManager.IsInitialized)
            {
                this.MainMessage = "Database is Ok";
            }
            else
            {
                this.Response.StatusCode = 500;
                if (error == null)
                {
                    this.MainMessage = "Database is not initialized";
                }
                else
                {
                    this.MainMessage = error.CreateMessage();

                    if (error.Exception != null)
                    {
                        ExceptionText = ExceptionToString(error.Exception);
                    }
                }
            }
        }

        private static string ExceptionToString(Exception e)
        {
            var builder = new StringBuilder();

            Exception? exception = e;

            while (exception != null)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine("----");
                }

                builder.AppendLine(exception.Message);
                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    builder.AppendLine("-Stack Trace-");
                    builder.AppendLine(exception.StackTrace);
                }
                exception = exception.InnerException;
            }

            return builder.ToString();
        }
    }
}
