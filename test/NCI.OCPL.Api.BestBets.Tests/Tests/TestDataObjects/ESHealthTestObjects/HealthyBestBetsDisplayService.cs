using System;

namespace NCI.OCPL.Api.BestBets.Tests.ESHealthTestData
{
    /// <summary>
    /// Mock version of the BestBetsDisplayService for use in health check tests.
    /// This version always reports itself as healthy.
    /// </summary>
    public class HealthyBestBetsDisplayService
        : IBestBetsDisplayService
    {
        public bool IsHealthy
        {
            get
            {
                return true;
            }
        }

        public IBestBetDisplay GetBestBetForDisplay(string categoryID)
        {
            throw new NotImplementedException();
        }
    }
}
