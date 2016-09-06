using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abt.Result.WebApi.BasketStore
{
    public class Basket
    {
        public Basket(int basketId, string customerEmail)
        {
            CustomerEmail = customerEmail;
            BasketId = basketId;
        }

        //public Basket(string customerEmail, int basketId)
        //{
        //    CustomerEmail = customerEmail;
        //    BasketId = basketId;
        //}

        public string CustomerEmail { get; set; }    
        public int BasketId { get; set; }
        private IList<string> Contents { get; set; }
    }


    public class MemoryBasketStore : IBasketStore
    {
        private static int _lastId = 0;
        private readonly IDictionary<int, Basket> _baskets = new Dictionary<int, Basket>()
        {
            {1,  new Basket(1, "thierry.rais@gmail.com")},
            {2,  new Basket(2, "thierry.rais@dominos.com.au")}
        };

        public int CreateBasket(string email)
        {
            _lastId = _baskets.Keys.Max() + 1;
            var basket =  new Basket(_lastId, email);
            _baskets.Add(basket.BasketId, basket);
            return basket.BasketId;
        }

        public IEnumerable<Basket> GetAllBaskets() => _baskets.Values;

        public void DeleteBasket(int id)
        {
            // send to Epsilon
        }

    }
}
