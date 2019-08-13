using System;
namespace NCI.OCPL.Api.BestBets.Tests.ESDisplayTestData
{
    public class NotFoundDisplayTestData : BaseDisplayTestData
    {
        public override string TestFilePath => "NotFound.json";
        public override BestBetsCategoryDisplay ExpectedData => null;
    }
}
