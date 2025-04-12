using System;
using System.Collections.Generic;
using MurderMystery.Interfaces.MurderMystery.Data.Providers;

namespace MurderMystery.Data
{

    public abstract class BaseDataProvider<T> : IDataProvider<T>
    {
        protected List<T> _items;

        protected static readonly Random _random = new Random();

        public List<T> GetAll()
        {
            if (_items == null)
            {
                _items = LoadItems();
            }

            return _items;
        }

        public virtual T GetRandom()
        {
            var items = GetAll();
            return items[_random.Next(items.Count)];
        }

        protected abstract List<T> LoadItems();
    }
}