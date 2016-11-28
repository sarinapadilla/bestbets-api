using System;

namespace NCI.OCPL.Services.BestBets.Tests.ESMatchTestData
{
    public class PancoastESMatchConnection : ESMatchConnection
    {
        protected override string TestFilePrefix
        {
            get
            {
                return "pancoast";
            }
        }
    }
}