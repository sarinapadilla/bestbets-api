using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Nest;
using Elasticsearch.Net;

namespace NCI.OCPL.Services.BestBets.Controllers
{
    [Route("[controller]")]
    public class BestBetsController : Controller
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<BestBetsController> _logger;

        /// <summary>
        /// Creates a new instance of a BestBetsController
        /// </summary>
        /// <param name="client">An IElasticClient for connecting to an elastic cluster</param>
        /// <param name="logger">A logger</param>
        public BestBetsController(
            IElasticClient client,
            ILogger<BestBetsController> logger
        ) 
        {
            this._elasticClient = client;
            this._logger = logger;
        }


        // GET api/values/5
        [HttpGet("{language}/{term}")]
        public string Get(string language, string term)
        {
            if (String.IsNullOrWhiteSpace(language))
                throw new APIErrorException(400, "You must supply a language and search term");

            if (language.ToLower() != "en" || language.ToLower() != "es")
                throw new APIErrorException(404, "Unsupported Language. Please try either 'en' or 'es'");
            
            if (String.IsNullOrWhiteSpace(term))
                throw new APIErrorException(400, "You must supply a search term");

            // Step 1. Remove Punctuation
            string cleanedTerm = CleanTerm(term);

            // Step 2. Get Number of Tokens in the term
            int numTokens = GetTokenCount(cleanedTerm);

            // Step 3. Setup language param
            bool isSpanish = false;
            if (language.ToLower() == "es")
                isSpanish = true;

            // Step 4. Iterate over the matches    
            var matches = GetBestBetMatches(
                cleanedTerm,
                isSpanish,
                numTokens
            );

            //CatIDs to ignore because of a negation
            List<string> excludedIDs = new List<string>();

            //The IDs that we will end up sending back to the client.
            List<string> matchedIDs = new List<string>();

            foreach (BestBetsMatch match in matches) {
                
                //Exact matches need to match the exact number of tokens as well
                if (match.IsExact && (match.TokenCount != numTokens))
                    continue;

                
                if (match.IsNegated)
                {
                    if (!excludedIDs.Contains(match.ContentID))
                    {
                        excludedIDs.Add(match.ContentID);
                    }
                    
                    matchedIDs.Remove(match.ContentID);
                }
                else
                {
                    if (!matchedIDs.Contains(match.ContentID) 
                        && !excludedIDs.Contains(match.ContentID))
                    {
                        matchedIDs.Add(match.ContentID);                                        
                    }
                }
            }

            //matchedIDs should be good.
            //Now get categories for ID.

            return term;
        }

        /// <summary>
        /// Helper function that generates a list of best bet matches to iterate through.
        /// </summary>
        /// <param name="cleanedTerm">The Cleaned Term</param>
        /// <param name="isSpanish">Is this term spanish or not</param>
        /// <param name="numTokens">The number of tokens an analyzer would break this up into</param>
        /// <returns>IEnumerable<BestBetsMatch> suitable for iterating through</returns>
        private IEnumerable<BestBetsMatch> GetBestBetMatches(string cleanedTerm, bool isSpanish, int numTokens) 
        {
            //We need to perform a separate query for each of the number of matches.
            for (int i = 1; i <= numTokens; i++)
            {
                var response = _elasticClient.SearchTemplate<BestBetsMatch>(sd => {
                    sd = sd
                    .Index("bestbets")
                    .Type("synonyms")
                    .File("bestbets_bbgetcategory")
                    .Params(pd =>
                    {
                        //Add params that are always set.
                        pd
                            .Add("searchstring", cleanedTerm)
                            .Add("searchtokencount", numTokens)
                            .Add("matchedtokencount", i);

                        //Add optional parameters
                        if (isSpanish)
                            pd.Add("is_spanish", 1);
                        
                        return pd;
                    });

                    return sd;
                });

                //Test if response is valid
                if (!response.IsValid) {
                    _logger.LogError("Elasticsearch Response is Not Valid.  Term '{0}'", cleanedTerm);
                    throw new APIErrorException(500, "Errors Occurred.");
                }

                if (response.Total > 0) 
                {
                    foreach (BestBetsMatch match in response.Documents)
                    {                        
                        yield return match;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a count of the number of tokens as tokenized by elasticsearch
        /// </summary>
        /// <param name="term">The term to get token count</param>
        /// <returns>The number of tokens in the term</returns>
        private int GetTokenCount(string term) 
        {
            var analyzeResponse = this._elasticClient.Analyze(
                a => a
                .Index("bestbets")
                .Analyzer("nostem")
                .Text(term)
            );

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


        //TODO: Move CleanTerm to a shared class for use by the indexer as well.

        /// <summary>
        /// This will remove punctuation from a term
        /// </summary>
        /// <param name="term">The term to clean</param>
        /// <returns>The cleaned term</returns>
        private string CleanTerm(string term)
        {
            var sb = new StringBuilder();
            foreach (char c in term)
            {
                if (!char.IsPunctuation(c))
                    sb.Append(c);
            }
            term = sb.ToString();

            //TODO: Verify that this list is not actually a duplicate of 
            return System.Text.RegularExpressions.Regex.Replace(term, "[-':;\"\\./]", "");
        }

    }
}
