using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Elasticsearch.Net;
using Nest;



namespace NCI.OCPL.Api.BestBets.Services
{
    /// <summary>
    /// Concrete implementation of an Elasticsearch backed ITokenAnalyzerService
    /// </summary>
    /// <seealso cref="NCI.OCPL.Api.BestBets.ITokenAnalyzerService" />
    public class ESTokenAnalyzerService : ITokenAnalyzerService
    {
        private IElasticClient _elasticClient;
        private CGBBIndexOptions _bestbetsConfig;
        private readonly ILogger<ESTokenAnalyzerService> _logger;
        private string _indexForAnalysis = null;

        /// <summary>
        /// Gets the index for analysis.
        /// </summary>
        /// <value>The index for analysis.</value>
        public string IndexForAnalysis {
            get 
            {
                //If someone has change the index, use that.
                return String.IsNullOrWhiteSpace(_indexForAnalysis) ? _bestbetsConfig.AliasName : _indexForAnalysis;
            }
        }

        /// <summary>
        /// Uses the default name of the index. (Which is based on the configuration)
        /// </summary>
        public void UseDefaultIndexName()
        {
            _indexForAnalysis = null;
        }

        /// <summary>
        /// Uses the name of the index.
        /// </summary>
        /// <param name="index">The name if the index to use for analysis</param>
        /// <exception cref="System.ArgumentNullException">Index Name must be set.</exception>
        public void UseIndexName(string index)
        {
            if (string.IsNullOrWhiteSpace(index))
                throw new ArgumentNullException("Index Name must be set.");

            _indexForAnalysis = index;
        }



        /// <summary>
        /// Creates a new instance of a ESBestBetsMatchService
        /// </summary>        
        public ESTokenAnalyzerService(IElasticClient client,
                        IOptions<CGBBIndexOptions> bestbetsConfig,
                        ILogger<ESTokenAnalyzerService> logger) //Needs someway to get an IElasticClient 
        {
            _elasticClient = client;
            _bestbetsConfig = bestbetsConfig.Value;
            _logger = logger;
        }

        /// <summary>
        /// Gets a count of the number of tokens as tokenized by elasticsearch
        /// </summary>
        /// <param name="term">The term to get token count</param>
        /// <returns>The number of tokens in the term</returns>
        public int GetTokenCount(string term)
        {
            IAnalyzeResponse analyzeResponse;

            try
            {
                analyzeResponse = this._elasticClient.Analyze(
                    a => a
                    .Index(IndexForAnalysis)
                    .Analyzer("nostem")
                    .Text(term)
                );
            }
            catch(UnexpectedElasticsearchClientException ex)
            {
                _logger.LogError("Error analyzing token count for term '{0}'. Reason: '{1}'. DebugInformation: {2}",
                    term, ex.FailureReason, ex.DebugInformation, ex);
                _logger.LogInformation("Trying again for term '{0}", term);

                // Try again.
                analyzeResponse = this._elasticClient.Analyze(
                    a => a
                    .Index(IndexForAnalysis)
                    .Analyzer("nostem")
                    .Text(term)
                );
            }

            if (!analyzeResponse.IsValid)
            {
                _logger.LogError("Elasticsearch Response for GetTokenCount is Not Valid.  Term '{0}'", term);
                _logger.LogError("Returned debug info: {0}.", analyzeResponse.DebugInformation);
                throw new APIErrorException(500, "Errors Occurred.");
            }

            int numberOfTokens = 0;
            if (analyzeResponse.Tokens != null)
            {
                foreach (AnalyzeToken t in analyzeResponse.Tokens)
                {
                    numberOfTokens++;
                }
            }

            return numberOfTokens;
        }

    }
}
