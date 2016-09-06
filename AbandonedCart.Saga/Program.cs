using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abt.Result.WebApi.BasketStore;
using Abt.Result.WebApi.Saga;
using Abt.Result.WebApi.Services.BasketManager;
using Microsoft.Owin.Hosting;
using Ninject;

namespace Abt.Result.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting ... ");


            //var store = new MemoryBasketStore();
            //var basketManager = new BasketManager(
            //    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\DOM\AzureTopicPOC\Abt.Result.WebApi\BasketSagaData.mdf;Integrated Security=True", 
            //    "RootManageSharedAccessKey", 
            //    "rj5Ga/C65/QltcYxBOIJrKdT26JgX91NnjixGW1hsyw=", 
            //    "dominosbasket");


            var url = ConfigurationManager.AppSettings.Get("owin:url");
            using (WebApp.Start<Startup>(url))
            {

                Console.WriteLine("Web Server running on {0}", url);

                //Console.ReadLine();
                var key = ConsoleKey.A;
                Console.Write("Please enter customer action (1=start new basket, 2=add item to basket, 3:complete payment):");
                while (key != ConsoleKey.Escape)
                {
                    key = Console.ReadKey().Key;

                    switch (key)
                    {
                        case ConsoleKey.D1:
                            {
                                Console.WriteLine();
                                Console.Write("New Order -> enter Customer email:");
                                var email = Console.ReadLine();
                                //  basketManager.CreateOrder(email);
                                break;
                            }
                        case ConsoleKey.D2:
                            {
                                Console.WriteLine();
                                Console.Write("Change Current Order -> Customer email:");
                                var email = Console.ReadLine();
                                //      basketManager.AddItem(email);
                                break;
                            }
                        case ConsoleKey.D3:
                            {
                                Console.WriteLine();
                                Console.Write("Pay order -> Customer email:");
                                var email = Console.ReadLine();
                                //           basketManager.CompleteOrder(email);
                                break;
                            }

                        case ConsoleKey.D0:
                            {
                                Console.WriteLine();
                                Console.Write("===========================");
                                //foreach (var row in basketManager.GetCurrentBaskets())
                                //{
                                //    Console.WriteLine($"{row.Username} {row.Email} -> {row.CurrentState}");
                                //   }
                                break;

                            }
                    }
                }

            }
        }
    }
}
