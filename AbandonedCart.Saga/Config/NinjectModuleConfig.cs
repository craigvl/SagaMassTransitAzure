
using Abt.Result.WebApi.BasketStore;
using Abt.Result.WebApi.Services.BasketManager;
using Ninject.Modules;

namespace Abt.Result.WebApi.Config
{
    public class NinjectModuleConfig : NinjectModule
    {
        public override void Load()
        {

            Bind<BasketManager>().ToConstant<BasketManager>(new BasketManager(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\DOM\AzureTopicPOC\Abt.Result.WebApi\BasketSagaData.mdf;Integrated Security=True", "RootManageSharedAccessKey", "rj5Ga/C65/QltcYxBOIJrKdT26JgX91NnjixGW1hsyw=", "dominosbasket")).InSingletonScope();





        }
    }
}

