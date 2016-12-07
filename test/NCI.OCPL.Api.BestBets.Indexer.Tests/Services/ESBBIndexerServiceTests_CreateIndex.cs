
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
    public class ESBBIndexerServiceTests_CreateIndex : ESBBIndexerServiceTests__Base
    {
        [Fact]
        public void CreateIndex_Response()
        {
            
        }

        [Fact]
        public void CreateIndex_QueryValidation()
        {
            string aliasName = "TestAlias";

            ESBBIndexerService service = this.GetIndexerService(
                aliasName,
                conn =>
                {
                    conn.RegisterRequestHandlerForType<Nest.CreateIndexResponse>((req, res) =>
                    {
                        res.StatusCode = 200;
                        res.Stream = TestingTools.GetTestFileAsStream("ES_Acknowledged_True_Response.json");
                    });
                }
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
