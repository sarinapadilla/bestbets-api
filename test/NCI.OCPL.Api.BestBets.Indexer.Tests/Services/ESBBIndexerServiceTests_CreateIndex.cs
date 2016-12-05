
using Elasticsearch.Net;
using Nest;

using Xunit;
using Moq;

using System;
using System.Threading.Tasks;

using NCI.OCPL.Utils.Testing;
using NCI.OCPL.Api.BestBets.Indexer.Services;
using Microsoft.Extensions.Options;
using NCI.OCPL.Api.BestBets.Tests.Util;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests
{
    public class ESBBIndexerServiceTests_CreateIndex
    {
        [Fact]
        public void CreateIndex_Response()
        {
            
        }

        [Fact]
        public void CreateIndex_QueryValidation()
        {

            RequestData actualRequest = null;
            string aliasName = "TestAlias";

            //Create connection and handle request.
            ElasticsearchInterceptingConnection interceptConnection = new ElasticsearchInterceptingConnection();            
            interceptConnection.RegisterRequestHandlerForType<Nest.CreateIndexResponse>((req, res) =>
            {
                res.StatusCode = 200;
                res.Stream = TestingTools.GetTestFileAsStream("ES_Acknowledged_True_Response.json");
            });

            Mock<IOptions<ESBBIndexerServiceOptions>> options = new Mock<IOptions<ESBBIndexerServiceOptions>>();
            options
                .SetupGet(o => o.Value)
                .Returns(new ESBBIndexerServiceOptions()
                {
                    AliasName = aliasName
                });
                
            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, interceptConnection);

            IElasticClient client = new ElasticClient(connectionSettings);

            ESBBIndexerService service = new ESBBIndexerService(
                client,
                options.Object
            );

            string currTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            string actualIndexName = service.CreateTimeStampedIndex();
            string actualTimeStamp = actualIndexName.Replace(aliasName, "");

            long currAsLong = long.Parse(currTimeStamp);
            long actualAsLong = long.Parse(actualTimeStamp);

            //Give it up to a 5 sec difference.
            Assert.InRange(actualAsLong, currAsLong - 5, currAsLong);
        }
    }
}
