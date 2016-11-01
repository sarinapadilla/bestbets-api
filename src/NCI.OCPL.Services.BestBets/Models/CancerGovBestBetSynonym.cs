using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace NCI.OCPL.Services.BestBets
{
    /// <summary>
    /// Represents a BestBetSynonym as returned by CGov XML
    /// </summary>
    public class CancerGovBestBetSynonym : IXmlSerializable, IBestBetSynonym
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

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {

            
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("IsExactMatch", IsExactMatch.ToString());
        }
    }
}