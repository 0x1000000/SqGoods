using System;
using System.Threading.Tasks;
using SqExpress.DataAccess;

namespace SqGoods.DomainLogic.DataAccess
{
    internal interface ISqlConnectionStorage : IDisposable, IAsyncDisposable
    {
        ISqDatabase CreateDatabase();

        Task OpenConnectionAsync();
    }
}