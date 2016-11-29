using System.Collections.Generic;
using System.Linq;

using Xunit;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Api.BestBets.Tests.CategoryTestData;

namespace NCI.OCPL.Api.BestBets.Tests
{
    /// <summary>
    /// Tests for the CancerGovBestBet class, 
    /// and by extension the CancerGovBestBetSynonym class.
    /// These are really just deserialization tests,    
    /// </summary>
    public class CancerGovBestBetCategoryTest
    {
        public static IEnumerable<object[]> XmlDeserializingData => new[] {
            new object[] { new PancoastTumorCategoryTestData() },
            new object[] { new BreastCancerCategoryTestData() }
        };

        [Theory, MemberData("XmlDeserializingData")]
        public void Can_Deserialize_XML(BaseCategoryTestData data) 
        {
            //Setup the expected object.
            CancerGovBestBet actCat = TestingTools.DeserializeXML<CancerGovBestBet>(data.TestFilePath);

            //Compare Object
            Assert.Equal(data.ExpectedData, actCat, new IBestBetCategoryComparer());
        }
    }
}