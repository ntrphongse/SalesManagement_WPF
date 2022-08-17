using BusinessObject;
using DataAccess.DAO;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public interface IProductRepository
    {
        public IEnumerable<Product> GetProducts();
        public int GetNextProductId();
        public Product GetProduct(int productId);
        public Product GetProduct(string productName);
        public Product Add(Product newProduct);
        public Product Update(Product updatedProduct);
        public void Delete(int productId);
        public IEnumerable<Product> SearchProduct(string productName);
        public IEnumerable<Product> SearchProduct(int productId);
        public IEnumerable<Product> SearchProduct(int startUnit, int endUnit);
        public IEnumerable<Product> SearchProduct(decimal startPrice, decimal endPrice);
        public IEnumerable<Product> SearchProduct(int unit, FilterMode mode);
        public IEnumerable<Product> SearchProduct(decimal price, FilterMode mode);
    }
}
