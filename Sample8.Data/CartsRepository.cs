using Sample8Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample8Data
{
    public sealed class CartsRepository : Repository<Cart>, ICartsRepository
    {
        public CartsRepository()
        {
            _contoller = "carts";
        }

        public override Task<IEnumerable<Cart>> List()
        {
            return base.List();
        }
    }
}
