using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SqGoods.Infrastructure
{
    public class TypeTagStorage
    {
        private static readonly ConcurrentDictionary<Type, TypeItem> Storage = new();

        public static Type GetTypeByTag(string tag, Type baseType)
        {
            if (!Get(EnsureBaseType(baseType)).ByTag.TryGetValue(tag, out var result))
            {
                throw new Exception($"Could not find type by tag '{tag}'");
            }
            return result;
        }

        public static string GetTagByType(Type type, Type baseType)
        {
            if (!Get(EnsureBaseType(baseType)).ByType.TryGetValue(type, out var result))
            {
                throw new Exception($"Could not find tag by type '{type.Name}'");
            }
            return result;
        }

        private static TypeItem Get(Type baseType)
        {
            return Storage.GetOrAdd(baseType, Init);

            static TypeItem Init(Type baseType)
            {
                baseType = EnsureBaseType(baseType);
                var knownTypes = baseType.GetCustomAttributes(typeof(KnownTypeAttribute), false);
                if (knownTypes.Length < 1)
                {
                    throw new Exception("There should be at least one known type");
                }

                var result = new TypeItem(new Dictionary<string, Type>(), new Dictionary<Type, string>());

                foreach (KnownTypeAttribute attribute in knownTypes)
                {
                    var type = attribute.Type ?? throw new Exception("No type in known type attribute");

                    var tagAttributes = type.GetCustomAttributes(typeof(TypeTagAttribute), false);
                    if (tagAttributes == null || tagAttributes.Length != 1)
                    {
                        throw new Exception($"There should be exactly one type tag attribute for '{type.Name}'");
                    }

                    var tagAttribute = (TypeTagAttribute)tagAttributes[0];
                    if (!result.ByTag.TryAdd(tagAttribute.TypeTag, type))
                    {
                        throw new Exception($"Type tag '{type}' is used twice");
                    }
                    if (!result.ByType.TryAdd(type, tagAttribute.TypeTag))
                    {
                        throw new Exception($"Duplication of known type '{type}' was found");
                    }
                }

                return result;
            }
        }

        private static Type EnsureBaseType(Type baseType)
        {
            Type? result = null;
            while (typeof(object) != baseType)
            {
                var attributes = baseType.GetCustomAttributes(typeof(JsonConverterAttribute), false);
                var converterType = typeof(TypeDiscriminationConverter<>).MakeGenericType(baseType);
                if (attributes.Any(a => ((JsonConverterAttribute)a).ConverterType == converterType))
                {
                    result = baseType;
                }
                baseType = baseType.BaseType ?? throw new Exception("Could not find JsonConverter attribute");
            }

            return result ?? throw new Exception("Could not find JsonConverter attribute");
        }


        private readonly struct TypeItem
        {
            public readonly Dictionary<string, Type> ByTag;

            public readonly Dictionary<Type, string> ByType;

            public TypeItem(Dictionary<string, Type> byTag, Dictionary<Type, string> byType)
            {
                this.ByTag = byTag;
                this.ByType = byType;
            }
        }
    }
}