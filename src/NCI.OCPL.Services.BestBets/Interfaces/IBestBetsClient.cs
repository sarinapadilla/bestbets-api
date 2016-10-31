
using System.Threading.Tasks;

namespace NCI.OCPL.Services.Bestbets {

    interface IBestBetsClient {

        /// <summary>
        /// Asynchronously gets as single Best Bet
        /// </summary>
        /// <returns></returns>
        Task GetBestBetAsync(string categoryID);

        
    }

}