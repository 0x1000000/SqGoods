using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqExpress;
using SqExpress.DataAccess;
using SqExpress.Syntax.Boolean;
using SqGoods.DomainLogic.Models;
using SqGoods.DomainLogic.Repositories.Utils;
using SqGoods.DomainLogic.Tables;

namespace SqGoods.DomainLogic.Repositories
{
    public interface ISgProductRepository
    {
        Task<DataPage<SgProduct>> Find(int offset, int pageSize, Guid? categoryId, ExprBoolean? extraFilter);

        Task Create(IReadOnlyList<SgProduct> data);

        Task Update(IReadOnlyList<SgProduct> data);
    }

    internal class SgProductRepository : ISgProductRepository
    {
        private readonly ISqDatabase _database;

        public SgProductRepository(ISqDatabase database)
        {
            this._database = database;
        }

        public async Task<DataPage<SgProduct>> Find(int offset, int pageSize, Guid? categoryId, ExprBoolean? extraFilter)
        {
            var tblProduct = AllTables.GetProduct();

            ExprBoolean? filter = categoryId.HasValue ? tblProduct.CategoryId == categoryId.Value : null;
 
            var selectBuilder = SqQueryBuilder.Select(SgProduct.GetColumns(tblProduct)).From(tblProduct);

            if (extraFilter != null)
            {
                var (preparedExtraFilter, view) = await FilterProcessor.Prepare(this._database, tblProduct.ProductId, extraFilter);
                filter = filter != null ? filter & preparedExtraFilter : preparedExtraFilter;
                if (view != null)
                {
                    selectBuilder = selectBuilder
                        .InnerJoin(view, on: view.ProductId == tblProduct.ProductId);
                }
            }

            return await selectBuilder
                .Where(filter)
                .OrderBy(tblProduct.Name)
                .OffsetFetch(offset, pageSize).Done()
                .QueryPage(this._database, r => SgProduct.Read(r, tblProduct));
        }

        public async Task Create(IReadOnlyList<SgProduct> data)
        {
            await SqQueryBuilder
                .InsertDataInto(AllTables.GetProduct(), data)
                .MapData(SgProduct.GetMapping)
                .Exec(this._database);
        }

        public async Task Update(IReadOnlyList<SgProduct> data)
        {
            await SqQueryBuilder
                .UpdateData(AllTables.GetProduct(), data)
                .MapDataKeys(SgProduct.GetUpdateKeyMapping)
                .MapData(SgProduct.GetUpdateMapping)
                .Exec(this._database);
        }
    }
}