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
using Microsoft.Extensions.Logging.Testing;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests
{
    public abstract class ESBBIndexerServiceTests__Base
    {
        /// <summary>
        /// Helper function to get an instance of a ESBBIndexerService Only needing to setup
        /// the connection request handler callbacks. (via connSetupCallback)
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="connSetupCallback"></param>
        /// <returns></returns>
        protected ESBBIndexerService GetIndexerService(string aliasName, Action<ElasticsearchInterceptingConnection> connSetupCallback)
        {
            //Create connection and handle request.
            ElasticsearchInterceptingConnection interceptConnection = new ElasticsearchInterceptingConnection();

            connSetupCallback(interceptConnection);

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
                options.Object,
                new NullLogger<ESBBIndexerService>()
            );

            return service;
        }
    }
}
