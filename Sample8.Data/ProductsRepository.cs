using Sample8Models;

namespace Sample8Data
{
    public sealed class ProductsRepository : Repository<Product>, IProductsRepository
    {
        public ProductsRepository()
        {
            _contoller = "products";
        }
    }
}
