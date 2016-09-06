using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;

namespace Abt.Result.WebApi.Saga
{
    public interface IOrderStartedEvent
    {
         DateTime Timestamp { get; }
         string UserName { get; }
         string Email { get; }
    }

    public interface IBasketContentChanged
    {
        Guid OrderId { get; }
        Guid BasketId { get; }
        DateTime Timestamp { get; }
        string Email { get; }
    }
    public interface IOrderCompletedEvent
    {
        Guid OrderId { get; }
        Guid BasketId { get; }
        DateTime Timestamp { get; }
        string Email { get; }
    }

    public interface IBasketExpiredEvent
    {      
        Guid BasketId { get; }
        Guid? CustomerId { get; }
        string Email { get; }
    }

    public interface IBasketRemovedEvent
    {
        Guid BasketId { get; }
        string Email { get; }
        Guid? CustomerId { get; }
    }




}
