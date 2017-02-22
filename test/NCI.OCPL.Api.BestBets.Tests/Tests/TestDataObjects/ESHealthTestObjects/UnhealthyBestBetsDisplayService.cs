using System;

namespace NCI.OCPL.Api.BestBets.Tests.ESHealthTestData
{
    /// <summary>
    /// Mock version of the BestBetsDisplayService for use in health check tests.
    /// This version always reports itself as unhealthy.
    /// </summary>
    public class UnhealthyBestBetsDisplayService
        : IBestBetsDisplayService
    {
        public bool IsHealthy
        {
            get
            {
                return false;
            }
        }

        public IBestBetDisplay GetBestBetForDisplay(string categoryID)
        {
            throw new NotImplementedException();
        }
    }
}
