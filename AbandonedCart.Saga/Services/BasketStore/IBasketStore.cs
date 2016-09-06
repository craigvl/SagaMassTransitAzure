using System;
using System.Collections.Generic;

namespace Abt.Result.WebApi.BasketStore
{
    public interface IBasketStore
    {
        IEnumerable<Basket> GetAllBaskets();

        void DeleteBasket(int id);
        int CreateBasket(string email);
    }
}