using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Tests.Util
{
    public class BestBetsMatchComparer : IEqualityComparer<BestBetsMatch>
    {
        public bool Equals(BestBetsMatch x, BestBetsMatch y)
        {
            return x.Category == y.Category
                && x.ContentID == y.ContentID
                && x.Synonym == y.Synonym
                && x.Language == y.Language
                && x.IsNegated == y.IsNegated
                && x.IsExact == y.IsExact;

                // TODO: Reinstate check for TokenCount
                //&& x.TokenCount == y.TokenCount;
        }

        public int GetHashCode(BestBetsMatch obj)
        {
            throw new NotImplementedException();
        }
    }
}
