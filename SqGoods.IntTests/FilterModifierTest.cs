using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqExpress;
using SqExpress.Syntax.Names;
using SqGoods.Models.Filter;
using SqGoods.Services;

namespace SqGoods.IntTests
{
    [TestClass]
    public class FilterModifierTest
    {
        [TestMethod]
        public void BasicTest()
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


            var (e, _) = root.Accept(FilterBoolModelMapper.Instance);
            Assert.IsNotNull(e);

            var columns = e.SyntaxTree()
                .Descendants()
                .OfType<ExprColumn>()
                .Distinct()
                .Select(c => Guid.TryParse(c.ColumnName.Name, out var guid) ? (Guid?)guid : null)
                .Where(g => g.HasValue)
                .Select(g => g.Value)
                .ToList();
            foreach (var column in columns)
            {
                Console.WriteLine(column);
            }
        }
    }
}