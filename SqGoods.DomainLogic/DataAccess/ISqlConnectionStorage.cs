using System;
using SqExpress.DataAccess;

namespace SqGoods.DomainLogic.DataAccess
{
    internal interface ISqlConnectionStorage : IDisposable
    {
        ISqDatabase CreateDatabase();

        void OpenConnection();
    }
}