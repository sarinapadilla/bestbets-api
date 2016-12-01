using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Nest;
using Elasticsearch.Net;

using NCI.OCPL.Api.BestBets.Indexer.Services;
using NCI.OCPL.Services.CDE.PublishedContentListing;

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
                services.Configure<ESBBIndexerService>(Configuration.GetSection("ESBBIndexerService"));
                services.Configure<CDEPubContentListingService>(Configuration.GetSection("CDEPubContentListingService"));

                services.AddTransient<IElasticClient>(p =>
                {
                    // Get the ElasticSearch credentials.
                    string username = Configuration["Elasticsearch:Userid"];
                    string password = Configuration["Elasticsearch:Password"];

                    //Get the ElasticSearch servers that we will be connecting to.
                    List<Uri> uris = GetServerUriList();

                    // Create the connection pool, the SniffingConnectionPool will 
                    // keep tabs on the health of the servers in the cluster and
                    // probe them to ensure they are healthy.  This is how we handle
                    // redundancy and load balancing.
                    var connectionPool = new SniffingConnectionPool(uris);

                    //Return a new instance of an ElasticClient with our settings
                    ConnectionSettings settings = new ConnectionSettings(connectionPool);

                    //Let's only try and use credentials if the username is set.
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        settings.BasicAuthentication(username, password);
                    }

                    return new ElasticClient(settings);
                });

                //Add others
                services.AddSingleton<IBBIndexerService, ESBBIndexerService>();
                services.AddSingleton<IPublishedContentListingService, CDEPubContentListingService>();
            }

            /// <summary>
            /// Retrieves a list of Elasticsearch server URIs from the configuration's Elasticsearch:Servers setting. 
            /// </summary>
            /// <returns>Returns a list of one or more Uri objects representing the configured set of Elasticsearch servers</returns>
            /// <remarks>
            /// The configuration's Elasticsearch:Servers property is required to contain URIs for one or more Elasticsearch servers.
            /// Each URI must include a protocol (http or https), a server name, and optionally, a port number.
            /// Multiple URIs are separated by a comma.  (e.g. "https://fred:9200, https://george:9201, https://ginny:9202")
            /// 
            /// Throws ConfigurationException if no servers are configured.
            ///
            /// Throws UriFormatException if any of the configured server URIs are not formatted correctly.
            /// </remarks>
            private List<Uri> GetServerUriList()
            {
                List<Uri> uris = new List<Uri>();

                string serverList = Configuration["Elasticsearch:Servers"];
                if (!String.IsNullOrWhiteSpace(serverList))
                {
                    // Convert the list of servers into a list of Uris.
                    string[] names = serverList.Split(',');
                    uris.AddRange(names.Select(server => new Uri(server)));
                }
                else
                {
                    throw new Exception("No servers configured");
                }

                return uris;
            }

            /// <summary>
            /// This actually runs the program.
            /// </summary>
            public void Run()
            {
                //This needs to be pulled into its own testable method.


                //Get the IBBIndexingService reference
                IBBIndexerService indexer = ServiceProvider.GetRequiredService<IBBIndexerService>();

                //Create new index
                string indexName = indexer.CreateTimeStampedIndex();

                //Fetch bunch of BB
                IPublishedContentListing bestBetList = LoadBestBetList();

                //Index the BB

                //Test the collection?

                //Optimize

                //Swap Alias

                //Clean Up old.

            }

            private IPublishedContentListing LoadBestBetList()
            {
                IPublishedContentListingService pcService = ServiceProvider.GetRequiredService<IPublishedContentListingService>();
                return pcService.GetItemsForPath("BestBets", "/");
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
        }

    }
}
