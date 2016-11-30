using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Services.CDE.PublishedContentListing
{
    public interface IPathListInfo
    {
        string DisplayName { get; set; }

        string Url { get; set; }
    }
}
