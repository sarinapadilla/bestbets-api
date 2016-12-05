using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;


using Xunit;
using Moq;
using RichardSzalay.MockHttp;

using NCI.OCPL.Api.BestBets;
using NCI.OCPL.Api.BestBets.Tests;
using NCI.OCPL.Api.BestBets.Tests.CategoryTestData;
using NCI.OCPL.Utils.Testing;
using NCI.OCPL.Services.CDE.PublishedContentListing;
using System.Threading.Tasks;

namespace NCI.OCPL.Services.CDE.Tests.CDEPubContentListingServiceTests
{

    /// <summary>
    /// Tests for CDEPubContentListingService.ListAvailablePaths()
    /// </summary>
    public class ListAvailablePaths
    {
        /// <summary>
        /// Test that a list of available paths is loaded.
        /// </summary>
        [Fact]
        public void DataLoading()
        {
            string TestFilePath = "CDEPubContentListingService.ListAvailablePaths.json";

            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            mockHttp
                .When("https://www.cancer.gov/PublishedContent/List")
                .Respond(System.Net.HttpStatusCode.OK, content);

            // Setup the mocked Options
            Mock<IOptions<PublishedContentListingServiceOptions>> clientOptions = new Mock<IOptions<PublishedContentListingServiceOptions>>();
            clientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new PublishedContentListingServiceOptions()
                {
                    Host = "https://www.cancer.gov",
                    ListRoot = "/PublishedContent/List"
                }
            );

            IPublishedContentListingService listClient = new CDEPubContentListingService(new HttpClient(mockHttp), clientOptions.Object);

            IEnumerable<IPathListInfo> actualList = listClient.ListAvailablePaths();

            /// TODO: Make this a list comparison.
            Assert.NotNull(actualList);
        }

        /// <summary>
        /// Negative Test for Handling HTTP status errors from CancerGov.  (e.g. 404, 500, etc)
        /// Does GetAllBestBetsForIndexing() throw the expected exception?
        /// </summary>
        [Theory]
        [InlineData(System.Net.HttpStatusCode.InternalServerError)]
        [InlineData(System.Net.HttpStatusCode.Forbidden)]
        [InlineData(System.Net.HttpStatusCode.NotFound)]
        public void HttpStatusError(System.Net.HttpStatusCode status)
        {
            string TestFilePath = "CDEPubContentListingService.ListAvailablePaths.json";

            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            mockHttp
                .When("https://www.cancer.gov/PublishedContent/List")
                .Respond(status, content);

            // Setup the mocked Options
            Mock<IOptions<PublishedContentListingServiceOptions>> clientOptions = new Mock<IOptions<PublishedContentListingServiceOptions>>();
            clientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new PublishedContentListingServiceOptions()
                {
                    Host = "https://www.cancer.gov",
                    ListRoot = "/PublishedContent/List"
                }
            );

            IPublishedContentListingService listClient = new CDEPubContentListingService(new HttpClient(mockHttp), clientOptions.Object);

            Exception ex = Assert.Throws<NCI.OCPL.Services.CDE.PublishedContentListing.APIErrorException>(
                // We don't care about the return value, only that an exception occured.
                () => listClient.ListAvailablePaths()
            );

            // All errors should return a status 500.
            Assert.Equal(500, ((NCI.OCPL.Services.CDE.PublishedContentListing.APIErrorException)ex).HttpStatusCode);
        }


    }

    /// <summary>
    /// Tests for CDEPubContentListingService.GetItemsForPath()
    /// </summary>
    public class GetItemsForPath
    {
        /// <summary>
        /// Test that a list of available paths is loaded.
        /// </summary>
        [Theory]
        [InlineData("BestBets", "/")]
        public void DataLoading(string rootName, string dataPath)
        {
            // TODO: Create a TestData object.
            string TestFilePath = "CDEPubContentListingService.BestBets.json";

            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            mockHttp
                .When(string.Format("https://www.cancer.gov/PublishedContent/List?root={0}&path={1}&fmt=json", rootName, dataPath))
                .Respond(System.Net.HttpStatusCode.OK, content);

            // Setup the mocked Options
            Mock<IOptions<PublishedContentListingServiceOptions>> clientOptions = new Mock<IOptions<PublishedContentListingServiceOptions>>();
            clientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new PublishedContentListingServiceOptions()
                {
                    Host = "https://www.cancer.gov",
                    ListRoot = "/PublishedContent/List",
                    SpecificListPathFormatter = "?root={0}&path={1}&fmt=json"
                }
            );

            IPublishedContentListingService listClient = new CDEPubContentListingService(new HttpClient(mockHttp), clientOptions.Object);

            IPublishedContentListing actualList = listClient.GetItemsForPath(rootName, dataPath);

            /// TODO: Make this a list comparison.
            Assert.NotNull(actualList);
        }

    }

    /// <summary>
    /// Tests for CDEPubContentListingService.GetPublishedFile()
    /// </summary>
    public class GetPublishedFile
    {
        /// <summary>
        /// Test that a BestBets entry can be loaded
        /// </summary>
        [Fact]
        public void BestBetsEntry()
        {
            CancerResearchIdeas testData = new CancerResearchIdeas();


            // TODO: Create a TestData object.
            string TestFilePath = testData.TestFilePath;

            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

            mockHttp
                .When(string.Format("https://www.cancer.gov/PublishedContent/BestBets/{0}", filePath))
                .Respond(System.Net.HttpStatusCode.OK, content);

            // Setup the mocked Options
            Mock<IOptions<PublishedContentListingServiceOptions>> clientOptions = new Mock<IOptions<PublishedContentListingServiceOptions>>();
            clientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new PublishedContentListingServiceOptions()
                {
                    Host = "https://www.cancer.gov"
                }
            );


            IPublishedContentListingService publishedContentClient = new CDEPubContentListingService(new HttpClient(mockHttp), clientOptions.Object);

            CancerGovBestBet actualReturn = publishedContentClient.GetPublishedFile<CancerGovBestBet>(String.Format("/PublishedContent/BestBets/{0}", filePath));

            Assert.Equal(testData.ExpectedData, actualReturn, new IBestBetDisplayComparer());
        }

    }

}