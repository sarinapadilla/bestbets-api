using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// Service that can call an analyzer to get a count of indexable tokens for a term.
    /// </summary>
    public interface ITokenAnalyzerService
    {
        /// <summary>
        /// Gets a count of the number of tokens as tokenized by search indexer
        /// </summary>
        /// <param name="term">The term to get token count</param>
        /// <returns>The number of tokens in the term</returns>
        int GetTokenCount(string term);
    }
}
