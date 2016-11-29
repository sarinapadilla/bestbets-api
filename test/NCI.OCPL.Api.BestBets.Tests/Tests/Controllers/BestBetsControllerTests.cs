using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Logging.Testing;

using Elasticsearch.Net;
using Nest;

using Newtonsoft.Json.Linq;

using Moq;
using Xunit;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Api.BestBets;
using NCI.OCPL.Api.BestBets.Controllers;
using NCI.OCPL.Api.BestBets.Tests.CategoryTestData;

namespace NCI.OCPL.Api.BestBets.Tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dotnet-core.html
    public class BestBetsControllerTests
    {
        public static IEnumerable<object[]> XmlDeserializingData => new[] {
            new object[] {
                "pancoast", 
                new PancoastTumorCategoryTestData() 
            }//,
            //new object[] {
            //    "breast cancer", 
            //    new BreastCancerCategoryTestData() 
            //}
        };

        [Fact]
        public void Get_Error_LanguageEmpty() 
        {
            Mock<IBestBetsDisplayService> displayService = new Mock<IBestBetsDisplayService>();
            Mock<IBestBetsMatchService> matchService = new Mock<IBestBetsMatchService>();

            // Create instance of controller
            BestBetsController controller = new BestBetsController(                
                matchService.Object,
                displayService.Object,
                NullLogger<BestBetsController>.Instance
            );

            APIErrorException ex = Assert.Throws<APIErrorException>( () => controller.Get(null, null) );

        }

        [Fact]
        public void Get_Error_LanguageBad() 
        {
            Mock<IBestBetsDisplayService> displayService = new Mock<IBestBetsDisplayService>();
            Mock<IBestBetsMatchService> matchService = new Mock<IBestBetsMatchService>();

            // Create instance of controller
            BestBetsController controller = new BestBetsController(                
                matchService.Object,
                displayService.Object,
                NullLogger<BestBetsController>.Instance
            );

            APIErrorException ex = Assert.Throws<APIErrorException>(() => controller.Get("Chicken", null));
        }

        [Fact]
        public void Get_Error_SearchTermBad() 
        {
            Mock<IBestBetsDisplayService> displayService = new Mock<IBestBetsDisplayService>();
            Mock<IBestBetsMatchService> matchService = new Mock<IBestBetsMatchService>();

            // Create instance of controller
            BestBetsController controller = new BestBetsController(                
                matchService.Object,
                displayService.Object,
                NullLogger<BestBetsController>.Instance
            );

            APIErrorException ex = Assert.Throws<APIErrorException>( () => controller.Get("en", null) );
        }


        [Theory, MemberData("XmlDeserializingData")]
        public void Get_EnglishTerm(string searchTerm, BaseCategoryTestData data) 
        {
            Mock<IBestBetsDisplayService> displayService = new Mock<IBestBetsDisplayService>(); 
            displayService
                .Setup(
                    dispSvc => dispSvc.GetBestBetForDisplay(
                        It.Is<string>(catID => catID == data.ExpectedData.ID)
                    )
                )
                .Returns(TestingTools.DeserializeXML<CancerGovBestBet>(data.TestFilePath));

            Mock<IBestBetsMatchService> matchService = new Mock<IBestBetsMatchService>();
            matchService
                .Setup(
                    matchSvc => matchSvc.GetMatches(                        
                        It.Is<string>(lang => lang == "en"),
                        It.Is<string>(term => term == searchTerm)
                    )
                )
                .Returns(new string[] { data.ExpectedData.ID });
            
            // Create instance of controller
            BestBetsController controller = new BestBetsController(                
                matchService.Object,
                displayService.Object,
                NullLogger<BestBetsController>.Instance
            );

            IBestBetDisplay[] actualItems = controller.Get("en", searchTerm);

            Assert.Equal(actualItems, new IBestBetDisplay[] { data.ExpectedData }, new IBestBetDisplayComparer());
        }

    }
}
