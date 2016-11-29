using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace NCI.OCPL.Api.BestBets
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
            //Read Attribute First 
            reader.MoveToAttribute("IsExactMatch");

            //This will trim up the attribute if it is not null
            string tmpExact = reader.Value?.Trim();
            IsExactMatch = bool.Parse(tmpExact);

            //Then move past the start element
            reader.ReadStartElement("synonym");

            //This should not do anything as we are already in the content
            reader.MoveToContent(); //Move to Value

            //Get out the inner text
            Text = reader.ReadContentAsString();

            //Finish reading and move to the next element... which
            //means we are done here.            
            reader.ReadEndElement();

        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException("Serialization is not supported for this type.");
        }
    }
}