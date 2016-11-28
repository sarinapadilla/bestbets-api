using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;

using Xunit;
using Moq;

using Nest;
using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Services.BestBets.Services;
using NCI.OCPL.Services.BestBets.Tests.ESMatchTestData;
using System;
using Microsoft.Extensions.Logging.Testing;

namespace NCI.OCPL.Services.BestBets.Tests
{
    public class ESBestBetsMatchServiceTests
    {


        public static IEnumerable<object[]> XmlDeserializingData => new[] {
            new object[] { 
                "pancoast", 
                "en", 
                new PancoastESMatchConnection(), 
                new string[] { "" } 
            },
            //new object[] { 
            //    "breast cancer", 
            //    "en", 
            //    new PancoastESMatchConnection(), 
            //    new string[] { "" } 
            //},
        };


        [Theory, MemberData("XmlDeserializingData")]
        public void GetMatches_Normal(
            string searchTerm, 
            string lang, 
            ESMatchConnection connection, 
            string[] expectedCategories
        )
        {
            //Use real ES client, with mocked connection.

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, connection);            
            
            IElasticClient client = new ElasticClient(connectionSettings);

            ESBestBetsMatchService service = new ESBestBetsMatchService(client, new NullLogger<ESBestBetsMatchService>());

            string[] actualMatches = service.GetMatches(lang, searchTerm);

            Assert.Equal(expectedCategories, actualMatches);
        }



    }
}