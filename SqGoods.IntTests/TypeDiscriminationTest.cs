using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqGoods.Models.Filter;
using SqGoods.Services;

namespace SqGoods.IntTests
{
    [TestClass]
    public class TypeDiscriminationTest
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
                            new FilterBoolPredicateModel { AttributeId = Guid.NewGuid(), Value = true },
                            new FilterSelectPredicateModel
                                { AttributeId = Guid.NewGuid(), SelectValueId = Guid.NewGuid() }
                        }
                    },
                    new FilterSetPredicateModel
                    {
                        AttributeId = Guid.NewGuid(),
                        SelectValueIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
                    },
                    new FilterIntBetweenPredicateModel
                    {
                        AttributeId = Guid.NewGuid(),
                        From = 1,
                        To = 8
                    }
                }
            };

            var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            var json1 = JsonSerializer.Serialize(root, jsonSerializerOptions);
            var root2 = JsonSerializer.Deserialize<FilterBoolModel>(json1, jsonSerializerOptions);
            var json2 = JsonSerializer.Serialize(root2, jsonSerializerOptions);

            Assert.AreEqual(json1, json2);
        }

        [TestMethod]
        public void PredicateTest()
        {
            var root = new FilterSelectPredicateModel { AttributeId = Guid.NewGuid(), SelectValueId = Guid.NewGuid() };

            var json1 = JsonSerializer.Serialize(root);
            var root2 = JsonSerializer.Deserialize<FilterBoolModel>(json1);
            var json2 = JsonSerializer.Serialize(root2);

            Assert.AreEqual(json1, json2);
        }
    }
}
