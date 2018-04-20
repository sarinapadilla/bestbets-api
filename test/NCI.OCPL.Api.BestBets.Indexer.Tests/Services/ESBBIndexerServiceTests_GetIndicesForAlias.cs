
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
    public class ESBBIndexerServiceTests_GetIndicesForAlias : ESBBIndexerServiceTests__Base
    {
        /// <summary>
        /// Tests what happens is NoAlias exists.
        /// </summary>
        [Fact]
        public void NoAlias()
        {
            string aliasName = "chicken";
            string actualPath = string.Empty;
            object actualRequestOptions = null;

            ESBBIndexerService service = this.GetIndexerService(
                aliasName,
                conn =>
                {

                    //This is the handler for AliasExists calls
                    conn.RegisterRequestHandlerForType<Nest.ExistsResponse>((req, res) =>
                    {
                        //Store off Request
                        actualPath = req.Path;
                        actualRequestOptions = conn.GetRequestPost(req);

                        //Setup Response
                        res.StatusCode = 404;
                    });
                }
            );

            string[] actualIndices = service.GetIndicesForAlias();

            //Make sure the Request is the expected request
            Assert.Equal("_alias/" + aliasName, actualPath);
            Assert.Null(actualRequestOptions);

            //Make sure the response is the same.
            Assert.Equal(new string[] { }, actualIndices);
        }

        /// <summary>
        /// Tests correct behavior with single index for alias.
        /// This is what should normally happen with our code.
        /// </summary>
        [Fact]
        public void SingleAlias()
        {
            string aliasName = "bestbets";
            string actualPath = string.Empty;
            object actualRequestOptions = null;

            ESBBIndexerService service = this.GetIndexerService(
                aliasName,
                conn =>
                {
                    //This is the handler for AliasExists calls
                    conn.RegisterRequestHandlerForType<Nest.ExistsResponse>((req, res) =>
                    {
                        //Just need to return 200 to say alias does exist
                        res.StatusCode = 200;                                                                     
                        res.Stream = null;
                        res.Exception = null;
                    });

                    conn.RegisterRequestHandlerForType<GetAliasResponse>((req, res) =>
                    {
                        //Store off Request
                        actualPath = req.Path;
                        actualRequestOptions = conn.GetRequestPost(req);
                        
                        //Setup Response
                        res.StatusCode = 200;
                        res.Stream = TestingTools.GetTestFileAsStream("GetIndicesForAlias_SingleItemResponse.json");
                    });                    
                }
            );

            string[] actualIndices = service.GetIndicesForAlias();

            //Make sure the Request is the expected request
            Assert.Equal(aliasName + "/_alias", actualPath);
            Assert.Null(actualRequestOptions);

            //Make sure the response is the same.
            Assert.Equal(new string[] { "bestbets20161202152737" }, actualIndices);
        }

        /// <summary>
        /// Tests Correct Behavior with multiple indices for an alias
        /// </summary>
        [Fact]
        public void MultiAlias()
        {
            string aliasName = "bestbets";
            string actualPath = string.Empty;
            object actualRequestOptions = null;

            ESBBIndexerService service = this.GetIndexerService(
                aliasName,
                conn =>
                {
                    //This is the handler for AliasExists calls
                    conn.RegisterRequestHandlerForType<Nest.ExistsResponse>((req, res) =>
                    {
                        //Just need to return 200 to say alias does exist
                        res.StatusCode = 200;
                    });

                    //This is the handler for GetAlias calls
                    conn.RegisterRequestHandlerForType<GetAliasResponse>((req, res) =>
                    {
                        //Store off Request
                        actualPath = req.Path;
                        actualRequestOptions = conn.GetRequestPost(req);

                        //Setup Response
                        res.StatusCode = 200;
                        res.Stream = TestingTools.GetTestFileAsStream("GetIndicesForAlias_MultiItemResponse.json");
                    });
                }
            );

            string[] actualIndices = service.GetIndicesForAlias();

            //Make sure the Request is the expected request
            Assert.Equal(aliasName + "/_alias", actualPath);
            Assert.Null(actualRequestOptions);

            //Make sure the response is the same.
            Assert.Equal(new string[] { "bestbets20161202152737", "bestbets20161201165226" }, actualIndices);
        }
    }
}
