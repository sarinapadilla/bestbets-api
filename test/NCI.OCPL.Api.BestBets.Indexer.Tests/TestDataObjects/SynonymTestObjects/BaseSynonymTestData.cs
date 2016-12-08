using System;
using System.Collections.Generic;
using System.Linq;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests.CategoryTestData
{
    public abstract class BaseSynonymTestData
    {
        public abstract string TestFilePath { get; }

        public IEnumerable<BestBetsMatch> ExpectedMatches
        {
            get
            {
                // Create a IEnumerable<> for the main category.
                var selfList =
                    from item in new Array[1] 
                    select new { Name = CoreData.Name, IsExact = CoreData.IsExactMatch, IsNegated = false };

                var IncludeNotExact =
                    from name in IncludeSyn
                    select new { Name = name, IsExact = false, IsNegated = false };
                var IncludeExact =
                    from name in IncludeSynExact
                    select new { Name = name, IsExact = true, IsNegated = false };
                var ExcludeNotExact =
                    from name in ExcludeSyn
                    select new { Name = name, IsExact = false, IsNegated = true };
                var ExcludeExact =
                    from name in ExcludeSynExact
                    select new { Name = name, IsExact = true, IsNegated = true };

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
                        TokenCount = 1
                    };

                foreach (BestBetsMatch  match in data)
                {
                    yield return match;
                }
            }
        }

        /// <summary>
        /// Gets the core data for the category.  I.E. everything except
        /// the include and exclude synonym arrays
        /// </summary>
        /// <returns></returns>
        protected abstract CancerGovBestBet CoreData { get; }

        /// <summary>
        /// A list of the non-exact include synonyms
        /// </summary>
        protected virtual string[] IncludeSyn => new string[] { };

        /// <summary>
        /// A list of the exact include synonyms
        /// </summary>
        protected virtual string[] IncludeSynExact => new string[] { };

        /// <summary>
        /// A list of the non-exact exclude synonyms
        /// </summary>
        protected virtual string[] ExcludeSyn => new string[] { };

        /// <summary>
        /// A list of the exact exclude synonyms
        /// </summary>
        protected virtual string[] ExcludeSynExact => new string[] { };

    }
}
