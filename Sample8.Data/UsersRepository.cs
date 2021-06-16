using Sample8Models;

namespace Sample8Data
{
    public class UsersRepository : Repository<User>, IUsersRepository
    {
        public UsersRepository()
        {
            _contoller = "users";
        }
    }
}
