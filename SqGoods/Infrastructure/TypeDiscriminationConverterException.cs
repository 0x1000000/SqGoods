using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SqGoods.Infrastructure
{
    public class TypeDiscriminationConverter<T> : JsonConverter<T> where T: notnull
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var readerCopy = reader;
            int isTypeAttribute = 0;
            int objectTier = 0;
            do
            {
                if (readerCopy.TokenType == JsonTokenType.StartObject)
                {
                    objectTier++;
                }
                else if (readerCopy.TokenType == JsonTokenType.EndObject)
                {
                    objectTier--;
                }
                else if (objectTier == 1 && isTypeAttribute == 0 && readerCopy.TokenType == JsonTokenType.PropertyName && string.Equals(readerCopy.GetString(),"type", StringComparison.InvariantCultureIgnoreCase))
                {
                    isTypeAttribute++;
                }
                else if (objectTier == 1 && isTypeAttribute == 1)
                {
                    if (readerCopy.TokenType != JsonTokenType.String)
                    {
                        throw new TypeDiscriminationConverterException("String was expected");
                    }

                    var tag = readerCopy.GetString() ?? throw new TypeDiscriminationConverterException("String was expected");
                    var targetType = TypeTagStorage.GetTypeByTag(tag, typeToConvert);

                    return (T?)JsonSerializer.Deserialize(ref reader, targetType, options) ?? throw new TypeDiscriminationConverterException("Could not deserialize");
                }

            } while (readerCopy.Read());

            throw new TypeDiscriminationConverterException("Could not find type tag");
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    public class TypeDiscriminationConverterException : Exception
    {
        public TypeDiscriminationConverterException(string message) : base(message)
        {
        }
    }
}