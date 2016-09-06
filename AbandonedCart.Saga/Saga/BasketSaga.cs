using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;

namespace Abt.Result.WebApi.Saga
{
    
    // Persisted content / data
    public class BasketSaga :
        SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid? ExpirationId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
