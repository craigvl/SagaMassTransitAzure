using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abt.Result.WebApi.BasketStore;
using Abt.Result.WebApi.Services.BasketManager;
using Magnum.Extensions;
using Nancy;
using Nancy.ModelBinding;

namespace Abt.Result.WebApi.Modules
{
    public class BasketsModule : NancyModule
    {
        public BasketsModule(BasketManager basketManager) 
        {

            //var basketManager = new BasketManager(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\DOM\AzureTopicPOC\Abt.Result.WebApi\BasketSagaData.mdf;Integrated Security=True", "RootManageSharedAccessKey", "rj5Ga/C65/QltcYxBOIJrKdT26JgX91NnjixGW1hsyw=", "dominosbasket");

            Post["/create"] = _ =>
            {
                var data = this.Request.Body.ReadJson<DynamicDictionary>();
                basketManager.CreateOrder(data["Email"]);

                return new Response() {  StatusCode = HttpStatusCode.Accepted };
            };

            Put["/add"] = args =>
            {
                var data = this.Request.Body.ReadJson<DynamicDictionary>();
                basketManager.CreateOrder(data["Email"]);
                return new Response() {StatusCode = HttpStatusCode.Accepted };
            };


            Delete["/complete"] = args =>
            {
                var data = this.Request.Body.ReadJson<DynamicDictionary>();
                basketManager.CompleteOrder(data["Email"]);
                return new Response() { StatusCode = HttpStatusCode.Accepted };
            };

            Get["/list"] = _ => Response.AsJson(basketManager.GetCurrentBaskets());
        }

    }
}
