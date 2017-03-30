using MvcTemplate.Data.Core;
using System;

namespace MvcTemplate.Services
{
    public abstract class BaseService : IService
    {
        protected IUnitOfWork UnitOfWork { get; }
        public Int32 CurrentAccountId { get; set; }

        protected BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}
