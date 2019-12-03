﻿using System.Net;

namespace ArangoDBNetStandard.GraphApi
{
    public class PutEdgeDefinitionResponse
    {
        public HttpStatusCode Code { get; set; }

        public bool Error { get; set; }

        public GraphResult Graph { get; set; }
    }
}
