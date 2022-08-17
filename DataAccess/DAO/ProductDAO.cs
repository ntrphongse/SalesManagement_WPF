using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAO
{
    public enum FilterMode
    {
        Min,
        Max
    }
    internal class ProductDAO
    {
        private static ProductDAO instance = null;
        private static readonly object instanceLock = new object();
        private ProductDAO()
        {

        }
        internal static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        internal IEnumerable<Product> GetProducts()
        {
            List<Product> products = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                products = database.Products.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return products;
        }

        internal int GetNextProductId()
        {
            int nextId = -1;
            try
            {
                var database = new FStoreDBAssignmentContext();
                Product currentMax = database.Products.OrderByDescending(product => product.ProductId).First();
                nextId = currentMax.ProductId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return nextId;
        }

        internal Product GetProduct(int productId)
        {
            Product product = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                product = database.Products.SingleOrDefault(product => product.ProductId == productId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        internal Product GetProduct(string productName)
        {
            Product product = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                product = database.Products
                    .SingleOrDefault(product => product.ProductName.ToLower().Equals(productName.ToLower()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        internal Product Add(Product newProduct)
        {
            Product product = null;
            try
            {
                if (GetProduct(newProduct.ProductId) != null)
                {
                    throw new Exception($"Product with the ID {newProduct.ProductId} is existed! " +
                        $"Please check with the developer for more information");
                }
                if (GetProduct(newProduct.ProductName) != null)
                {
                    throw new Exception($"Product with the name {newProduct.ProductName} is existed!");
                }
                var database = new FStoreDBAssignmentContext();
                database.Products.Add(newProduct);
                database.SaveChanges();
                product = newProduct;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        internal Product Update(Product updatedProduct)
        {
            Product product = null;
            try
            {
                if (GetProduct(updatedProduct.ProductId) == null)
                {
                    throw new Exception($"Product with the ID {updatedProduct.ProductId} is not existed! " +
                        $"Please check with the developer for more information");
                }
                var database = new FStoreDBAssignmentContext();
                database.Products.Update(updatedProduct);
                database.SaveChanges();
                product = updatedProduct;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        internal void Delete(int productId)
        {
            try
            {
                if (GetProduct(productId) == null)
                {
                    throw new Exception($"Product with the ID {productId} is not existed...");
                }
                var database = new FStoreDBAssignmentContext();
                database.Products.Remove(GetProduct(productId));
                database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal IEnumerable<Product> SearchProduct(string productName)
        {
            IEnumerable<Product> searchResult = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                searchResult = database.Products
                                .Where(product => product.ProductName.ToLower().Contains(productName.ToLower()))
                                .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return searchResult;
        }
        internal IEnumerable<Product> SearchProduct(int productId)
        {
            IEnumerable<Product> searchResult = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                searchResult = database.Products
                                .Where(product => product.ProductId == productId)
                                .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return searchResult;
        }

        internal IEnumerable<Product> SearchProduct(int startUnit, int endUnit)
        {
            IEnumerable<Product> searchProduct = null;
            if (startUnit > endUnit)
            {
                int temp = startUnit;
                startUnit = endUnit;
                endUnit = temp;
            }
            try
            {
                var database = new FStoreDBAssignmentContext();
                searchProduct = database.Products
                    .Where(product => product.UnitslnStock >= startUnit && product.UnitslnStock <= endUnit)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return searchProduct;
        }

        internal IEnumerable<Product> SearchProduct(decimal startPrice, decimal endPrice)
        {
            IEnumerable<Product> searchProduct = null;
            if (startPrice > endPrice)
            {
                decimal temp = startPrice;
                startPrice = endPrice;
                endPrice = temp;
            }
            try
            {
                var database = new FStoreDBAssignmentContext();
                searchProduct = database.Products
                    .Where(product => product.UnitPrice >= startPrice && product.UnitPrice <= endPrice)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return searchProduct;
        }

        internal IEnumerable<Product> SearchProduct(int unit, FilterMode mode)
        {
            IEnumerable<Product> searchProduct = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                if (mode == FilterMode.Min)
                {
                    searchProduct = database.Products
                        .Where(product => product.UnitslnStock >= unit)
                        .ToList();
                }
                else if (mode == FilterMode.Max)
                {
                    searchProduct = database.Products
                        .Where(product => product.UnitslnStock <= unit)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return searchProduct;
        }

        internal IEnumerable<Product> SearchProduct(decimal price, FilterMode mode)
        {
            IEnumerable<Product> searchProduct = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                if (mode == FilterMode.Min)
                {
                    searchProduct = database.Products
                        .Where(product => product.UnitPrice >= price)
                        .ToList();
                }
                else if (mode == FilterMode.Max)
                {
                    searchProduct = database.Products
                        .Where(product => product.UnitPrice <= price)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return searchProduct;
        }
    }
}
