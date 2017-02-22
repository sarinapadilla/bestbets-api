using System;

namespace NCI.OCPL.Api.BestBets.Tests.ESHealthTestData
{
    /// <summary>
    /// Mock version of the BestBetsMatchService for use in health check tests.
    /// This version always reports itself as healthy.
    /// </summary>
    public class HealthyBestBetsMatchService
        : IBestBetsMatchService
    {
        public bool IsHealthy
        {
            get
            {
                return true;
            }
        }

        public string[] GetMatches(string language, string cleanedTerm)
        {
            throw new NotImplementedException();
        }
    }
}
