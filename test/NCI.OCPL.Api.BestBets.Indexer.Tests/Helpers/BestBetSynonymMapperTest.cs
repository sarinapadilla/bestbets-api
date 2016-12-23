using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using Xunit;
using NCI.OCPL.Utils.Testing;
using NCI.OCPL.Api.BestBets.Tests.Util;

using NCI.OCPL.Api.BestBets.Indexer.Tests.CategoryTestData;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests

{
    public class BestBetSynonymMapperTest
    {
        public static IEnumerable<object[]> MappingData => new[] {
            new object[] { SynonymTestData.LoadTestData("CGBBSynonyms.PancoastTumor.json") },
            new object[] { SynonymTestData.LoadTestData("CGBBSynonyms.BreastCancer.json") }
        };

        /// <summary>
        /// Verify that the correct number of synonmyms is created.
        /// </summary>
        /// <param name="data">The BaseSynonymTestData containing reference data.</param>
        [Theory, MemberData("MappingData")]
        void CorrectNumberOfSynonymsFound(SynonymTestData data)
        {
            // Load test BestBet object
            string filePath = data.TestFilePath;
            CancerGovBestBet testData = TestingTools.DeserializeXML<CancerGovBestBet>(filePath);

            int expectedCount = data.ExpectedMatches.Count();

            // Create a BestBetMapper from a test BestBet.
            BestBetSynonymMapper mapper = new BestBetSynonymMapper(
                GetTokenizerServiceForData(data), 
                testData
            );

            int actualCount = mapper.Count();

            // Verify that returned list of BestBetMatch objects matches what's expected.
            Assert.Equal(expectedCount, actualCount);
        }

        /// <summary>
        /// Verify that synonyms are correctly converted into BestBetsMatch objects.
        /// </summary>
        /// <param name="data"></param>
        [Theory, MemberData("MappingData")]
        void SynonymsMappedCorrectly(SynonymTestData data)
        {
            // Load test BestBet object
            string filePath = data.TestFilePath;
            CancerGovBestBet testData = TestingTools.DeserializeXML<CancerGovBestBet>(filePath);

            // Put the expected matches into a dictionary for fast lookup.
            Dictionary<string, BestBetsMatch> dictExpectedMatches = new Dictionary<string, BestBetsMatch>();
            foreach (BestBetsMatch item in data.ExpectedMatches)
                dictExpectedMatches.Add(item.Synonym, item);

            // Create a BestBetMapper from a test BestBet.
            BestBetSynonymMapper mapper = new BestBetSynonymMapper(
                GetTokenizerServiceForData(data), 
                testData
            );

            Assert.All(mapper, match => {
                string synonym = match.Synonym;
                Assert.True(dictExpectedMatches.ContainsKey(synonym));
                Assert.Equal(dictExpectedMatches[synonym], match, new BestBetsMatchComparer());
            });
        }

        private ITokenAnalyzerService GetTokenizerServiceForData(SynonymTestData data)
        {
            

            Mock<ITokenAnalyzerService> mockTokenService = new Mock<ITokenAnalyzerService>();
            mockTokenService
                .Setup(ts => ts.GetTokenCount(
                    It.IsAny<string>()
                 ))
                 .Returns((string input) =>
                 {
                     return data.GetTokenCount(input);
                 });

            return mockTokenService.Object;
        }

    }
}
