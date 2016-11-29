using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace NCI.OCPL.Api.BestBets.Tests
{

    /// <summary>
    /// A IEqualityComparer for IBestBetDisplay
    /// </summary>
    public class IBestBetDisplayComparer : IEqualityComparer<IBestBetDisplay>
    {
            public bool Equals(IBestBetDisplay x, IBestBetDisplay y)
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
                    && x.Weight == y.Weight
                    && x.HTML?.Trim() == y.HTML?.Trim(); //Special case where we will trim on comparison
                
                return isEqual;                
            }

            public int GetHashCode(IBestBetDisplay obj)
            {
                int hash = 0;
                hash ^= 
                    obj.ID.GetHashCode()
                    ^ obj.Name.GetHashCode()
                    ^ obj.Weight.GetHashCode()
                    ^ (obj.HTML?.Trim()).GetHashCode(); //Explicitly trim HTML as leading and trailing WS does not matter

                return hash;
            }
        
    }
}