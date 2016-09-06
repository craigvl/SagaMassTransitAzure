using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abt.Result.WebApi.Config;
using Abt.Result.WebApi.Infrastructure;
using Nancy.Owin;
using Ninject;
using Owin;

namespace Abt.Result.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IKernel container = new StandardKernel(new NinjectModuleConfig());

            var nancyConfig = new NancyOptions() {Bootstrapper = new NinjectNancyAppBootstrapper(container)};
            
            //app.UseNancy();
            app.UseNancy(nancyConfig);
        }
    }
}
