using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abt.Result.WebApi.Saga;
using Automatonymous;
using MassTransit;
using MassTransit.Saga;

namespace Abt.Result.WebApi.Services.BasketManager
{
    public class BasketStateMachine :
    MassTransitStateMachine<BasketSaga>
    {
        // Events
        public Event<IOrderStartedEvent> OrderStarted { get; private set; }
        public Event<IOrderCompletedEvent> OrderCompleted { get; private set; }

        public Event<IBasketContentChanged> OrderChanged { get; private set; }

        public Event<IBasketRemovedEvent> OrderRemoved { get; private set; }

        // public Event<IOrderCompletedEvent> BasketContentChanged { get; private set; }


        public Schedule<BasketSaga, IBasketExpiredEvent> BasketExpiredTimeout { get; private set; }

        public State Active { get; private set; }

        public State Idle { get; private set; }

        public State Completed { get; private set; }

        public Guid CorrelationId { get; set; }

        public BasketStateMachine()
        {
            Console.Out.WriteLineAsync("BasketStateMachine init ... ");
            InstanceState(x => x.CurrentState);

            Event(() => OrderStarted, x => x.CorrelateBy(cart => cart.UserEmail, context => context.Message.Email)
                .SelectId(context => Guid.NewGuid()));


            // any activity found
            Event(() => OrderChanged, x => x.CorrelateBy(cart => cart.UserEmail, context => context.Message.Email));//x => x.CorrelateById(context => context.Message.BasketId));


            // payment done
            Event(() => OrderCompleted, x => x.CorrelateBy(cart => cart.UserEmail, context => context.Message.Email));//x => x.CorrelateById(context => context.Message.BasketId));

            // cancelled 
            Event(() => OrderRemoved, x => x.CorrelateBy(cart => cart.UserEmail, context => context.Message.Email));//x => x.CorrelateById(context => context.Message.BasketId));

            
            Schedule(() => BasketExpiredTimeout, x => x.ExpirationId, x => //x => x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(20);
                x.Received = e => e.CorrelateBy(cart => cart.UserEmail, context => context.Message.Email);//e => e.CorrelateById(context => context.Message.BasketId);
            });



            Initially(
                When(OrderStarted)
                    .Then(context =>
                    {
                        context.Instance.Created = context.Data.Timestamp;
                        context.Instance.Updated = context.Data.Timestamp;
                        context.Instance.UserName = context.Data.UserName;
                        context.Instance.UserEmail = context.Data.Email;
                    })
                    .ThenAsync(
                        context =>
                            Console.Out.WriteLineAsync(
                                $"Basket created for {context.Instance.UserEmail}:{context.Instance.CorrelationId}"))
                    .Schedule(BasketExpiredTimeout, context => new BasketExpired(context.Instance))
                    .TransitionTo(Active)
                );


            During(Active,
                When(OrderChanged)
                    .Then(context =>
                    {
                        if (context.Data.Timestamp > context.Instance.Updated)
                            context.Instance.Updated = context.Data.Timestamp;

                        context.Instance.OrderId = context.Data.OrderId;
                    })
                    .ThenAsync(
                        context =>
                            Console.Out.WriteLineAsync(
                                $"Order changed for {context.Instance.UserEmail}:{context.Instance.CorrelationId}"))
                    .Unschedule(BasketExpiredTimeout)
                    .Schedule(BasketExpiredTimeout, context => new BasketExpired(context.Instance)),

                When(OrderCompleted)
                    .Then(context =>
                    {
                        if (context.Data.Timestamp > context.Instance.Updated)
                            context.Instance.Updated = context.Data.Timestamp;

                        context.Instance.OrderId = context.Data.OrderId;
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Order complete for {context.Instance.UserEmail}:{context.Instance.CorrelationId}"))
                    .TransitionTo(Completed),



                //            When(BasketExpiredTimeout.Received)
                //    .ThenAsync(
                //        context =>
                //        {
                //            return Console.Out.WriteLineAsync(
                //                $"Basket Expired for {context.Instance.UserEmail}: {context.Instance.CorrelationId}");

                //        })
                //    .Publish(
                //    context => new BasketRemoved(context.Instance)) // HACK :                     // .Publish(context => new BasketRemoved(context.Instance))
                //    .Finalize()
                //);

            //When(ItemAdded)
            //    .Then(context =>
            //    {
            //        if (context.Data.Timestamp > context.Instance.Updated)
            //            context.Instance.Updated = context.Data.Timestamp;
            //    })
            //    .ThenAsync(context => Console.Out.WriteLineAsync($"Item Added: {context.Data.UserName} to {context.Instance.CorrelationId}"))
            //    .Schedule(CartExpired, context => new CartExpiredEvent(context.Instance)),
            When(BasketExpiredTimeout.Received)
                    .ThenAsync(
                        context =>
                        {
                            return Console.Out.WriteLineAsync(
                                $"Basket Expired for {context.Instance.UserEmail}:{context.Instance.CorrelationId}");

                        })
                    .Publish(
                    context => new BasketRemoved(context.Instance)) // HACK :                     // .Publish(context => new BasketRemoved(context.Instance))
                    .Finalize()
                );
        //    .Publish(context => (IMessageInterfaceType)new ConcreteMessageType(context.Instance, context.Data))


            SetCompletedWhenFinalized();

        }

        class BasketExpired :
            IBasketExpiredEvent
        {
            readonly BasketSaga _instance;

            public BasketExpired(BasketSaga instance)
            {
                _instance = instance;
            }

            public Guid BasketId => _instance.CorrelationId;
            public string Email => _instance.UserEmail;
            public Guid? CustomerId => _instance.CustomerId;
        }

        // for publishing to subsriber (outside)
        class BasketRemoved :
            IBasketRemovedEvent
        {
            readonly BasketSaga _instance;

            public BasketRemoved(BasketSaga instance)
            {
                _instance = instance;
            }

            public Guid BasketId { get; }
            public string Email => _instance.UserEmail;
            public Guid? CustomerId => _instance.CustomerId;
        }










    }



    }

    //public class BasketStateMachine
    //{
    //    public static State Initial { get; set; }
    //    public static State Completed { get; set; }
    //    public static State Open { get; set; }
    //    public static State Closed { get; set; }
    //}