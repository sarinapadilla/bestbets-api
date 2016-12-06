using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// Provides mapping functions to map CancerGovBestBet objects
    /// into other object types.
    /// </summary>
    public class BestBetMapper : IEnumerable<BestBetsMatch>
    {
        private CancerGovBestBet _bestBet;

        private bool _isFirst;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The CancerGovBestBet which is being mapped to other objects.</param>
        public BestBetMapper(CancerGovBestBet source)
        {
            _bestBet = source;
            _isFirst = true;
        }

        public IEnumerator<BestBetsMatch> GetEnumerator()
        {
            //string synonymText;


            // TODO: Handle the case for the BestBet being its own synonym.

            // TODO: Iterate over the list of exclude synonyms.


            foreach (IBestBetSynonym synonym in _bestBet.IncludeSynonyms)
            {
                // Allocate a new match each time so we don't end up modifying an
                // object which is being used somewhere else.
                BestBetsMatch match = new BestBetsMatch()
                {
                    Category = synonym.Text,
                    ContentID = _bestBet.ID,
                    Synonym = synonym.Text,
                    Language = _bestBet.Language,
                    IsNegated = true,      // does the synonym exist in the ExcludeSynonyms list?
                    IsExact = synonym.IsExactMatch,
                    TokenCount = -5   // Needs logic to set this from elastic search.
                };

                yield return match;
            }

        }

        /// <summary>
        /// Required for IEnumerable<T>.  Do not use."/>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
