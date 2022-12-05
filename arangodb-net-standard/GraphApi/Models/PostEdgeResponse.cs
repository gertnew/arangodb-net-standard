﻿using System.Net;

namespace ArangoDBNetStandard.GraphApi.Models
{
    /// <summary>
    /// Represents a response containing information about the created edge in a graph.
    /// </summary>
    public class PostEdgeResponse<T>
    {
        /// <summary>
        /// Indicates whether an error occurred
        /// </summary>
        /// <remarks>
        /// Note that in cases where an error occurs, the ArangoDBNetStandard
        /// client will throw an <see cref="ApiErrorException"/> rather than
        /// populating this property. A try/catch block should be used instead
        /// for any required error handling.
        /// </remarks>
        public bool Error { get; set; }

        /// <summary>
        /// The HTTP status code.
        /// </summary>
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// The internal attributes for the edge.
        /// </summary>
        public EdgeResult Edge { get; set; }

        /// <summary>
        /// The complete newly written edge document.
        /// Includes all written attributes in <typeparamref name="T" /> and all internal attributes generated by ArangoDB.
        /// Will only be present if <see cref="PostEdgeQuery.ReturnNew"/> is true.
        /// </summary>
        public T New { get; set; }
    }
}
