
using System.Threading.Tasks;

namespace NCI.OCPL.Services.Bestbets {

    interface IBestBetsClient {

        /// <summary>
        /// Asynchronously gets as single Best Bet
        /// </summary>
        /// <returns></returns>
        //Task GetBestBetForDisplayAsync(string categoryID);

        /// <summary>
        /// Synchronously gets a single Best Bet
        /// </summary>
        /// <param name="categoryID">The category ID to retrieve</param>
        /// <returns>A IBestBetDisplay represented by the category</returns>
        IBestBetDisplay GetBestBetForDisplay(string categoryID);
        
    }

}