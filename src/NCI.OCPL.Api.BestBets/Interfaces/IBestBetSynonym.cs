namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// Represents a synonym of a Best Bet Category 
    /// </summary>
    public interface IBestBetSynonym
    {
        /// <summary>
        ///Is this Synonym an exact match? 
        /// </summary>
        bool IsExactMatch { get; }

        /// <summary>
        /// Gets the text of the synonym
        /// </summary>
        string Text { get; }   
    }
}