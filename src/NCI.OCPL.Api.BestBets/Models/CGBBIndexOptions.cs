namespace NCI.OCPL.Api.BestBets
{
    /// <summary>
    /// Options for the BestBets indices
    /// </summary>
    public class CGBBIndexOptions
    {
        /// <summary>
        /// Gets or sets the name of the live alias.
        /// </summary>
        /// <value>The name of the live alias.</value>
        public string LiveAliasName { get; set; }

        /// <summary>
        /// Gets or sets the name of the preview alias.
        /// </summary>
        /// <value>The name of the preview alias.</value>
        public string PreviewAliasName { get; set; }
    }
}
