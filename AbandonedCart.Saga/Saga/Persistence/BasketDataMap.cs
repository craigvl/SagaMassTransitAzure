using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magnum.Extensions;
using MassTransit.EntityFrameworkIntegration;

namespace Abt.Result.WebApi.Saga.Persistence
{
        public class BasketDataMap :  SagaClassMapping<BasketSaga>
        {
            public BasketDataMap()
            {
                Property(x => x.CurrentState)
                 .HasMaxLength(64);

                //Property(x => x.Updated).IsOptional();

            Property(x => x.UserEmail)
                    .HasMaxLength(256);

                Property(x => x.ExpirationId);
                Property(x => x.OrderId);
                Property(x => x.CustomerId);
            }
        }
    
}
