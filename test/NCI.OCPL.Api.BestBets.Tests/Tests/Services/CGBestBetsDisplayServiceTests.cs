using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Testing;

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
        [Theory, MemberData(nameof(XmlDeserializingData))]
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
                .Respond(HttpStatusCode.OK, content);

            // Setup the mocked Options
            IOptions<CGBestBetsDisplayServiceOptions> bbClientOptions = GetMockOptions();

            CGBestBetsDisplayService bbClient = new CGBestBetsDisplayService(new HttpClient(mockHttp),
                bbClientOptions,
                NullLogger<CGBestBetsDisplayService>.Instance);

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

        /// <summary>
        /// Test that CGBestBetsDisplayService.IsHealthy responds correctly when the 
        /// health check URL returns an OK (HTTP 200) status.
        /// </summary>
        [Fact]
        public void HealthStatus_Healthy()
        {
            //Setup a mock HTTP handler
            var mockHttp = new MockHttpMessageHandler();

            // We don't care about the content for this test, just the return code
            ByteArrayContent content = new ByteArrayContent(Encoding.Unicode.GetBytes("<pretend_content />"));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");

            mockHttp
                .When("https://www.cancer.gov/")
                .Respond(HttpStatusCode.OK, content);

            // Setup the mocked Options
            IOptions<CGBestBetsDisplayServiceOptions> bbClientOptions = GetMockOptions();

            CGBestBetsDisplayService bbClient = new CGBestBetsDisplayService(new HttpClient(mockHttp),
                bbClientOptions,
                NullLogger<CGBestBetsDisplayService>.Instance);

            bool isHealthy = bbClient.IsHealthy;

            Assert.True(isHealthy);
        }

        /// <summary>
        /// Test that CGBestBetsDisplayService.IsHealthy responds correctly when the 
        /// health check URL returns an HTTP error code.
        /// </summary>
        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public void HealthStatus_Unhealthy(HttpStatusCode statusCode)
        {
            //Setup a mock HTTP handler
            var mockHttp = new MockHttpMessageHandler();

            // We don't care about the content for this test, just the return code
            ByteArrayContent content = new ByteArrayContent(Encoding.Unicode.GetBytes("<pretend_content />"));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");

            mockHttp
                .When("https://www.cancer.gov/")
                .Respond(statusCode, content);

            // Setup the mocked Options
            IOptions<CGBestBetsDisplayServiceOptions> bbClientOptions = GetMockOptions();

            CGBestBetsDisplayService bbClient = new CGBestBetsDisplayService(new HttpClient(mockHttp),
                bbClientOptions,
                NullLogger<CGBestBetsDisplayService>.Instance);

            bool isHealthy = bbClient.IsHealthy;

            Assert.False(isHealthy);
        }


        private IOptions<CGBestBetsDisplayServiceOptions> GetMockOptions()
        {
            Mock<IOptions<CGBestBetsDisplayServiceOptions>> bbClientOptions = new Mock<IOptions<CGBestBetsDisplayServiceOptions>>();
            bbClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new CGBestBetsDisplayServiceOptions()
                {
                    Host = "https://www.cancer.gov",
                    BBCategoryPathFormatter = "/PublishedContent/BestBets/{0}.xml",
                    HealthCheckPath = "/"
                }
            );

            return bbClientOptions.Object;
        }
    }

}