using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;


using Xunit;
using Moq;
using RichardSzalay.MockHttp;

using NCI.OCPL.Utils.Testing;
using NCI.OCPL.Services.CDE.PublishedContentListing;

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
            Mock<IOptions<PublishedContentListingServiceOptions>> bbClientOptions = new Mock<IOptions<PublishedContentListingServiceOptions>>();
            bbClientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new PublishedContentListingServiceOptions()
                {
                    Host = "https://www.cancer.gov",
                    ListRoot = "/PublishedContent/List"
                }
            );

            IPublishedContentListingService listClient = new CDEPubContentListingService(new HttpClient(mockHttp), bbClientOptions.Object);

            IEnumerable<IPathListInfo> actualList = listClient.ListAvailablePaths();

            /// TODO: Make this a list comparison.
            Assert.NotNull(actualList);
        }


    }

    /// <summary>
    /// Tests for CDEPubContentListingService.GetItemsForPath()
    /// </summary>
    public class GetItemsForPath
    {

    }

}