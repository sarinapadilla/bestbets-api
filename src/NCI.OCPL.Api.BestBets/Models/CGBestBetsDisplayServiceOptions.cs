namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// Defines the options for a CGBestBetsDisplayService
    /// </summary>
    public class CGBestBetsDisplayServiceOptions
    {
        /// <summary>
        /// This is the host of the preview site, with protocol, of where to fetch info from.
        /// </summary>
        /// <returns></returns>
        public string PreviewHost { get; set; }

        /// <summary>
        /// This is the host of the live site, with protocol, of where to fetch info from.
        /// </summary>
        /// <returns></returns>
        public string LiveHost { get; set; }

        /// <summary>
        /// Get and sets the formatter for building a path to a BB category
        /// file.
        /// </summary>
        /// <returns></returns>
        public string BBCategoryPathFormatter { get; set; }

        /// <summary>
        /// Gets and sets the path, relative to Host, to access when verifying connectivity to the
        /// server where Best Bets display information is retrieved.
        /// </summary>
        public string HealthCheckPath { get; set; }
    }
}