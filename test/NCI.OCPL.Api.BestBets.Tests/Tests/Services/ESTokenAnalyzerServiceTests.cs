using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;

using Xunit;
using Moq;

using Nest;
using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Api.BestBets.Services;
using NCI.OCPL.Api.BestBets.Tests.ESMatchTestData;
using System;
using Microsoft.Extensions.Logging.Testing;
using NCI.OCPL.Api.BestBets.Tests.Util;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace NCI.OCPL.Api.BestBets.Tests
{
    public class ESTokenAnalyzerServiceTests : TestServiceBase
    {

        public static IEnumerable<object[]> GetTokenCountData => new[] {
            new object[] {
                "pancoast",
                "en",
                new object[] {
                    new { token = "pancoast", start_offset = 0, end_offset = 6, type = "<ALPHANUM>", position= 0 },
                },
                1
            },

            new object[] {
                "breast cancer",
                "en",
                new object[] {
                    new { token = "breast", start_offset = 0, end_offset = 6, type = "<ALPHANUM>", position= 0 },
                    new { token = "cancer", start_offset = 7, end_offset = 13, type = "<ALPHANUM>", position= 1 },
                },
                2
            },
            //TODO: Add crazier tests
        };

        [Theory, MemberData("GetTokenCountData")]
        public void GetTokenCount_Responses(
           string searchTerm,           
           string language,
           object[] responseTokens,
           int expectedCount
       )
        {

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();

            conn.RegisterRequestHandlerForType<AnalyzeResponse>(
                (req, res) =>
                {
                    JObject resObject = new JObject();
                    resObject["tokens"] = JArray.FromObject(responseTokens);
                    byte[] byteArray = Encoding.UTF8.GetBytes(resObject.ToString());

                    res.Stream = new MemoryStream(byteArray);
                    res.StatusCode = 200;
                }
            );

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            IOptions<CGBBIndexOptions> config = GetMockConfig();

            ESTokenAnalyzerService service = new ESTokenAnalyzerService(client, config, new NullLogger<ESTokenAnalyzerService>());
            int actualCount = service.GetTokenCount(searchTerm);

            Assert.Equal(expectedCount, actualCount);

        }

    }
}
