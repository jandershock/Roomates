using System.Collections.Generic;

namespace Roommates.Repositories
{
    internal interface IGenericRepository<T>
    {
        List<T>  GetAll();
        T GetById(int id);
        void Insert(T obj);

    }
}
