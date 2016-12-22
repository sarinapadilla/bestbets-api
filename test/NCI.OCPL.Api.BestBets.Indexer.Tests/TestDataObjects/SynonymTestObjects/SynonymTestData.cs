using NCI.OCPL.Utils.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests.CategoryTestData
{
    /// <summary>
    /// Class that hosts test data for testing BestBetSynonymMapper
    /// </summary>
    public class SynonymTestData
    {

        private CancerGovBestBet _coreData = null;

        /// <summary>
        /// Gets and sets the name of the XML file to be used for comparing?
        /// </summary>
        /// <value>The test file path.</value>
        [JsonProperty(Required = Required.Always)]
        public string TestFilePath { get; set; }

        #region Best Bet Category Data Items

        /// <summary>
        /// Gets or sets the name of the category for this Best Bet Match
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content ID of the category of this match
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the HTML for display for this category
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string HTML { get; set; }

        /// <summary>
        /// Gets the weight of this category to determine ordering on display
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int Weight { get; set; }

        /// <summary>
        /// Is this Best Bet's Category Name an Exact Match? 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool IsExactMatch { get; set; }

        /// <summary>
        /// Gets or sets the language of this category
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Language { get; set; }

        //No clue what this is...
        [JsonProperty(Required = Required.Always)]
        public bool Display { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the category name's token count.
        /// </summary>
        /// <value>The category token count.</value>
        [JsonProperty(Required = Required.Always)]
        public int CategoryTokenCount { get; set; }

        /// <summary>
        /// A list of the non-exact include synonyms
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public Syn[] IncludeSyn { get; set; } = new Syn[] { };

        /// <summary>
        /// A list of the exact include synonyms
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public Syn[] IncludeSynExact { get; set; } = new Syn[] { };

        /// <summary>
        /// A list of the non-exact exclude synonyms
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public Syn[] ExcludeSyn { get; set; } = new Syn[] { };

        /// <summary>
        /// A list of the exact exclude synonyms
        /// </summary>
        [JsonProperty(Required = Required.DisallowNull)]
        public Syn[] ExcludeSynExact { get; set; } = new Syn[] { };


        /// <summary>
        /// Gets the core data for the category.  I.E. everything except
        /// the include and exclude synonym arrays
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public CancerGovBestBet CoreData
        {
            get
            {
                if (_coreData == null)
                {
                    _coreData = new CancerGovBestBet()
                    {
                        Name = this.Name,
                        ID = this.ID,
                        Display = this.Display,
                        HTML = this.HTML,
                        IsExactMatch = this.IsExactMatch,
                        Language = this.Language,
                        Weight = this.Weight,
                        IncludeSynonyms = GetSynonymList(IncludeSyn, IncludeSynExact),
                        ExcludeSynonyms = GetSynonymList(ExcludeSyn, ExcludeSynExact)
                    };
                }

                return _coreData;
            }
        }

        /// <summary>
        /// Gets a List of CancerGovBestBetSynonym object from two lists of Syn objects.
        /// </summary>
        /// <param name="NotExactSyn">The synonyms that are NOT exact match.</param>
        /// <param name="ExactSyn">The synonyms that are exact matches.</param>
        /// <returns>CancerGovBestBetSynonym[].</returns>
        private CancerGovBestBetSynonym[] GetSynonymList(Syn[] NotExactSyn, Syn[] ExactSyn)
        {
            return
                //Get Includes
                (
                    from syn in NotExactSyn
                    select new CancerGovBestBetSynonym()
                    {
                        Text = syn.Term,
                        IsExactMatch = false,
                    }
                )
                //Add Exact Matches
                .Union(
                    from syn in ExactSyn
                    select new CancerGovBestBetSynonym()
                    {
                        Text = syn.Term,
                        IsExactMatch = true,
                    }
                )
                ///Make it an array
                .ToArray();
        }

        /// <summary>
        /// Gets a collection of expected BestBetMatches for this category based on the data.
        /// </summary>
        /// <value>The expected matches.</value>
        [JsonIgnore]
        public IEnumerable<BestBetsMatch> ExpectedMatches
        {
            get
            {
                // Create a IEnumerable<> for the main category.
                var selfList =
                    from item in new Array[1] 
                    select new { Name = CoreData.Name, IsExact = CoreData.IsExactMatch, IsNegated = false, TokenCount = this.CategoryTokenCount };

                var IncludeNotExact =
                    from syn in IncludeSyn
                    select new { Name = syn.Term, IsExact = false, IsNegated = false, TokenCount = syn.TokenCount };
                var IncludeExact =
                    from syn in IncludeSynExact
                    select new { Name = syn.Term, IsExact = true, IsNegated = false, TokenCount = syn.TokenCount };
                var ExcludeNotExact =
                    from syn in ExcludeSyn
                    select new { Name = syn.Term, IsExact = false, IsNegated = true, TokenCount = syn.TokenCount };
                var ExcludeExact =
                    from syn in ExcludeSynExact
                    select new { Name = syn.Term, IsExact = true, IsNegated = true, TokenCount = syn.TokenCount };

                IEnumerable<BestBetsMatch> data =
                    from match in
                        selfList
                        .Union(IncludeNotExact)
                        .Union(IncludeExact)
                        .Union(ExcludeNotExact)
                        .Union(ExcludeExact)
                    select new BestBetsMatch()
                    {
                        Synonym = match.Name,
                        IsExact = match.IsExact,
                        IsNegated = match.IsNegated,

                        Category = CoreData.Name,
                        ContentID = CoreData.ID,
                        Language = CoreData.Language,
                        TokenCount = match.TokenCount
                    };

                foreach (BestBetsMatch  match in data)
                {
                    yield return match;
                }
            }
        }

        public int GetTokenCount(string term)
        {
            if (this.Name == term)
                return this.CategoryTokenCount;

            foreach(Syn[] synList in new Syn[][] { IncludeSyn, ExcludeSyn, IncludeSynExact, ExcludeSynExact })
            {
                foreach (Syn syn in synList)
                {
                    if (syn.Term == term)
                        return syn.TokenCount;
                }
            }

            throw new Exception("Token Count not found for term: " + term);
        }

        /// <summary>
        /// Represents a BB Synonym for testing
        /// </summary>
        public class Syn
        {
            /// <summary>
            /// Gets or sets the term.
            /// </summary>
            /// <value>The term.</value>
            public string Term { get; set; }

            /// <summary>
            /// Gets or sets the number of tokens an analyzer would return
            /// </summary>
            /// <value>The token count.</value>
            public int TokenCount { get; set; }
        }

        /// <summary>
        /// Loads a SynonymTestData object based on a JSON file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>SynonymTestData.</returns>
        public static SynonymTestData LoadTestData(string path)
        {
            

            string content = File.ReadAllText(
                TestingTools.GetPathToTestFile("SynonymTestData/" + path)
            );

            return JsonConvert.DeserializeObject<SynonymTestData>(content);
        }

    }
}
