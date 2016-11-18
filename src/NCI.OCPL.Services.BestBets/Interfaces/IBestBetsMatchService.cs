namespace NCI.OCPL.Services.BestBets
{
    /// <summary>
    /// Defines an interface to get best bets matches
    /// </summary>
    public interface IBestBetsMatchService
    {
        /// <summary>
        /// Gets a list of the BestBet Category IDs that matched our term 
        /// </summary>
        /// <param name="cleanedTerm">A term that have been cleaned of punctuation and special characters</param>
        /// <param name="language">The two-character language code to constrain the matches to</param>
        /// <returns>An array of category ids</returns>
        string[] GetMatches(string cleanedTerm, string language);
    }
}