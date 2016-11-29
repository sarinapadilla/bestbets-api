namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// Defines the options for a CGBestBetsDisplayService
    /// </summary>
    public class CGBestBetsDisplayServiceOptions
    {
        /// <summary>
        /// This is the host, with protocol, of where to fetch info from.
        /// </summary>
        /// <returns></returns>
        public string Host { get; set; }

        /// <summary>
        /// Get and sets the formatter for building a path to a BB category
        /// file.
        /// </summary>
        /// <returns></returns>
        public string BBCategoryPathFormatter { get; set; }
    }
}