using System;
using System.Xml.Serialization;


namespace NCI.OCPL.Services.BestBets
{
    /// <summary>
    /// Represents a BestBetSynonym as returned by CGov XML
    /// </summary>
    public class CancerGovBestBetSynonym : IBestBetSynonym
    {
        /// <summary>
        ///Is this Synonym an exact match? 
        /// </summary>
        [System.Xml.Serialization.XmlAttribute]
        public bool IsExactMatch { get; set; }

        /// <summary>
        /// Gets the text of the synonym
        /// </summary>
        [System.Xml.Serialization.XmlText]
        public string Text { get; set; }
    }
}