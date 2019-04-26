using System;
namespace NCI.OCPL.Api.BestBets.Tests.ESDisplayTestData
{
    public abstract class BaseDisplayTestData
    {
        /// <summary>
        /// Gets the file name containing the actual test data.
        /// </summary>
        /// <returns></returns>
        public abstract string TestFilePath { get; }

        /// <summary>
        /// Gets an instance of the Expected Data object
        /// </summary>
        /// <returns></returns>
        public abstract BestBetsCategoryDisplay ExpectedData { get; }
    }
}
