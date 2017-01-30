using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Indexer.Services
{
    public class ESBBIndexerServiceOptions :
        CGBBIndexOptions
    {
        // Wraps CGBBIndexOptions so we only need a single configuaration
        // unit for their overlapping data.
    }
}
