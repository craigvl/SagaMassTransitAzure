using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abt.Result.WebApi.BasketStore;
using Abt.Result.WebApi.Saga;
using Abt.Result.WebApi.Saga.Persistence;
using Automatonymous;
using MassTransit.AzureServiceBusTransport;
using MassTransit.EntityFrameworkIntegration;
using MassTransit.EntityFrameworkIntegration.Saga;
using MassTransit.Saga;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using MassTransit.QuartzIntegration;
using Quartz;
using Quartz.Impl;
using MassTransit;
using MassTransit.Monitoring.Introspection;
using Quartz.Util;


namespace Abt.Result.WebApi.Services.BasketManager
{
    // the Saga
    public class BasketManager
    {
        private readonly TokenProvider _credentials;
       // private readonly NamespaceManager _namespaceClient;
     //   private readonly IBasketStore _basketStore;
        private  IBusControl _bus;
        private readonly string ServiceNamespace;

        private QueueClient _queueClient;
        private QueueDescription _basketEventsQueue;
        private TopicDescription _topic;
        private BasketStateMachine _machine;
        private readonly Lazy<ISagaRepository<BasketSaga>> _repository;
        private readonly IScheduler _scheduler;
        private readonly string _dbConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\DOM\AzureTopicPOC\Abt.Result.WebApi\BasketSagaData.mdf;Integrated Security=True";

        static IScheduler CreateScheduler()
        {
            return new StdSchedulerFactory().GetScheduler();
        }

        public BasketManager(string cs, string sasKeyName, string sasKeyValue, string serviceNamespace)
        {
            _scheduler = CreateScheduler();
            _dbConnectionString = cs;
            //    _basketStore = basketStore;
            ServiceNamespace = serviceNamespace;
            _credentials = TokenProvider.CreateSharedAccessSignatureTokenProvider(sasKeyName, sasKeyValue);
           // _namespaceClient = new NamespaceManager(ServiceBusEnvironment.CreateServiceUri("sb", ServiceNamespace, string.Empty), _credentials);

            Console.WriteLine("DB init ... ");
            //var dbConnectionString =
            //    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\DOM\AzureTopicPOC\Abt.Result.WebApi\BasketSagaData.mdf;Integrated Security=True";
            // DB
            SagaDbContextFactory sagaDbContextFactory = () => new SagaDbContext<BasketSaga, BasketDataMap>(_dbConnectionString);
            using (var dbConnection = new SqlConnection(_dbConnectionString))
            {
                dbConnection.Open();
                var cmd = dbConnection.CreateCommand();
                cmd.CommandText = "DELETE FROM BasketSagas";
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Persistence Data Table flushed ... ");

            _repository = new Lazy<ISagaRepository<BasketSaga>>(() => new EntityFrameworkSagaRepository<BasketSaga>(sagaDbContextFactory));
            
            Init();

        }

        public IList<dynamic> GetCurrentBaskets()
        {
            List<dynamic> rows = new List<dynamic>();
            using (var dbConnection = new SqlConnection(_dbConnectionString))
            {
                dbConnection.Open();
                //using(var db = new EntityFrameworkSagaRepository<D>())
                var cmd = dbConnection.CreateCommand();
                cmd.CommandText = "SELECT * FROM BasketSagas";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new {
                            CurrentState = reader["CurrentState"],
                            Username = reader["UserName"],
                            Email = reader["UserEmail"]
                        });
                            /*
                             *  public string CurrentState { get; set; }
                                public string UserName { get; set; }
                                public string UserEmail { get; set; }
                                public DateTime Created { get; set; }
                                public DateTime Updated { get; set; }
                                public Guid? ExpirationId { get; set; }
                             * 
                             * 
                             * 
                             */

                            //sifre = reader["u_password"].ToString();
                        }
                    }
                }
            return rows;
            // cmd.ExecuteReader();
        }
        



        //public void SendBusPing()
        //{
        //    _bus.Publish<IOrderStartedEvent>(new CreateOrderMessage()
        //    {
        //        Email = "hello" + DateTime.Now.ToFileTimeUtc().ToString(),
        //        Timestamp = DateTime.Now,
        //        UserName = "thierry" + DateTime.Now.ToFileTimeUtc().ToString()
        //    });

        //    //_bus.Publish<IOrderStartedEvent>(new CreateOrderMessage());
        //}

        protected void Init()
        {
            Console.WriteLine("Bus init ... ");

        //    MessagingFactory factory = MessagingFactory.Create(ServiceBusEnvironment.CreateServiceUri("sb", ServiceNamespace, string.Empty), _credentials);

            //_basketEventsQueue = !_namespaceClient.QueueExists("BasketsQueue") ?  _namespaceClient.CreateQueue("BasketsQueue") : _namespaceClient.GetQueue("BasketsQueue");
            //_topic = !_namespaceClient.TopicExists("BasketsTopic") ? _namespaceClient.CreateTopic("BasketsTopic") : _namespaceClient.GetTopic("BasketsTopic");
            //_queueClient = factory.CreateQueueClient("BasketsQueue");

            _machine = new BasketStateMachine();
            _bus = Bus.Factory.CreateUsingAzureServiceBus(bus =>
            {
                var busHost = bus.Host(new Uri("sb://dominosbasket.servicebus.windows.net"), host =>
                {
                    host.OperationTimeout = TimeSpan.FromSeconds(20);
                    host.TokenProvider = _credentials;

                });

                //bus.UseMessageScheduler();

                //var queue = busHost.GetQueuePath(new QueueDescription("BasketsQueue") {});
                bus.ReceiveEndpoint(busHost, "basketevents", e =>
                {
                    //e.Handler<IOrderStartedEvent>(o=>
                    // {
                    //     Console.WriteLine("received");
                    //     return Task.FromResult(o);
                    // });

                    e.PrefetchCount = 8;
                    e.StateMachineSaga(_machine, _repository.Value);
                  
                });
                bus.UseInMemoryScheduler();
                bus.UsePublishMessageScheduler();

                //bus.ReceiveEndpoint(busHost, "quartz", e =>
                //{
                //    bus.UseMessageScheduler(e.InputAddress);
                //    e.PrefetchCount = 1;
                //    e.Consumer(() => new ScheduleMessageConsumer(_scheduler));
                //    e.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
                //});
                //bus.UsePublishMessageScheduler();
                //bus.UseInMemoryScheduler();
                _scheduler.Start();
                //   cfg

                //bus.ReceiveEndpoint(busHost, "my-srv1",
                //           conf =>
                //           {
                //               conf.Consumer(componentContext.Resolve<Func<Notify1Consumer>>());
                //           });

                //bus.UseServiceBusMessageScheduler();
                // bus.UseMessageScheduler();
                //bus.UseInMemoryMessageScheduler();
                //_scheduler.Start();
                //bus.ReceiveEndpoint(busHost, "quartz", e =>
                //{
                //    bus.UseMessageScheduler(e.InputAddress);
                //    e.PrefetchCount = 2;
                //    e.Consumer(() => new ScheduleMessageConsumer(_scheduler));
                //    e.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
                //});
            });
            _bus.Start();
            //_bus.Publish<IOrderStartedEvent>(new CreateOrderMessage());

            //{
            //    Email = "thierry"
            //})
            //_bus.Send<IOrderStartedEvent>(new CreateOrderMessage()
            //{
            //    Email = "thierry"
            //}).Wait();
            Console.WriteLine("Bus ready");

            //  Once the machine and repository are declared, the receive endpoint is declared on the bus configuration.

            //_busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            //{
            //    IRabbitMqHost host = x.Host(...);

            //    x.ReceiveEndpoint(host, "shopping_cart_state", e =>
            //    {
            //        e.PrefetchCount = 8;
            //        e.StateMachineSaga(_machine, _repository.Value);
            //    });

            //    x.ReceiveEndpoint(host, "scheduler", e =>
            //    {
            //        x.UseMessageScheduler(e.InputAddress);

            //        e.PrefetchCount = 1;

            //        e.Consumer(() => new ScheduleMessageConsumer(_scheduler));
            //        e.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
            //    });
            //});

        }

        public class CreateOrderMessage : IOrderStartedEvent
        {
            public DateTime Timestamp { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }            
        }

        public void CreateOrder(string email)
        {
            //_bus.Publish<IOrderStartedEvent>(new
            //{
            //    Email = email,
            //    Timestamp = DateTime.UtcNow,

            //});

            _bus.Publish<IOrderStartedEvent>(new CreateOrderMessage
            {
                Email = email,
                Timestamp = DateTime.UtcNow
            });
            

        //    var x = new OrderStarted
            //_machine.RaiseEvent()
            // _queueClient.Send(new BrokeredMessage(new CreateOrderMessage() {Email = email}));
        }

        public void AddItem(string email)
        {
            _bus.Publish<IBasketContentChanged>(new
            {
                Email = email,
                Timestamp = DateTime.UtcNow
            });
        }

        // Create management credentials
        //TokenProvider credentials = TokenProvider.CreateSharedAccessSignatureTokenProvider(sasKeyName, sasKeyValue);
        //// Create namespace client
        //NamespaceManager namespaceClient = new NamespaceManager(ServiceBusEnvironment.CreateServiceUri("sb", ServiceNamespace, string.Empty), credentials);
        public void CompleteOrder(string email)
        {
            _bus.Publish<IOrderCompletedEvent>(new 
            {
                Email = email,
                Timestamp = DateTime.UtcNow
            });
        }


    }
}
