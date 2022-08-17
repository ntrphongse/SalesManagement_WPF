using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using System;
using System.Windows;

namespace SalesWPFApp.ProductUI
{
    /// <summary>
    /// Interaction logic for ProductDetailsWindow.xaml
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
        private Product selectedProduct;
        private PopupMode mode;
        private IProductRepository productRepository;
        public ProductDetailsWindow(PopupMode mode, IProductRepository productRepository, Product selectedProduct = null)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.mode = mode;
            this.productRepository = productRepository;
            this.selectedProduct = selectedProduct;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mode == PopupMode.Add)
            {
                selectedProduct = new Product()
                {
                    ProductId = productRepository.GetNextProductId(),
                    ProductName = "",
                    CategoryId = 0,
                    Weight = "",
                    UnitPrice = 0,
                    UnitslnStock = 0
                };
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
            }
            else if (mode == PopupMode.Update)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
            txtProductId.Text = selectedProduct.ProductId.ToString();
            txtProductName.Text = selectedProduct.ProductName;
            txtCategory.Text = selectedProduct.CategoryId.ToString();
            txtWeight.Text = selectedProduct.Weight;
            txtPrice.Text = selectedProduct.UnitPrice.ToString();
            txtUnitsInStock.Text = selectedProduct.UnitslnStock.ToString();
        }

        private Product GetProductObject() => new Product
        {
            ProductId = int.Parse(txtProductId.Text),
            ProductName = txtProductName.Text,
            CategoryId = int.Parse(txtCategory.Text),
            Weight = txtWeight.Text,
            UnitPrice = decimal.Parse(txtPrice.Text),
            UnitslnStock = int.Parse(txtUnitsInStock.Text)
        };

        private void CheckObject()
        {
            if (string.IsNullOrEmpty(txtProductId.Text) || txtProductId.Text.Equals("-1"))
            {
                throw new Exception("Product ID cannot be empty or it is invalid! This must be a program error... " +
                    "Please contact the developer for more information");
            }
            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                throw new Exception("Product name cannot be empty!!");
            }
            if (string.IsNullOrEmpty(txtWeight.Text))
            {
                throw new Exception("Weight cannot be empty!!");
            }
            if (!int.TryParse(txtCategory.Text, out _) || int.Parse(txtCategory.Text) <= 0)
            {
                throw new Exception("Category has to be a positive number!!");
            }
            if (!decimal.TryParse(txtPrice.Text, out _) || decimal.Parse(txtPrice.Text) <= 0)
            {
                throw new Exception("Unit Price has to be a positive number!!");
            }
            if (!int.TryParse(txtUnitsInStock.Text, out _) || int.Parse(txtUnitsInStock.Text) < 0)
            {
                throw new Exception("Units In Stock has to be a positive number!!");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                Product newProduct = productRepository.Add(GetProductObject());
                if (newProduct == null)
                {
                    throw new Exception("An error has occured when adding a new product! Please try again...");
                }
                MessageBox.Show($"Product {newProduct.ProductName} is added successfully!!", "Add new Product",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add new Product", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                Product updatedProduct = productRepository.Update(GetProductObject());
                if (updatedProduct == null)
                {
                    throw new Exception("An error has occured when updating the product! Please try again...");
                }
                MessageBox.Show($"Product {updatedProduct.ProductName} is updated successfully!", "Update Product",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Product", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
