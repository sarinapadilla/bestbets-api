using System;
using System.Collections.Generic;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Testing;

using Nest;
using Elasticsearch.Net;
using Xunit;

using NCI.OCPL.Api.BestBets.Services;
using NCI.OCPL.Api.BestBets.Tests.ESHealthTestData;
using NCI.OCPL.Api.BestBets.Tests.ESMatchTestData;

namespace NCI.OCPL.Api.BestBets.Tests
{
    public class ESBestBetsHealthServiceTests : TestServiceBase
    {
        [Theory]
        [InlineData("green")]
        [InlineData("yellow")]
        public async void HealthStatus_Healthy(string datafile)
        {
            ESHealthConnection connection = new ESHealthConnection(datafile);

            ESBestBetsHealthService service = GetHealthService(connection);

            bool isHealthy = await service.IsHealthy();

            Assert.True(isHealthy);
        }

        [Theory]
        [InlineData("red")]
        [InlineData("unexpected")]   // i.e. "Unexpected color"
        public async void HealthStatus_Unhealthy(string datafile)
        {
            ESHealthConnection connection = new ESHealthConnection(datafile);
            ESBestBetsHealthService service = GetHealthService(connection);

            bool isHealthy = await service.IsHealthy();

            Assert.False(isHealthy);
        }

        /// <summary>
        /// Test for when the ES healthcheck returns a non-200 response code
        /// (response.IsValid comes back as false).
        /// </summary>
        /// <param name="httpStatus"></param>
        [Theory]
        [InlineData(404)]
        [InlineData(500)]
        public async void HealthStatus_InvalidResponse(int httpStatus)
        {
            ESErrorConnection connection = new ESErrorConnection(httpStatus);
            ESBestBetsHealthService service = GetHealthService(connection);

            bool res = await service.IsHealthy();
            Assert.False(res);

        }

        private ESBestBetsHealthService GetHealthService(IConnection connection)
        {
            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, connection);
            IElasticClient client = new ElasticClient(connectionSettings);

            IOptions<CGBBIndexOptions> config = GetMockConfig();

            return new ESBestBetsHealthService(client, config, new NullLogger<ESBestBetsHealthService>());
        }
    }
}