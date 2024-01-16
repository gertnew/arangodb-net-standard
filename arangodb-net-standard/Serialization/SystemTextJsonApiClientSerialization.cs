using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArangoDBNetStandard.Serialization
{
    /// <summary>
    /// Implements a <see cref="IApiClientSerialization"/> that uses Json.NET.
    /// </summary>
    public class SystemTextJsonApiClientSerialization : ApiClientSerialization
    {
        public JsonSerializerOptions DefaultJsonSerializerOptions { get; private set; }

        public SystemTextJsonApiClientSerialization(JsonSerializerOptions defaultJsonSerializerOption = null)
        {
            if (defaultJsonSerializerOption != null)
            {
                DefaultJsonSerializerOptions = new JsonSerializerOptions(defaultJsonSerializerOption);
            }
            else
            {
                DefaultJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            }

        }


        private JsonSerializerOptions GetJsonSerializerOptions(ApiClientSerializationOptions options = null)
        {
            if (options == null)
            {
                options = DefaultOptions;
            }

            JsonSerializerOptions serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            if (DefaultJsonSerializerOptions != null)
            {
                serializerOptions = new JsonSerializerOptions(DefaultJsonSerializerOptions);
            }

            serializerOptions.Converters.Add(new DictionaryStringJsonConverter<object>(options));

            if (options.IgnoreMissingMember)
            {
                serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            }
            else
            {
                serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            }

            if (options.IgnoreNullValues)
            {
                serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }
            else
            {
                serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            }

            if (options.UseCamelCasePropertyNames)
            {
                serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //serializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            }
            else
            {
                serializerOptions.PropertyNamingPolicy = null;
                serializerOptions.DictionaryKeyPolicy = null;
            }

            if (options.UseStringEnumConversion)
            {
                serializerOptions.Converters.Add(new JsonStringEnumConverter());
            }

            return serializerOptions;
        }

        /// <summary>
        /// Deserializes the JSON structure contained by the specified stream
        /// into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The stream containing the JSON structure to deserialize.</param>
        /// <returns></returns>
        public override T DeserializeFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default(T);
            }

            using (var sr = new StreamReader(stream))
            {
                T result = JsonSerializer.Deserialize<T>(stream, GetJsonSerializerOptions());
                return result;
            }
        }

        /// <summary>
        /// Asynchronously deserializes the data structure contained by the specified stream
        /// into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="stream">The stream containing the JSON structure to deserialize.</param>
        /// <returns></returns>
        public override async Task<T> DeserializeFromStreamAsync<T>(Stream stream)
        {
            T result = DeserializeFromStream<T>(stream);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="serializationOptions">The serialization options. When the value is null the
        /// the serialization options should be provided by the serializer, otherwise the given options should be used.</param>
        /// <returns></returns>

        public override byte[] Serialize<T>(T item, ApiClientSerializationOptions serializationOptions)
        {
            string json = SerializeToString(item, serializationOptions);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Asynchronously serializes the specified object to a sequence of bytes,
        /// following the provided rules for camel case property name and null value handling.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="item">The object to serialize.</param>
        /// <param name="serializationOptions">The serialization options. When the value is null the
        /// the serialization options should be provided by the serializer, otherwise the given options should be used.</param>
        /// <returns></returns>
        public override async Task<byte[]> SerializeAsync<T>(T item, ApiClientSerializationOptions serializationOptions)
        {
            var result = Serialize<T>(item, serializationOptions);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Serializes an object to JSON string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="serializationOptions">The serialization options. When the value is null the
        /// the serialization options should be provided by the serializer, otherwise the given options should be used.</param>
        /// <returns></returns>

        public override string SerializeToString<T>(T item, ApiClientSerializationOptions serializationOptions)
        {
            string json = JsonSerializer.Serialize<T>(item, GetJsonSerializerOptions(serializationOptions));

            return json;
        }

        /// <summary>
        /// Asynchronously serializes the specified object to a JSON string,
        /// following the provided rules for camel case property name and null value handling.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="item">The object to serialize.</param>
        /// <param name="serializationOptions">The serialization options. When the value is null the
        /// the serialization options should be provided by the serializer, otherwise the given options should be used.</param>
        /// <returns></returns>
        public override async Task<string> SerializeToStringAsync<T>(T item, ApiClientSerializationOptions serializationOptions)
        {
            var result = SerializeToString(item, serializationOptions);
            return await Task.FromResult(result);
        }
    }
}
