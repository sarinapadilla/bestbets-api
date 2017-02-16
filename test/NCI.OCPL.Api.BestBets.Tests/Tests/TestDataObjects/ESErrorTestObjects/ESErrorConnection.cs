using System;
using System.IO;
using System.Threading.Tasks;

using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NCI.OCPL.Api.BestBets.Tests.Util;

namespace NCI.OCPL.Api.BestBets.Tests.ESHealthTestData
{
    /// <summary>
    /// Class used for mocking requests to Elasticsearch that return an error. 
    /// </summary>
    /// <seealso cref="NCI.OCPL.Api.BestBets.Tests.Util.ElasticsearchInterceptingConnection" />
    public class ESErrorConnection : ElasticsearchInterceptingConnection
    {
        /// <summary>
        /// Creates a new instance of the ESMatchConnection class
        /// </summary>
        /// <param name="testErrorCode">HTTP status code to return</param>
        public ESErrorConnection(int testErrorCode)
        {
            //Add Handlers            
            this.RegisterRequestHandlerForType<Nest.ClusterHealthResponse>((req, res) =>
            {
                // Health check is a GET request (e.g. https://ncias-d1592-v.nci.nih.gov:9299/_cluster/health/bestbets?pretty)
                // so we don't need to do anything special, just load the data file.
                //Get the file name for this round
                //res.Stream = //TestingTools.GetTestFileAsStream(GetTestFileName());

                res.StatusCode = testErrorCode;
            });

        }
    }
}

