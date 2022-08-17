using BusinessObject;
using DataAccess.DAO;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        public Product Add(Product newProduct) => ProductDAO.Instance.Add(newProduct);

        public void Delete(int productId) => ProductDAO.Instance.Delete(productId);

        public int GetNextProductId() => ProductDAO.Instance.GetNextProductId();

        public Product GetProduct(int productId) => ProductDAO.Instance.GetProduct(productId);

        public Product GetProduct(string productName) => ProductDAO.Instance.GetProduct(productName);

        public IEnumerable<Product> GetProducts() => ProductDAO.Instance.GetProducts();

        public IEnumerable<Product> SearchProduct(string productName) => ProductDAO.Instance.SearchProduct(productName);

        public IEnumerable<Product> SearchProduct(int productId) => ProductDAO.Instance.SearchProduct(productId);

        public IEnumerable<Product> SearchProduct(int startUnit, int endUnit) => ProductDAO.Instance.SearchProduct(startUnit, endUnit);

        public IEnumerable<Product> SearchProduct(decimal startPrice, decimal endPrice) => ProductDAO.Instance.SearchProduct(startPrice, endPrice);

        public IEnumerable<Product> SearchProduct(int unit, FilterMode mode) => ProductDAO.Instance.SearchProduct(unit, mode);

        public IEnumerable<Product> SearchProduct(decimal price, FilterMode mode) => ProductDAO.Instance.SearchProduct(price, mode);

        public Product Update(Product updatedProduct) => ProductDAO.Instance.Update(updatedProduct);
    }
}
