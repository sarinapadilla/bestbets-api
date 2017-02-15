using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;

using Xunit;
using Moq;

using Nest;
using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Api.BestBets.Services;
using NCI.OCPL.Api.BestBets.Tests.ESHealthTestData;
using NCI.OCPL.Api.BestBets.Tests.ESMatchTestData;
using System;
using Microsoft.Extensions.Logging.Testing;

namespace NCI.OCPL.Api.BestBets.Tests
{
    public class ESBestBetsMatchServiceTests : TestServiceBase
    {


        public static IEnumerable<object[]> GetMatchesData => new[] {
            // "pancoast" is a simple test as it only has 1 hit, 1 word, and 1 BB category.
            new object[] { 
                "pancoast", 
                "en", 
                new ESMatchConnection("pancoast"), 
                new ESMatchTokenizerConnection("pancoast"),
                new string[] { "36012" } 
            },
            // "breast cancer" is more complicated, it has 1 hit, 2 words, and the BB category
            // it matches is on page 2.  It also has a ton of negations for breast.
            new object[] { 
                "breast cancer", 
                "en", 
                new ESMatchConnection("breastcancer"),
                new ESMatchTokenizerConnection("breastcancer"),
                new string[] { "36408" } 
            },
            // "breast cancer treatment" is more complicated, it has 1 hit, 3 words, and no results for last page.
            // It also has a ton of negations for various combinations.
            new object[] {
                "breast cancer treatment",
                "en",
                new ESMatchConnection("breastcancertreatment"),
                new ESMatchTokenizerConnection("breastcancertreatment"),
                new string[] { "36408" }
            },
            // "seer stat" is a negated exact match test.  SEER should not be returned
            new object[] {
                "seer stat",
                "en",
                new ESMatchConnection("seerstat"),
                new ESMatchTokenizerConnection("seerstat"),
                new string[] { }
            },
            // "seer stat fact sheet" is a test to make sure the "seer stat" exact match is not hit because
            // we are not exactly matching the phrase "seet stat". Those search terms also match seer.
            new object[] {
                "seer stat fact sheet",
                "en",
                new ESMatchConnection("seerstatfactsheet"),
                new ESMatchTokenizerConnection("seerstatfactsheet"),
                new string[] { "36681" }
            },
        };


        [Theory, MemberData("GetMatchesData")]
        public void GetMatches_Normal(
            string searchTerm, 
            string lang, 
            ESMatchConnection connection, 
            ESMatchTokenizerConnection tokenizerConn,
            string[] expectedCategories
        )
        {
            //Use real ES client, with mocked connection.


            ESTokenAnalyzerService tokenService = GetTokenizerService(tokenizerConn);
            ESBestBetsMatchService service = GetMatchService(tokenService, connection);

            string[] actualMatches = service.GetMatches(lang, searchTerm);

            Assert.Equal(expectedCategories, actualMatches);
        }

        [Theory]
        [InlineData("green")]
        [InlineData("yellow")]
        public void HealthStatus_Healthy(string datafile)
        {
            ESHealthConnection connection = new ESHealthConnection(datafile);
            ESHealthTokenizerConnection tokenizerConn = new ESHealthTokenizerConnection(datafile);

            ESTokenAnalyzerService tokenService = GetTokenizerService(tokenizerConn);
            ESBestBetsMatchService service = GetMatchService(tokenService, connection);

            bool isHealthy = service.IsHealthy;

            Assert.True(isHealthy);
        }

        [Theory]
        [InlineData("red")]
        [InlineData("unexpected")]   // i.e. "Unexpected color"
        public void HealthStatus_Unhealthy(string datafile)
        {
            ESHealthConnection connection = new ESHealthConnection(datafile);
            ESHealthTokenizerConnection tokenizerConn = new ESHealthTokenizerConnection(datafile);

            ESTokenAnalyzerService tokenService = GetTokenizerService(tokenizerConn);
            ESBestBetsMatchService service = GetMatchService(tokenService, connection);

            bool isHealthy = service.IsHealthy;

            Assert.False(isHealthy);
        }

        private ESTokenAnalyzerService GetTokenizerService(IConnection connection)
        {
            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, connection);
            IElasticClient client = new ElasticClient(connectionSettings);

            IOptions<CGBBIndexOptions> config = GetMockConfig();

            return new ESTokenAnalyzerService(client, config, new NullLogger<ESTokenAnalyzerService>());
        }

        private ESBestBetsMatchService GetMatchService(ESTokenAnalyzerService tokenService, IConnection connection)
        {
            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, connection);
            IElasticClient client = new ElasticClient(connectionSettings);

            IOptions<CGBBIndexOptions> config = GetMockConfig();

            return new ESBestBetsMatchService(client, tokenService, config, new NullLogger<ESBestBetsMatchService>());
        }

    }
}