using Microsoft.Extensions.Options;
using Sample8Data;
using Sample8Models;
using Sample8Models.Common;
using Sample8Models.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample8Biz
{
    public sealed class UserService : IUserService
    {
        IUsersRepository _usersRepository;

        public UserService(IUsersRepository usersRepository, IOptions<AppSettings> settings)
        {
            _usersRepository = usersRepository;
            _usersRepository.SetConfig(settings.Value.RepoExternalUrl);
        }

        public Task<User> Get(int id)
        {
            return _usersRepository.Get(id);
        }

        public Task<object> Import(SearchParams searchParams)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<User>> List()
        {
            return _usersRepository.List();
        }

        public Task<SearchResultView> List(SearchParams searchParams)
        {
            throw new System.NotImplementedException();
        }
    }
}
