using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;

using Xunit;
using Moq;
using RichardSzalay.MockHttp;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Api.BestBets.Services;
using NCI.OCPL.Api.BestBets.Tests.CategoryTestData;

namespace NCI.OCPL.Api.BestBets.Tests.CGBestBetsDisplayServiceTests
{
    public class GetBestBetForDisplayTests
    {
        
        public static IEnumerable<object[]> XmlDeserializingData => new[] {
            new object[] { new PancoastTumorCategoryTestData() },
            new object[] { new BreastCancerCategoryTestData() }
        };

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData("XmlDeserializingData")]
        public void GetBestBetForDisplay_DataLoading(BaseCategoryTestData data)
        {
            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = data.TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");

            mockHttp
                .When(string.Format("https://www.cancer.gov/PublishedContent/BestBets/{0}.xml", data.ExpectedData.ID))
                .Respond(System.Net.HttpStatusCode.OK, content);

            // Setup the mocked Options
            Mock<IOptions<CGBestBetsDisplayServiceOptions>> bbClientOptions = new Mock<IOptions<CGBestBetsDisplayServiceOptions>>();
            bbClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new CGBestBetsDisplayServiceOptions(){
                    Host = "https://www.cancer.gov",
                    BBCategoryPathFormatter = "/PublishedContent/BestBets/{0}.xml"
                }
            );

            CGBestBetsDisplayService bbClient = new CGBestBetsDisplayService(new HttpClient(mockHttp), bbClientOptions.Object);

            IBestBetDisplay actDisplay = bbClient.GetBestBetForDisplay(data.ExpectedData.ID);

            Assert.Equal(data.ExpectedData, actDisplay, new IBestBetDisplayComparer());
        }

        /// <summary>
        /// Test for Handling HTTP status errors from the Cgov side.  (e.g. 404, 500, etc)
        /// </summary>
        public void GetBestBetForDisplay_HttpStatusError() 
        {

        }

        /// <summary>
        /// Test for bad configuration file errors.
        /// </summary>
        public void GetBestBetForDisplay_ConfigError() 
        {
        }

    }

    /// <summary>
    /// Implements tests for CGBestBetsDisplayService.GetAllBestBetsForIndexing().
    /// </summary>
    public class GetAllBestBetsForIndexingTests
    {

        /// <summary>
        /// Test that data is retrieved from GetAllBestBetsForIndexing()
        /// </summary>
        [Fact]
        public void DataLoading()
        {
            string TestFilePath = "CGBBCategory.BestBetsList.json";

            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            mockHttp
                .When("https://www.cancer.gov/PublishedContent/List?root=BestBets&fmt=json")
                .Respond(System.Net.HttpStatusCode.OK, content);

            // Setup the mocked Options
            Mock<IOptions<CGBestBetsDisplayServiceOptions>> bbClientOptions = new Mock<IOptions<CGBestBetsDisplayServiceOptions>>();
            bbClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new CGBestBetsDisplayServiceOptions(){
                    Host = "https://www.cancer.gov",
                    BBCategoryPathFormatter = "/PublishedContent/List?root=BestBets&fmt=json"
                }
            );

            CGBestBetsDisplayService bbClient = new CGBestBetsDisplayService(new HttpClient(mockHttp), bbClientOptions.Object);

            IEnumerable<PublishedContentInfo> actualList = bbClient.GetAllBestBetsForIndexing();

            /// TODO: Make this a list comparison.
            Assert.NotNull(actualList);
        }

        /// <summary>
        /// Negative Test for Handling HTTP status errors from CancerGov.  (e.g. 404, 500, etc)
        /// Does GetAllBestBetsForIndexing() throw the expected exception?
        /// </summary>
        [Theory]
        [InlineData(System.Net.HttpStatusCode.InternalServerError)]
        //[InlineData(System.Net.HttpStatusCode.Forbidden)]
        //[InlineData(System.Net.HttpStatusCode.NotFound)]
        public void GetBestBetForDisplay_HttpStatusError(System.Net.HttpStatusCode status) 
        {
            string TestFilePath = "CGBBCategory.BestBetsList.json";

            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = TestFilePath;

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            mockHttp
                .When("https://www.cancer.gov/PublishedContent/List?root=BestBets&fmt=json")
                .Respond(status, content);

            // Setup the mocked Options
            Mock<IOptions<CGBestBetsDisplayServiceOptions>> bbClientOptions = new Mock<IOptions<CGBestBetsDisplayServiceOptions>>();
            bbClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new CGBestBetsDisplayServiceOptions(){
                    Host = "https://www.cancer.gov",
                    BBCategoryPathFormatter = "/PublishedContent/List?root=BestBets&fmt=json"
                }
            );

            CGBestBetsDisplayService bbClient = new CGBestBetsDisplayService(new HttpClient(mockHttp), bbClientOptions.Object);

            Exception ex = Assert.Throws<APIErrorException>(
                // We don't care about the return value, only that an exception occured.
                () => {
                    IEnumerable<PublishedContentInfo> actualList = bbClient.GetAllBestBetsForIndexing();
                }
            );

            // All errors should return a status 500.
            Assert.Equal(400, ((APIErrorException)ex).HttpStatusCode);
        }
    }
}