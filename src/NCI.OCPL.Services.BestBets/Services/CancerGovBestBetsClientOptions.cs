namespace NCI.OCPL.Services.BestBets.Services
{
    public class CancerGovBestBetsClientOptions
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