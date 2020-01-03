﻿using ArangoDBNetStandard.AuthApi.Models;
using ArangoDBNetStandard.Serialization;
using ArangoDBNetStandard.Transport;
using System.Threading.Tasks;

namespace ArangoDBNetStandard.AuthApi
{
    /// <summary>
    /// ArangoDB authentication endpoints.
    /// </summary>
    public class AuthApiClient : ApiClientBase, IAuthApiClient
    {
        private IApiClientTransport _client;

        public AuthApiClient(IApiClientTransport client)
            : base(new JsonNetApiClientSerialization())
        {
            _client = client;
        }

        public AuthApiClient(IApiClientTransport client, IApiClientSerialization serializer)
            : base(serializer)
        {
            _client = client;
        }

        /// <summary>
        /// Gets a JSON Web Token generated by the ArangoDB server.
        /// </summary>
        /// <param name="username">The username of the user for whom to generate a JWT token.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>Object containing the encoded JWT token value.</returns>
        public async Task<JwtTokenResponse> GetJwtTokenAsync(string username, string password)
        {
            return await GetJwtTokenAsync(new JwtTokenRequestBody
            {
                Username = username,
                Password = password
            });
        }

        /// <summary>
        /// Gets a JSON Web Token generated by the ArangoDB server.
        /// </summary>
        /// <param name="body">Object containing username and password.</param>
        /// <returns>Object containing the encoded JWT token value.</returns>
        public async Task<JwtTokenResponse> GetJwtTokenAsync(JwtTokenRequestBody body)
        {
            byte[] content = GetContent(body, true, false);
            using (var response = await _client.PostAsync("/_open/auth", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return DeserializeJsonFromStream<JwtTokenResponse>(stream);
                }
                throw await GetApiErrorException(response);
            }
        }
    }
}
