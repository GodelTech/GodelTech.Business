using System;
using GodelTech.Data;
using Microsoft.Extensions.Logging;

namespace GodelTech.Business.Tests.Fakes
{
    public class FakeBusinessService<TKey> : BusinessService<FakeEntity<TKey>, TKey, IUnitOfWork, FakeDto<TKey>, FakeAddDto, FakeEditDto<TKey>>
    {
        public FakeBusinessService(
            IUnitOfWork unitOfWork,
            Func<IUnitOfWork, IRepository<FakeEntity<TKey>, TKey>> repositorySelector,
            IBusinessMapper businessMapper,
            ILogger logger)
            : base(
                unitOfWork,
                repositorySelector,
                businessMapper,
                logger)
        {

        }
    }
}
