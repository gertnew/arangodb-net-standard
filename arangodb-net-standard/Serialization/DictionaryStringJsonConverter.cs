using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArangoDBNetStandard.Serialization
{

    public class DictionaryStringJsonConverter<TValue> : JsonConverter<Dictionary<string, TValue>>
    {
        private ApiClientSerializationOptions serializationOptions;

        public DictionaryStringJsonConverter(ApiClientSerializationOptions options)
        {
            this.serializationOptions = options;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Dictionary<string, TValue>)
                   || typeToConvert == typeof(Dictionary<string, TValue>);
        }

        public override Dictionary<string, TValue> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            var dictionary = new Dictionary<string, TValue>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JsonTokenType was not PropertyName");
                }

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException("Failed to get property name");
                }

                reader.Read();

                dictionary.Add(propertyName, (TValue)ExtractValue(ref reader, options));
            }

            return dictionary;
        }

        public override void Write(
            Utf8JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options)
        {

            if (!this.serializationOptions.ApplySerializationOptionsToDictionaryValues)
            {
                JsonSerializerOptions newOptions = new JsonSerializerOptions(options);
                newOptions.DictionaryKeyPolicy = null;
                newOptions.PropertyNamingPolicy = null;
                JsonSerializer.Serialize(writer, (IDictionary<string, TValue>)value, newOptions);
            }
            else
            {
                JsonSerializer.Serialize(writer, (IDictionary<string, TValue>)value, options);
            }

        }

        private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out var date))
                    {
                        return date;
                    }
                    return reader.GetString();
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var result))
                    {
                        return result;
                    }
                    return reader.GetDecimal();
                case JsonTokenType.StartObject:
                    return Read(ref reader, null, options);
                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(ExtractValue(ref reader, options));
                    }
                    return list;
                default:
                    throw new JsonException($"'{reader.TokenType}' is not supported");
            }
        }
    }
}