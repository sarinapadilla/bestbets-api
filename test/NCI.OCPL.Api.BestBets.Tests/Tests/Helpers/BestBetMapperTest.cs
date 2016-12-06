using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;


using NCI.OCPL.Api.BestBets.Tests.CategoryTestData;


namespace NCI.OCPL.Api.BestBets.Tests.Tests.Helpers
{
    public class BestBetMapperTest
    {
        public static IEnumerable<object[]> MappingData => new[] {
            new object[] { new PancoastTumorSynonymTestData() }//,
            //new object[] { new BreastCancerCategoryTestData() }
        };


        [Theory, MemberData("MappingData")]
        void Mapping(BaseSynonymTestData data)
        {
            foreach (BestBetsMatch match in data.ExpectedData)
            {

            }
            Assert.True(false);
        }
    }
}
