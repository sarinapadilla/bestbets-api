using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

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
        private readonly ILogger<ESTokenAnalyzerService> _logger;

        /// <summary>
        /// Creates a new instance of a ESBestBetsMatchService
        /// </summary>        
        public ESTokenAnalyzerService(IElasticClient client, ILogger<ESTokenAnalyzerService> logger) //Needs someway to get an IElasticClient 
        {
            _elasticClient = client;
            _logger = logger;
        }

        /// <summary>
        /// Gets a count of the number of tokens as tokenized by elasticsearch
        /// </summary>
        /// <param name="term">The term to get token count</param>
        /// <returns>The number of tokens in the term</returns>
        public int GetTokenCount(string term)
        {
            var analyzeResponse = this._elasticClient.Analyze(
                a => a
                //TODO: Make alias a configuration option
                .Index("bestbets")
                .Analyzer("nostem")
                .Text(term)
            );

            if (!analyzeResponse.IsValid)
            {
                _logger.LogError("Elasticsearch Response for GetTokenCount is Not Valid.  Term '{0}'", term);
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
