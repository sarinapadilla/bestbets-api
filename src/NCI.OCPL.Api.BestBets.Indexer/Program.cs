using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nest;
using Elasticsearch.Net;

using NCI.OCPL.Api.BestBets.Indexer.Services;
using NCI.OCPL.Services.CDE.PublishedContentListing;
using Microsoft.Extensions.Options;

namespace NCI.OCPL.Api.BestBets.Indexer
{
    public class Program
    {
        /// <summary>
        /// This class will actually handle the real work of the program.
        /// Encapsulating this to make a more logical separation without having
        /// to actually create another file...
        /// </summary>
        private class Worker {

            /// <summary>
            /// Creates a new instance of "the program"
            /// </summary>
            public Worker()
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                Configuration = builder.Build();

                ServiceCollection services = new ServiceCollection();

                this.ConfigureServices(services);

                ServiceProvider = services.BuildServiceProvider();
            }

            public IConfigurationRoot Configuration { get; }

            public IServiceProvider ServiceProvider { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {

                services.AddLogging();
                services.AddOptions();

                services.Configure<CGBestBetsDisplayServiceOptions>(Configuration.GetSection("CGBestBetsDisplayService"));
                services.Configure<ESBBIndexerServiceOptions>(Configuration.GetSection("ESBBIndexerService"));
                services.Configure<ElasticSearchOptions>(Configuration.GetSection("Elasticsearch"));
                services.Configure<PublishedContentListingServiceOptions>(Configuration.GetSection("CDEPubContentListingService"));

                services.AddSingleton<ConfiguredElasticClientFactory, ConfiguredElasticClientFactory>();
                services.AddTransient<IElasticClient>(p => p.GetRequiredService<ConfiguredElasticClientFactory>().GetInstance());

                services.AddSingleton<HttpClient, HttpClient>();

                services.AddTransient<IPublishedContentListingService, CDEPubContentListingService>();

               
                //Add others
                services.AddSingleton<IBBIndexerService, ESBBIndexerService>();
            }

            /// <summary>
            /// This actually runs the program.
            /// </summary>
            public void Run()
            {
                try
                {
                    //This needs to be pulled into its own testable method.


                    //Get the IBBIndexingService reference
                    IBBIndexerService indexer = ServiceProvider.GetRequiredService<IBBIndexerService>();

                    //Create new index
                    string indexName = indexer.CreateTimeStampedIndex();

                    //Fetch bunch of BB
                    IPublishedContentListing bestBetsList = GetBestBetsList();

                    //Index the BB
                    int numBBIndexed = indexer.IndexBestBetsMatches(indexName, GetBestBetMatchesFromListing(bestBetsList));

                    //Test the collection?

                    //Optimize
                    bool didOptimize = indexer.OptimizeIndex(indexName);

                    //Swap Alias
                    bool didSwap = indexer.MakeIndexCurrentAlias(indexName);

                    Console.WriteLine("Indexed " + numBBIndexed.ToString());
                    //Clean Up old.
                }
                catch (Exception ex)
                {
                    throw new Exception("We should really have a logger for this error.", ex);
                }
            }

            private IPublishedContentListing GetBestBetsList()
            {
                IPublishedContentListingService pcService = ServiceProvider.GetRequiredService<IPublishedContentListingService>();
                return pcService.GetItemsForPath("BestBets", "/");
            }

            /// <summary>
            /// Gets the best bet matches from a IPublishedContentListing.
            /// </summary>
            /// <param name="bestBetsList">The best bets list.</param>
            /// <returns>IEnumerator&lt;BestBetsMatch&gt;.</returns>
            private IEnumerable<BestBetsMatch> GetBestBetMatchesFromListing(IPublishedContentListing bestBetsList)
            {
                IPublishedContentListingService pcService = ServiceProvider.GetRequiredService<IPublishedContentListingService>();

                foreach (IPublishedContentInfo item in bestBetsList.Files)
                {
                    //Fetch Item
                    CancerGovBestBet bbCategory = pcService.GetPublishedFile<CancerGovBestBet>(item.FullWebPath);

                    //Do we really need a mapper that does this per category?  
                    BestBetSynonymMapper mapper = new BestBetSynonymMapper(bbCategory);
                    foreach (BestBetsMatch match in mapper)
                    {
                        yield return match;
                    }
                }
            }
        }


        /// <summary>
        /// Main entry point into this program.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Worker program = new Worker();
            program.Run();
           
            Console.Read();
        }

    }
}
