using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqExpress.SqlExport;
using SqGoods.Models.Filter;
using SqGoods.Services;

namespace SqGoods.IntTests
{
    [TestClass]
    public class FilterBoolModelMapperTest
    {
        [TestMethod]
        public void ExportTest()
        {
            var root = new FilterAndModel
            {
                Items = new FilterBoolModel[]
                {
                    new FilterOrModel
                    {
                        Items = new FilterBoolModel[]
                        {
                            new FilterBoolPredicateModel { AttributeId = new Guid("7F201A9C-C943-4614-A3C5-15171E348527"), Value = true },
                            new FilterSelectPredicateModel
                            {
                                AttributeId = new Guid("61B884D7-3895-4CF7-91C9-49BFFC4BC26A"),
                                SelectValueId = new Guid("265451DC-8C0D-47D5-AF6E-9812990C1D68")
                            }
                        }
                    },
                    new FilterSetPredicateModel
                    {
                        AttributeId = new Guid("233F310F-2673-4694-86CE-7DE46DEB58CB"),
                        SelectValueIds = new[]
                        {
                            new Guid("A3D8D9D9-BF37-4A19-9598-D150F0F8B01D"),
                            new Guid("66D73621-9E4D-4C3D-B373-5B9D87697CFE")
                        }
                    },
                    new FilterIntBetweenPredicateModel
                    {
                        AttributeId = new Guid("CE71F176-6D59-4D77-A392-CA3211AF741B"),
                        From = 1,
                        To = 8
                    }
                }
            };

            var (expr, error) = root.Accept(FilterBoolModelMapper.Instance);

            Assert.IsNotNull(expr);
            Assert.IsNull(error);

            var expected = "(" +
                           "[7f201a9c-c943-4614-a3c5-15171e348527]=1 " +
                           "OR " +
                           "[61b884d7-3895-4cf7-91c9-49bffc4bc26a]='265451dc-8c0d-47d5-af6e-9812990c1d68'" +
                           ")" +
                           "AND " +
                           "[233f310f-2673-4694-86ce-7de46deb58cb] IN('a3d8d9d9-bf37-4a19-9598-d150f0f8b01d','66d73621-9e4d-4c3d-b373-5b9d87697cfe') " +
                           "AND [ce71f176-6d59-4d77-a392-ca3211af741b]>=1 " +
                           "AND [ce71f176-6d59-4d77-a392-ca3211af741b]<=8";

            Assert.AreEqual(expected, TSqlExporter.Default.ToSql(expr));
        }

        [TestMethod]
        public void ExportTest_ErrorEmptyAnd()
        {
            var root = new FilterAndModel();
            var (expr, error) = root.Accept(FilterBoolModelMapper.Instance);
            Assert.IsNull(expr);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void ExportTest_ErrorEmptyIn()
        {
            var root = new FilterSetPredicateModel{AttributeId = Guid.NewGuid()};
            var (expr, error) = root.Accept(FilterBoolModelMapper.Instance);
            Assert.IsNull(expr);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void ExportTest_ErrorEmptyBetween()
        {
            var root = new FilterIntBetweenPredicateModel { AttributeId = Guid.NewGuid()};
            var (expr, error) = root.Accept(FilterBoolModelMapper.Instance);
            Assert.IsNull(expr);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        public void ExportTest_ErrorEmptyColumn()
        {
            var root = new FilterBoolPredicateModel { Value = true };
            var (expr, error) = root.Accept(FilterBoolModelMapper.Instance);
            Assert.IsNull(expr);
            Assert.IsNotNull(error);
        }
    }
}