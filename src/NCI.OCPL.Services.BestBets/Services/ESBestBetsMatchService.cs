using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Elasticsearch.Net;
using Nest;

namespace NCI.OCPL.Services.BestBets.Services
{
    /// <summary>
    /// Class represents an Elasticsearch based Best Bets Match Service
    /// </summary>
    public class ESBestBetsMatchService : IBestBetsMatchService
    {
        private IElasticClient _elasticClient;
        private readonly ILogger<ESBestBetsMatchService> _logger;

        /// <summary>
        /// Creates a new instance of a ESBestBetsMatchService
        /// </summary>        
        public ESBestBetsMatchService(IElasticClient client, ILogger<ESBestBetsMatchService> logger) //Needs someway to get an IElasticClient 
        {
            _elasticClient = client;
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of the BestBet Category IDs that matched our term 
        /// </summary>        
        /// <param name="language">The two-character language code to constrain the matches to</param>
        /// <param name="cleanedTerm">A term that have been cleaned of punctuation and special characters</param>
        /// <returns>An array of category ids</returns>
        public string[] GetMatches(string language, string cleanedTerm) 
        {

            // Step 2. Get Number of Tokens in the term
            int numTokens = GetTokenCount(cleanedTerm);

            // Step 4. Iterate over the matches    
            IEnumerable<BestBetsMatch> matches = GetBestBetMatches(
                cleanedTerm,
                language,
                numTokens
            );

            // Step 5. Process the matches and extract only the category IDs we will
            // be returning to the client
            string[] validCategories = FilterValidCategories(matches, numTokens); 

            return validCategories;
        }


        /// <summary>
        /// Process a list of BestBetsMatches and returns an array of category IDs for display. 
        /// </summary>
        /// <param name="matches">A list of matches</param>
        /// <param name="numTokens">The number of tokens in the search term</param>
        /// <returns>An array of category ids</returns>
        private string[] FilterValidCategories(IEnumerable<BestBetsMatch> matches, int numTokens) 
        {
            //CatIDs to ignore because of a negation
            List<string> excludedIDs = new List<string>();

            //The IDs that we will end up sending back to the client.
            List<string> matchedIDs = new List<string>();

            // Iterate through ALL the matches and extract the categories that
            // should be displayed.  There may be multiple matches for a single
            // category, the probability increase with the number of tokens. 
            foreach (BestBetsMatch match in matches) {
                
                //Exact matches need to match the exact number of tokens as well.
                //Exact matches can be used for both inclusion and exclusion
                if (match.IsExact && (match.TokenCount != numTokens))
                    continue;

                
                if (match.IsNegated)
                {
                    // A negated match will remove a category from the display.
                    // For example, "Breast Cancer Treatment" would return the 
                    // Best Bets for "Breast Cancer" and "Breast Cancer Treatment".
                    // However, as "Breast Cancer Treatment" is more specific, a 
                    // BB editor has created a Negated synonyn of "Treatment" for
                    // the "Breast Cancer" category.  So we should only show
                    // "Breast Cancer Treatment" to the user. 
                    if (!excludedIDs.Contains(match.ContentID))
                    {
                        excludedIDs.Add(match.ContentID);
                    }
                    
                    matchedIDs.Remove(match.ContentID);
                }
                else
                {
                    // Just a normal match.  Let's make sure that we are not excluding
                    // that category first, otherwise, add it to the list of matches.
                    if (!matchedIDs.Contains(match.ContentID) 
                        && !excludedIDs.Contains(match.ContentID))
                    {
                        matchedIDs.Add(match.ContentID);                                        
                    }
                }
            }

            return matchedIDs.ToArray();            
        }

        /// <summary>
        /// Helper function that generates a list of best bet matches to iterate through.
        /// </summary>
        /// <param name="cleanedTerm">The Cleaned Term</param>
        /// <param name="isSpanish">Is this term spanish or not</param>
        /// <param name="numTokens">The number of tokens an analyzer would break this up into</param>
        /// <returns>IEnumerable<BestBetsMatch> suitable for iterating through</returns>
        private IEnumerable<BestBetsMatch> GetBestBetMatches(string cleanedTerm, string language, int numTokens) 
        {            
            string templateFileName = "bestbets_bestbets_cgov_" + language;

            //We need to perform a separate query for each of the number of matches.
            for (int i = 1; i <= numTokens; i++)
            {
                var response = _elasticClient.SearchTemplate<BestBetsMatch>(sd => {
                    sd = sd
                    .Index("bestbets")
                    .Type("synonyms")
                    .File(templateFileName)
                    .Params(pd =>
                    {
                        //Add params that are always set.
                        pd
                            .Add("searchstring", cleanedTerm)
                            .Add("searchtokencount", numTokens)
                            .Add("matchedtokencount", i);
                        
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


    }
}