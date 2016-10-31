using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace NCI.OCPL.Services.BestBets.Tests.Tests
{

    /// <summary>
    /// Comparer for IBestBetSynonym
    /// </summary>
    public class IBestBetSynonymComparer : IEqualityComparer<IBestBetSynonym>
    {
        public bool Equals(IBestBetSynonym x, IBestBetSynonym y)
        {

            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.
            if (x == null && y== null) 
            {
                return true;
            } 
            else if (x == null || y == null)
            {
                return false;
            }

            bool isEqual =
                x.IsExactMatch == y.IsExactMatch
                && x.Text == y.Text;
                                
            return isEqual;
        }

        public int GetHashCode(IBestBetSynonym obj)
        {
            throw new NotImplementedException();
        }
    }


    public class IBestBetCategoryComparer : IEqualityComparer<IBestBetCategory>
    {
            public bool Equals(IBestBetCategory x, IBestBetCategory y)
            {
                // If the items are both null, or if one or the other is null, return 
                // the correct response right away.
                if (x == null && y== null) 
                {
                    return true;
                } 
                else if (x == null || y == null)
                {
                    return false;
                }                

                bool isEqual = 
                    x.Name == y.Name 
                    && x.ID == y.ID
                    && x.Display == y.Display
                    && x.Weight == y.Weight
                    && x.IsExactMatch == y.IsExactMatch
                    && x.Language == y.Language
                    && AreSynonymListsEqual(x.ExcludeSynonyms, y.ExcludeSynonyms)
                    && AreSynonymListsEqual(x.IncludeSynonyms, y.IncludeSynonyms);
                    //HTML
                
                return isEqual;                
            }

            private bool AreSynonymListsEqual(IBestBetSynonym[] x, IBestBetSynonym[] y) {
                // If the items are both null, or if one or the other is null, return 
                // the correct response right away.
                
                if (x == null && y== null) 
                {
                    return true;
                } 
                else if (x == null || y == null)
                {
                    return false;
                }                

                //Generate a set of those values that are not in both lists.
                //if this is not 0, then there is an error.
                var diffxy = x.Except(y, new IBestBetSynonymComparer());

                return diffxy.Count() == 0;
            }

            public int GetHashCode(IBestBetCategory obj)
            {
                throw new NotImplementedException();
            }
        
    }
}