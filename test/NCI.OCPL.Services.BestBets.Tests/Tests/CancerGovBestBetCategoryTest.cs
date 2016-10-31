
using Xunit;

using NCI.OCPL.Utils.Testing;

namespace NCI.OCPL.Services.BestBets.Tests
{
    public class CancerGovBestBetCategoryTest
    {
        [Fact]
        public void Can_Deserialize_Basic_XML() 
        {
            //Setup the expected object.
            CancerGovBestBet expCat = new CancerGovBestBet() {
                ID = "36012",
                Name = "Pancoast Tumor",
                Weight = 20,
                IsExactMatch = false,
                Language = "en",
                Display = true,
                IncludeSynonyms = new CancerGovBestBetSynonym[] {
                    new CancerGovBestBetSynonym() {
                        IsExactMatch = false,
                        Text = "pancoast"
                    }                
                },
                ExcludeSynonyms = new CancerGovBestBetSynonym[] {},
                HTML = ""
            };

            CancerGovBestBet actCat = TestingTools.DeserializeXML<CancerGovBestBet>("CGBBCategory.PancoastTumor.xml");

            Assert.Equal(expCat, actCat);
        }

        [Fact]
        public void Can_Deserialize_Complex_XML() 
        {
            Assert.True(false);
        }
    }
}