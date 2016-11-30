using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Setup services for DI?

            ServiceCollection services = new ServiceCollection();

            services.AddLogging();
            services.AddOptions();

            var serviceProvider = services.BuildServiceProvider();

            
            
            //Create new index

            //Fetch bunch of BB

            //Index the BB

            //Test the collection?

            //Optimize

            //Swap Alias

            //Clean Up old.
        }
    }
}
