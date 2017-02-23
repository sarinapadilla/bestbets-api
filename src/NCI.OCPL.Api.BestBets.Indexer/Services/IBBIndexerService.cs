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
        /// <returns>The number of items successfully indexed</returns>
        int IndexBestBetsMatches(string indexName, IEnumerable<BestBetsMatch> matches, int batchSize = 1000);

        /// <summary>
        /// Optimizes an index.  This ensures consistent scoring across servers.
        /// </summary>
        /// <param name="indexName">The name of the index to optimize.</param>
        /// <returns>A bool indicating success</returns>
        bool OptimizeIndex(string indexName);

        /// <summary>
        /// This will repoint the alias to the supplied index name.
        /// </summary>
        /// <param name="indexName">The name of the index to make current</param>
        /// <returns>A bool indicating success</returns>
        bool MakeIndexCurrentAlias(string indexName);

        /// <summary>
        /// Gets a list of indices currently associated with an alias
        /// </summary>
        /// <returns>A list of index names.</returns>
        string[] GetIndicesForAlias();

        /// <summary>
        /// This will remove any 
        /// </summary>
        /// <param name="removeBefore"></param>
        /// <param name="minIndices">The minimum number of indices to keep regardless of date.</param>
        /// <returns>A list of the deleted indices</returns>
        string[] DeleteOldIndices(DateTime olderThan, int minIndices);

    }
}
