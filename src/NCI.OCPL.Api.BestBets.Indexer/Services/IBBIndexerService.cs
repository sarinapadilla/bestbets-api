using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public interface IBBIndexerService
    {
        /// <summary>
        /// Creates a new timestamped Best Bets Index beginning
        /// with the best bets prefix.
        /// </summary>
        /// <returns>The name of the index that was created</returns>
        string CreateTimeStampedIndex();

        /// <summary>
        /// Indexes a collection of best bet matches
        /// </summary>
        /// <param name="indexName">The name of the index where the matches should be placed.</param>
        /// <param name="matches">A collection of BestBetsMatch items.</param>
        void IndexBestBetsMatches(string indexName, IEnumerable<BestBetsMatch> matches);

        /// <summary>
        /// Optimizes an index.  This ensures consistent scoring across servers.
        /// </summary>
        /// <param name="indexName">The name of the index to optimize.</param>
        void OptimizeIndex(string indexName);

        /// <summary>
        /// This will repoint the alias to the supplied index name.
        /// </summary>
        /// <param name="indexName">The name of the index to make current</param>
        void MakeIndexCurrentAlias(string indexName);

        /// <summary>
        /// Gets a list of indices currently associated with an alias
        /// </summary>
        /// <returns>A list of index names.</returns>
        string[] GetIndicesForAlias();

        /// <summary>
        /// This will remove any 
        /// </summary>
        /// <param name="removeBefore"></param>
        void DeleteOldIndices(DateTime olderThan);

    }
}
