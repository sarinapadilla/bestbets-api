
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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests
{
    public class ESBBIndexerServiceTests_DeleteOldIndices : ESBBIndexerServiceTests__Base
    {


        public static IEnumerable<object[]> DeleteData => new[] {
            //Test with old indices to be deleted.
            new object[] {
                "ES_INDEX_SETTINGS_WITHOLD.json", //The Response for get settings
                7,
                String.Join("%2C", GetExpectedIndices()), //Delete Path
                GetExpectedIndices() //Deleted indices
            },
            //Test with no indices
            new object[] {
                "ES_Empty_Response.json", //The Response for get settings
                7,
                string.Empty, //Delete Path
                new string[] { } //Deleted indices
            },
            //Test with no old indices
            new object[] {
                "ES_INDEX_SETTINGS_WITHOUTOLD.json", //The Response for get settings
                7,
                string.Empty, //Delete Path
                new string[] { } //Deleted indices
            },
            //Test with 10 candidates for deletion, with min of 10, so no deletes
            new object[] {
                "ES_INDEX_SETTINGS_FORMINKEEP.json", //The Response for get settings
                10,
                string.Empty, //Delete Path
                new string[] { } //Deleted indices
            },
            //Test with 10 candidates for deletion, with min of 7, so only 3 deletes
            new object[] {
                "ES_INDEX_SETTINGS_FORMINKEEP.json", //The Response for get settings
                7,
                String.Join("%2C", "bestbets20170129011006", "bestbets20170128161004", "bestbets20170128061005"), //Delete Path
                new string[] { "bestbets20170129011006", "bestbets20170128161004", "bestbets20170128061005" } //Deleted indices
            }
        };

        /// <summary>
        /// Tests the deletion function when there are old indices to delete
        /// </summary>
        [Theory, MemberData("DeleteData")]
        public void TestDelete_WithOutError(string responseFile, int minIndices, string expectedDeletePath, string[] expectedDeletedIndices)
        {
            string aliasName = "bestbets";

            string actualGetSettingsPath = string.Empty;
            string actualDeletePath = string.Empty;
            JObject actualGetSettingsRequestOptions = null;
            JObject actualDeleteRequestOptions = null;

            ESBBIndexerService service = this.GetIndexerService(
            aliasName,
            conn =>
            {
                //This is the handler for getting the listing of indices/creationdates
                conn.RegisterRequestHandlerForType<Nest.GetIndexSettingsResponse>((req, res) =>
                {
                    //Store off Request
                    actualGetSettingsPath = req.Path;
                    actualGetSettingsRequestOptions = conn.GetRequestPost(req);

                    //Setup Response
                    res.StatusCode = 200;
                    res.Stream = TestingTools.GetTestFileAsStream(responseFile);
                });

                //Handle Delete Index response here
                conn.RegisterRequestHandlerForType<Nest.DeleteIndexResponse>((req, res) =>
                {
                    //Store off Request
                    actualDeletePath = req.Path;
                    actualDeleteRequestOptions = conn.GetRequestPost(req);
                    //Could also test the method...

                    //Setup Response
                    res.StatusCode = 200;
                    res.Stream = TestingTools.GetTestFileAsStream("ES_Acknowledged_True_Response.json");
                });
            });

            //The data in our test file only goes to 2/16, so let's set the date to 2/10
            DateTime olderThan = new DateTime(2017, 2, 10);

            var indices = service.DeleteOldIndices(olderThan, minIndices);

            //Check Settings Path
            Assert.Equal("bestbets%2A/_settings/index.creation_date?flat_settings=true", actualGetSettingsPath);

            //Check Delete Path
            Assert.Equal(expectedDeletePath, actualDeletePath);

            //Check indices list
            Assert.Equal(expectedDeletedIndices, indices);
        }

        [Fact]
        public void TestDelete_AuthError()
        {
            string responseFile = "ES_INDEX_SETTINGS_WITHOLD.json"; //The Response for get settings
            string expectedDeletePath = String.Join("%2C", GetExpectedIndices()); //Delete Path
            string[] expectedDeletedIndices = GetExpectedIndices(); //Deleted indices

            string aliasName = "bestbets";

            string actualGetSettingsPath = string.Empty;
            string actualDeletePath = string.Empty;
            JObject actualGetSettingsRequestOptions = null;
            JObject actualDeleteRequestOptions = null;

            ESBBIndexerService service = this.GetIndexerService(
            aliasName,
            conn =>
            {
                //This is the handler for getting the listing of indices/creationdates
                conn.RegisterRequestHandlerForType<Nest.GetIndexSettingsResponse>((req, res) =>
                {
                    //Store off Request
                    actualGetSettingsPath = req.Path;
                    actualGetSettingsRequestOptions = conn.GetRequestPost(req);

                    //Setup Response
                    res.StatusCode = 200;
                    res.Stream = TestingTools.GetTestFileAsStream(responseFile);
                });

                //Handle Delete Index response here
                conn.RegisterRequestHandlerForType<Nest.DeleteIndexResponse>((req, res) =>
                {
                    //Store off Request
                    actualDeletePath = req.Path;
                    actualDeleteRequestOptions = conn.GetRequestPost(req);
                    //Could also test the method...

                    //Setup Response
                    res.StatusCode = 401;
                    res.Stream = TestingTools.GetTestFileAsStream("ES_Acknowledged_False_Response.json");
                });
            });

            //The data in our test file only goes to 2/16, so let's set the date to 2/10
            DateTime olderThan = new DateTime(2017, 2, 10);

            //TODO: Should test logging functions here.
            Assert.Throws(typeof(Exception), () => service.DeleteOldIndices(olderThan, 7));
        }



        private static string[] GetExpectedIndices()
        {
            return new string[]
                {
                "bestbets20170202211006",
                "bestbets20170201131515",
                "bestbets20170201111006",
                "bestbets20170131165139",
                "bestbets20170131111005",
                "bestbets20170130061005",
                "bestbets20170129111005",
                "bestbets20170129011006",
                "bestbets20170128161004",
                "bestbets20170128061005",
                "bestbets20170128011005",
                "bestbets20170127161004",
                "bestbets20170126211005",
                "bestbets20170125161004",
                "bestbets20170125111005",
                "bestbets20170125061005",
                "bestbets20170124111004",
                "bestbets20170124061005",
                "bestbets20170123132300",
                "bestbets20170123111005",                                
                
                };
            }
    }
}
