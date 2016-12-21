using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Nest;
using Elasticsearch.Net;

namespace NCI.OCPL.Api.BestBets.Controllers
{
    [Route("[controller]")]
    public class BestBetsController : Controller
    {
        private readonly IBestBetsMatchService _matchService;
        private readonly IBestBetsDisplayService _displayService;
        private readonly ILogger<BestBetsController> _logger;

        /// <summary>
        /// Represents a IBestBetDisplay as returned by the get method.
        /// </summary>
        private class BestBetAPIGetResult : IBestBetDisplay
        {
            public string HTML { get; set; }
            public string ID { get; set; }

            public string Name { get; set; }

            public int Weight { get; set; }
        }

        /// <summary>
        /// Creates a new instance of a BestBetsController
        /// </summary>
        /// <param name="matchService">An IBestBetsMatchService for getting matched best bets categories</param>
        /// <param name="displayService">An IBestBetsDisplayService for getting the display HTML for matched categories</param>
        /// <param name="logger">A logger</param>
        public BestBetsController(
            IBestBetsMatchService matchService,
            IBestBetsDisplayService displayService,
            ILogger<BestBetsController> logger
        ) 
        {
            this._matchService = matchService;
            this._displayService = displayService;
            this._logger = logger;
        }


        // GET api/values/5
        [HttpGet("{language}/{term}")]
        public IBestBetDisplay[] Get(string language, string term)
        {
            if (String.IsNullOrWhiteSpace(language))
                throw new APIErrorException(400, "You must supply a language and search term");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(404, "Unsupported Language. Please try either 'en' or 'es'");
            
            if (String.IsNullOrWhiteSpace(term))
                throw new APIErrorException(400, "You must supply a search term");

            // Step 1. Remove Punctuation
            string cleanedTerm = CleanTerm(term);

            string[] categoryIDs = _matchService.GetMatches(language.ToLower(), cleanedTerm);
            
            List<IBestBetDisplay> displayItems = new List<IBestBetDisplay>();            

            //Now get categories for ID.
            foreach (string categoryID in categoryIDs)
            {
                IBestBetDisplay item = _displayService.GetBestBetForDisplay(categoryID);
                displayItems.Add(new BestBetAPIGetResult()
                {
                    ID = item.ID,
                    Name = item.Name,
                    Weight = item.Weight,
                    HTML = item.HTML
                });
            }

            return displayItems.ToArray();
        }

        //TODO: Move CleanTerm to a shared class for use by the indexer as well.

        /// <summary>
        /// This will remove punctuation from a term
        /// </summary>
        /// <param name="term">The term to clean</param>
        /// <returns>The cleaned term</returns>
        private string CleanTerm(string term)
        {
            var sb = new StringBuilder();
            foreach (char c in term)
            {
                if (!char.IsPunctuation(c))
                    sb.Append(c);
            }
            term = sb.ToString();

            //TODO: Verify that this list is not actually a duplicate of 
            return System.Text.RegularExpressions.Regex.Replace(term, "[-':;\"\\./]", "");
        }

    }
}
