using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NCI.OCPL.Api.BestBets 
{
    /// <summary>
    /// Represents a Best Best for Display
    /// (Example URL https://www.cancer.gov/PublishedContent/BestBets/36012.xml)
    /// </summary>
    [System.Xml.Serialization.XmlRootAttribute("BestBetsCategory", Namespace = "http://www.example.org/CDESchema", IsNullable = false)]
    public class CancerGovBestBet : IXmlSerializable, IBestBetCategory
    {       
        /// <summary>
        /// Gets or sets the name of the category for this Best Bet Match
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content ID of the category of this match
        /// </summary>
        public string ID {get; set;}

        /// <summary>
        /// Gets or sets the HTML for display for this category
        /// </summary>
        public string HTML { get; set; }

        /// <summary>
        /// Gets the weight of this category to determine ordering on display
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Is this Best Bet's Category Name an Exact Match? 
        /// </summary>
        public bool IsExactMatch { get; set; }

        /// <summary>
        /// Gets or sets the language of this category
        /// </summary>
        public string Language { get; set; }

        //No clue what this is...
        public bool Display { get; set; }        

        /// <summary>
        /// Gets all of the Synonyms that if matched would INCLUDE this category in results
        /// </summary>
        public CancerGovBestBetSynonym[] IncludeSynonyms { get; set; }

        //Explicit Interface method
        IBestBetSynonym[] IBestBetCategory.IncludeSynonyms 
        { 
            get 
            {
                return this.IncludeSynonyms;
            }
        }

        /// <summary>
        /// Gets all of the Synonyms that if matched would EXCLUDE this category in results
        /// </summary>
        public CancerGovBestBetSynonym[] ExcludeSynonyms { get; set; } 

        //Explicit Interface method
        IBestBetSynonym[] IBestBetCategory.ExcludeSynonyms 
        { 
            get 
            {
                return this.ExcludeSynonyms;
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Helper method to read content as a tring with whitespace and newlines
        /// removed from the beginning and end of the string.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string ReadElementAsTrimmedString(XmlReader reader) 
        {
            string tmp = reader.ReadElementContentAsString();
            if (tmp != null)
                tmp = tmp.Trim();

            return tmp;
        }

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if ((reader.LocalName == "BestBetsCategory") && (reader.IsStartElement() == false)) { reader.Read(); break; }

                switch (reader.LocalName) 
                {
                    case "CategoryId" : {
                        ID = ReadElementAsTrimmedString(reader);
                        break;
                    }
                    case "CategoryName" : {
                        Name = System.Net.WebUtility.HtmlDecode(ReadElementAsTrimmedString(reader));
                        break;
                    }
                    case "CategoryWeight" : {
                        Weight = reader.ReadElementContentAsInt();
                        break; 
                    }
                    case "IsExactMatch" : {
                        IsExactMatch = reader.ReadElementContentAsBoolean();
                        break; 
                    }
                    case "Language" : {
                        Language = ReadElementAsTrimmedString(reader);
                        break; 
                    } 
                    case "Display" : {
                        Display = reader.ReadElementContentAsBoolean();
                        break; 
                    }
                    case "CategoryDisplay" : {
                        HTML = ReadElementAsTrimmedString(reader);
                        break; 
                    }
                    case "IncludeSynonyms" : {
                        IncludeSynonyms = ReadSynonyms(reader);

                        break; 
                    }
                    case "ExcludeSynonyms" : {
                        ExcludeSynonyms = ReadSynonyms(reader);

                        break; 
                    }
                }
            }
        }

        /// <summary>
        /// Helper to read a collection of Synonyms.  Used by both IncludeSynonyms and ExcludeSynonyms
        /// </summary>
        /// <param name="reader">An XmlReader that is positioned to the collections StartElement</param>
        /// <returns>An array of CancerGovBestBetSynonym objects</returns>
        private CancerGovBestBetSynonym[] ReadSynonyms(XmlReader reader) {
            List<CancerGovBestBetSynonym> synList = new List<CancerGovBestBetSynonym>();

            string arrayName = reader.LocalName;

            XmlSerializer synSerializer = new XmlSerializer(typeof (CancerGovBestBetSynonym));

            //Swallow the wrapping element that surrounds the contents.
            //E.G. <IncludeSynonyms>
            reader.ReadStartElement(); 

            //Keep reading while we gots synonyms
            while (reader.LocalName == "synonym") 
            {
                synList.Add((CancerGovBestBetSynonym)synSerializer.Deserialize(reader));
                
                // It looks like there is a bunch of whitespace generated in the XML
                // when the IsExactMatch attribute is true.  We will fastforward to the next
                // element. (which can be an end element too e.g. </IncludeSynonyms>)  
                if (reader.NodeType != XmlNodeType.Element) {
                    reader.MoveToContent();
                }
            }

            //Now we need to swallow the end element.
            //First fast forward to the end of this grouping
            while (! ((reader.LocalName == arrayName) && (reader.NodeType == XmlNodeType.EndElement))) {
                reader.Read();
            }

            //Second, read the end element
            reader.ReadEndElement();

            return synList.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException("Serialization is not supported by this type.");
        }         
    }    
}