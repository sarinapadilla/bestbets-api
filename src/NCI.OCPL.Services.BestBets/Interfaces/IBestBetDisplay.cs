namespace NCI.OCPL.Services.BestBets
{
    /// <summary>
    /// Represents Display information about a Best Bet
    /// </summary>
    public interface IBestBetDisplay
    {
        /// <summary>
        /// Gets the ID of the category
        /// </summary>
        /// <returns></returns>
        string ID { get; }

        /// <summary>
        /// Gets the name of the category
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the HTML to be displayed
        /// </summary>
        string HTML { get; }

        /// <summary>
        /// Gets the weighting of this category for diplay purposes
        /// </summary>
        int Weight { get; }

    }
}