using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NCI.OCPL.Services.BestBets 
{
    /// <summary>
    /// Represents a Best Best for Display
    /// (Example URL https://www.cancer.gov/PublishedContent/BestBets/36012.xml)
    /// </summary>

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.example.org/CDESchema")]
    [System.Xml.Serialization.XmlRootAttribute("BestBetsCategory", Namespace = "http://www.example.org/CDESchema", IsNullable = false)]
    public class CancerGovBestBet : IBestBetCategory
    {       
        /// <summary>
        /// Gets or sets the name of the category for this Best Bet Match
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "CategoryName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content ID of the category of this match
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "CategoryId")]
        public string ID {get; set;}

        /// <summary>
        /// Gets or sets the HTML for display for this category
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "CategoryDisplay")]
        public string HTML { get; set; }

        /// <summary>
        /// Gets the weight of this category to determine ordering on display
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified, ElementName = "CategoryWeight")]
        public int Weight { get; set; }

        /// <summary>
        /// Is this Best Bet's Category Name an Exact Match? 
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public bool IsExactMatch { get; set; }

        /// <summary>
        /// Gets or sets the language of this category
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Language { get; set; }

        //No clue what this is...
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public bool Display { get; set; }        

        /// <summary>
        /// Gets all of the Synonyms that if matched would INCLUDE this category in results
        /// </summary>
        [System.Xml.Serialization.XmlArray("IncludeSynonyms", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItem("synonym", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
        [System.Xml.Serialization.XmlArray("ExcludeSynonyms", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItem("synonym", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CancerGovBestBetSynonym[] ExcludeSynonyms { get; set; } 

        //Explicit Interface method
        IBestBetSynonym[] IBestBetCategory.ExcludeSynonyms 
        { 
            get 
            {
                return this.ExcludeSynonyms;
            }
        }


    }    
}