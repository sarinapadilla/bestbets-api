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
using NCI.OCPL.Api.BestBets.Services;

namespace NCI.OCPL.Api.BestBets.Indexer
{
    public class Program
    {

        public Program()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            ServiceCollection services = new ServiceCollection();

            this.ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            //Setup the logger
            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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
            services.Configure<CGBBIndexOptions>(Configuration.GetSection("CGBestBetsIndex"));
            services.Configure<ElasticSearchOptions>(Configuration.GetSection("Elasticsearch"));
            services.Configure<PublishedContentListingServiceOptions>(Configuration.GetSection("CDEPubContentListingService"));

            services.AddSingleton<ConfiguredElasticClientFactory, ConfiguredElasticClientFactory>();
            services.AddTransient<IElasticClient>(p => p.GetRequiredService<ConfiguredElasticClientFactory>().GetInstance());

            services.AddSingleton<HttpClient, HttpClient>();
            services.AddTransient<ITokenAnalyzerService, ESTokenAnalyzerService>();

            services.AddTransient<IPublishedContentListingService, CDEPubContentListingService>();


            //Add others
            services.AddSingleton<IBBIndexerService, ESBBIndexerService>();

            //Add the indexer, this is so all those wonderful services above can be 
            //injected into a new BestBetsIndexer instance.
            services.AddTransient<BestBetsIndexer, BestBetsIndexer>();
        }

        private void Run()
        {
            BestBetsIndexer indexer = ServiceProvider.GetRequiredService<BestBetsIndexer>();
            indexer.Run();
        }

        /// <summary>
        /// Main entry point into this program.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();


            Console.Read();
        }

    }
}
