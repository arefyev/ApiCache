using Microsoft.Extensions.Options;
using Sample8Common;
using Sample8Data;
using Sample8Models;
using Sample8Models.Common;
using Sample8Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample8Biz
{
    public sealed class ProductsService : IProductsService
    {
        IProductsRepository _productsRepository;

        public ProductsService(IProductsRepository productsRepository, IOptions<AppSettings> settings)
        {
            _productsRepository = productsRepository;
            _productsRepository.SetConfig(settings.Value.RepoExternalUrl);
        }

        public Task<Product> Get(int id)
        {
            return _productsRepository.Get(id);
        }

        public Task<object> Import(SearchParams searchParams)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> List()
        {
            return _productsRepository.List();
        }

        public async Task<SearchResultView> List(SearchParams searchParams)
        {
            var _products = await _productsRepository.List(); // in this sample need a cache

            if (searchParams.Asc)
                _products = _products.OrderByName(searchParams.SortField);
            else
                _products.OrderByDescendingName(searchParams.SortField);

            var products = _products.Skip(searchParams.Page * searchParams.Count).Take(searchParams.Count);

            return new SearchResultView(products, _products.Count());
        }
    }
}