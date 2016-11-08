using System.Net.Http;

using Xunit;
using Moq;
using RichardSzalay.MockHttp;

using NCI.OCPL.Utils.Testing;

namespace NCI.OCPL.Services.BestBets.Tests
{
    public class CancerGovBestBetsClientTests
    {
        
        [Fact]
        public void GetBestBetForDisplay()
        {
            //Setup a mock handler, which is what HttpClient uses under the hood to fetch
            //data.
            var mockHttp = new MockHttpMessageHandler();

            string filePath = "";

            ByteArrayContent content = new ByteArrayContent(TestingTools.GetTestFileAsBytes(filePath));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");

            mockHttp
                .When("https://www.cancer.gov/PublishedContent/BestBets/36408.xml")
                .Respond(System.Net.HttpStatusCode.OK, content);

            

        }


    }
}