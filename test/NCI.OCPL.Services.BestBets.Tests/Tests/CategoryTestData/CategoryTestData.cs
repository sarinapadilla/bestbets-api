using System.Collections.Generic;

namespace NCI.OCPL.Services.BestBets.Tests.CategoryTestData
{

    /// <summary>
    /// Helper base class for CancerGovBestBet objects that need to be 
    /// used as ClassData in Theorys 
    /// </summary>
    public abstract class BaseCategoryTestData 
    {
        /// <summary>
        /// Used for getting theory data.
        /// </summary>
        public IEnumerable<object> IntTestData => new object[] { TestFilePath, ExpectedData };

        /// <summary>
        /// Gets the file name containing the actual test data.
        /// </summary>
        /// <returns></returns>
        public abstract string TestFilePath { get; }

        /// <summary>
        /// Gets an instance of the Expected Data object
        /// </summary>
        /// <returns></returns>
        public CancerGovBestBet ExpectedData { 
            get 
            {
                CancerGovBestBet bb = CoreData;

                List<CancerGovBestBetSynonym> incSyn = new List<CancerGovBestBetSynonym>();
                //Add inlcude synonyms that are not exact match
                foreach (string synName in IncludeSyn) {
                    incSyn.Add(
                        new CancerGovBestBetSynonym(){
                            IsExactMatch = false,
                            Text = synName
                        }
                    );
                }

                //Add inlcude synonyms that ARE exact match
                foreach (string synName in IncludeSynExact) {
                    incSyn.Add(
                        new CancerGovBestBetSynonym(){
                            IsExactMatch = true,
                            Text = synName
                        }
                    );
                }

                List<CancerGovBestBetSynonym> exSyn = new List<CancerGovBestBetSynonym>();
                //Add inlcude synonyms that are not exact match
                foreach (string synName in ExcludeSyn) {
                    exSyn.Add(
                        new CancerGovBestBetSynonym(){
                            IsExactMatch = false,
                            Text = synName
                        }
                    );
                }

                //Add inlcude synonyms that ARE exact match
                foreach (string synName in ExcludeSyn) {
                    exSyn.Add(
                        new CancerGovBestBetSynonym(){
                            IsExactMatch = true,
                            Text = synName
                        }
                    );
                }

                bb.IncludeSynonyms = incSyn.ToArray();
                bb.ExcludeSynonyms = exSyn.ToArray();

                return bb;
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
        protected virtual string[] IncludeSyn => new string[] {};

        /// <summary>
        /// A list of the exact include synonyms
        /// </summary>
        protected virtual string[] IncludeSynExact => new string[] {};

        /// <summary>
        /// A list of the non-exact exclude synonyms
        /// </summary>
        protected virtual string[] ExcludeSyn => new string[] {};

        /// <summary>
        /// A list of the exact exclude synonyms
        /// </summary>
        protected virtual string[] ExcludeSynExact => new string[] {};        
    }
}