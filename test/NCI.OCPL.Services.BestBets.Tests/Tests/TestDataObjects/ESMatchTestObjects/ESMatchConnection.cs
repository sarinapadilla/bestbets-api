using System;
using System.IO;
using System.Threading.Tasks;

using Elasticsearch.Net;

using NCI.OCPL.Utils.Testing;

namespace NCI.OCPL.Services.BestBets.Tests.ESMatchTestData
{
    /// <summary>
    /// Class used for mocking BestBet Match requests to Elasticsearch.  This should be
    /// used as the base class of test specific Connections object passed into an ElasticClient. 
    /// </summary>
    public class ESMatchConnection : IConnection, IDisposable
    {

        /// <summary>
        /// Creates a new instance of the ESMatchConnection class
        /// </summary>
        /// <param name="testFilePrefix">The prefix of the test files</param>
        public ESMatchConnection(string testFilePrefix)
        {
            this.TestFilePrefix = testFilePrefix;
        }

        /// <summary>
        /// Gets the prefix of a testdata file for this test.
        /// </summary>
        /// <returns></returns>
        private string TestFilePrefix { get; set; } 

        private string GetTestFileName(int numTerms)
        {
            return string.Format("ESMatchData/{0}_{1}.json", TestFilePrefix, numTerms);
        }

        private string GetAnalyzeFileName()
        {
            return string.Format("ESMatchData/{0}_analyze.json", TestFilePrefix);
        }


        /// <summary>
        /// Mocks asynchronous request to an Elasticsearch server. 
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<ElasticsearchResponse<TReturn>> RequestAsync<TReturn>(RequestData requestData)
			where TReturn : class 
        {
            ResponseBuilder<TReturn> builder = GetResponseBuilder<TReturn>(requestData);
            return await builder.ToResponseAsync().ConfigureAwait(false); 
        }

        /// <summary>
        /// Mocks synchronous request to an Elasticsearch server.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
		public ElasticsearchResponse<TReturn> Request<TReturn>(RequestData requestData)
			where TReturn : class
        {
            ResponseBuilder<TReturn> builder = GetResponseBuilder<TReturn>(requestData);
            return builder.ToResponse(); 
        }

        /// <summary>
        /// "Factory-ish" method for handling requests and mapping to the correct response data.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private ResponseBuilder<TReturn> GetResponseBuilder<TReturn>(RequestData requestData) 
            where TReturn : class
        {
            ResponseBuilder<TReturn> builder = new ResponseBuilder<TReturn>(requestData);
            builder.StatusCode = 200;

            switch (requestData.Path) 
            {
                case "bestbets/_analyze" : {
                    SetAnalyzeResponseOnBuilder(builder, requestData);
                    break;
                } 
            }
            
            return builder; 
        }

        /// <summary>
        /// Handles setting up the response data for an Analyze request.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="requestData"></param>
        private void SetAnalyzeResponseOnBuilder<T>(ResponseBuilder<T> builder, RequestData requestData)
            where T : class
        {
            builder.Stream = TestingTools.GetTestFileAsStream(GetAnalyzeFileName());
        }

        /// <summary>
        /// Handles setting up the response data for a search template request.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="requestData"></param>
        private void SetSearchResponseOnBuilder<T>(ResponseBuilder<T> builder, RequestData requestData)
            where T : class
        {
            builder.Stream = TestingTools.GetTestFileAsStream(GetAnalyzeFileName());

            //Let's find the matchtokens for this request.
            int numTokens = 1;
                        
            builder.Stream = TestingTools.GetTestFileAsStream(GetTestFileName(numTokens));
        }












        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }


        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ESMatchConnection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}