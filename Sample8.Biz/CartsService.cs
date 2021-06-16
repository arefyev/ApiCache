using Sample8Data;
using Sample8Models.Views;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sample8Models;
using Microsoft.Extensions.Options;
using Sample8Models.Common;
using Sample8Common;

namespace Sample8Biz
{
    public sealed class CartsService : ICartsService
    {
        ICartsRepository _cartsRepository;
        IUsersRepository _usersRepository;
        IProductsRepository _productsRepository;

        public CartsService(ICartsRepository cartsRepository, IUsersRepository usersRepository, IProductsRepository productsRepository, IOptions<AppSettings> settings)
        {
            _cartsRepository = cartsRepository;
            _usersRepository = usersRepository;
            _productsRepository = productsRepository;

            _productsRepository.SetConfig(settings.Value.RepoExternalUrl);
            _usersRepository.SetConfig(settings.Value.RepoExternalUrl);
            _cartsRepository.SetConfig(settings.Value.RepoExternalUrl);
        }

        public async Task<CartView> Get(int id)
        {
            var _cart = await _cartsRepository.Get(id);
            if (_cart == null)
                return null;

            var products = await _productsRepository.Find(_cart.Products.Select(x => x.ProductId).Distinct().ToArray());

            return new CartView { Id = _cart.Id, Date = _cart.Date, User = await _usersRepository.Get(_cart.UserId), Products = products };
        }

        public async Task<IEnumerable<CartView>> List()
        {
            var carts = await _cartsRepository.List();

            return await BuildCarts(carts);
        }

        public async Task<SearchResultView> List(SearchParams searchParams)
        {
            var _carts = await _cartsRepository.List(); // in this sample need a cache

            if (searchParams.Asc)
                _carts = _carts.OrderByName(searchParams.SortField);
            else
                _carts.OrderByDescendingName(searchParams.SortField);

            var carts = _carts.Skip(searchParams.Page * searchParams.Count).Take(searchParams.Count);

            var views = await BuildCarts(carts);

            return new SearchResultView(views, _carts.Count());
        }

        public Task<object> Import(SearchParams searchParams)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<CartView>> BuildCarts(IEnumerable<Cart> carts)
        {
            var productsIds = carts.SelectMany(x => x.Products).Select(x => x.ProductId).Distinct().ToArray();
            var usersIds = carts.Select(x => x.UserId).Distinct().ToArray();

            var productsTask = _productsRepository.Find(productsIds);
            var usersTask = _usersRepository.Find(usersIds);

            await Task.WhenAll(usersTask, productsTask);

            var products = productsTask.Result;
            var users = usersTask.Result;


            return carts.Select(x => new CartView { Id = x.Id, Date = x.Date, User = users.First(y => y.Id == x.UserId), Products = products.Where(y => x.Products.Select(x => x.ProductId).Contains(x.Id)) }).ToList();
        }
    }
}