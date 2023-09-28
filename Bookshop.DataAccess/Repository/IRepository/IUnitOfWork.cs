﻿using Bookshop.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookshop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {

        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }

        void Save();
    }
}
