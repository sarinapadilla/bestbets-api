using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace NCI.OCPL.Services.BestBets
{
    /// <summary>
    /// Represents a BestBetSynonym as returned by CGov XML
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("synonym", Namespace = "", IsNullable = false)]
    public class CancerGovBestBetSynonym : IXmlSerializable, IBestBetSynonym
    {
        /// <summary>
        ///Is this Synonym an exact match? 
        /// </summary>
        public bool IsExactMatch { get; set; }

        /// <summary>
        /// Gets the text of the synonym
        /// </summary>
        public string Text { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("synonym");
            reader.MoveToAttribute("IsExactMatch");

            IsExactMatch = bool.Parse(reader.Value);

            reader.MoveToContent(); //Move to Value

            Text = reader.ReadContentAsString();

            int i=1;

        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException("Serialization is not supported for this type.");
        }
    }
}