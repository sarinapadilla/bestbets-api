using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer.Tests.Util
{
    /// <summary>
    /// Helper class to compare URIs, a bit more flexible than Uri.Comparer for comparing
    /// query string parameters.
    /// </summary>
    public class UriComparer : IEqualityComparer<Uri>
    {
        public bool Equals(Uri x, Uri y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            // Compare paths.
            string xPath = x.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            string yPath = y.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            if (xPath != yPath)
                return false;

            return AreQueryParamsEquivalent(x, y);
        }

        /// <summary>
        /// Compares query parameters, regardless of order.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool AreQueryParamsEquivalent(Uri x, Uri y)
        {
            string rawXParams = x.GetComponents(UriComponents.Query, UriFormat.Unescaped);
            string rawYParams = y.GetComponents(UriComponents.Query, UriFormat.Unescaped);

            // fast checks for presence/absence of query params.
            if (rawXParams == null & rawYParams == null)
                return true;
            if ((rawXParams == null && rawYParams != null)
                || (rawXParams != null && rawYParams == null))
                return false;

            string[] xParams = rawXParams.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            string[] yParams = rawYParams.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            // List sizes.
            if (xParams.Length != yParams.Length)
                return false;

            // Do all of the parameters in the first list appear in the second?
            foreach (string param in xParams)
            {
                if (!yParams.Contains(param))
                    return false;
            }

            return true;
        }


        public int GetHashCode(Uri obj)
        {
            throw new NotImplementedException();
        }
    }
}
