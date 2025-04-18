using System;
namespace MurderMystery.Interfaces
{
    using System;
    using System.Collections.Generic;

    namespace MurderMystery.Data.Providers
    {

        public interface IDataProvider<T>
        {
            List<T> GetAll();
            T GetRandom();
        }
    }
}
