using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Validation;

namespace SalesWPFApp.OrderUI
{
    /// <summary>
    /// Interaction logic for OrderDetailsInformationWindow.xaml
    /// </summary>
    public partial class OrderDetailsInformationWindow : Window
    {
        private OrderDetail selectedOrderDetail;
        private int orderId;
        private PopupMode mode;
        private IOrderDetailRepository orderDetailRepository;
        private IProductRepository productRepository;
        private IEnumerable<Product> products;
        public OrderDetailsInformationWindow(PopupMode mode,
            IOrderDetailRepository orderDetailRepository, IProductRepository productRepository,
            int orderId, OrderDetail selectedOrderDetail = null)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.mode = mode;
            this.orderDetailRepository = orderDetailRepository;
            this.productRepository = productRepository;
            this.orderId = orderId;
            this.selectedOrderDetail = selectedOrderDetail;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProductList();
            if (mode == PopupMode.Add)
            {
                selectedOrderDetail = new OrderDetail()
                {
                    OrderId = orderId,
                    UnitPrice = 0,
                    Quantity = 0,
                    Discount = 0
                };
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
                cboProduct.IsEnabled = true;
            }
            else if (mode == PopupMode.Update)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
            LoadOrderDetailInformation();
        }

        private void LoadOrderDetailInformation()
        {
            if (mode == PopupMode.Update)
            {
                selectedOrderDetail = orderDetailRepository.GetOrderDetail(orderId, selectedOrderDetail.ProductId);
                if (selectedOrderDetail.Product != null)
                {
                    cboProduct.SelectedItem = selectedOrderDetail.Product.ProductName;
                }
            }
            txtOrderId.Text = selectedOrderDetail.OrderId.ToString();

            txtUnitPrice.Text = selectedOrderDetail.UnitPrice.ToString();
            txtQuantity.Text = selectedOrderDetail.Quantity.ToString();
            txtDiscount.Text = selectedOrderDetail.Discount.ToString();
        }
        private void LoadProductList()
        {
            try
            {
                cboProduct.ItemsSource = null;
                products = productRepository.GetProducts();
                if (products == null || products.Count() == 0)
                {
                    throw new Exception("There is no product to load..." +
                        " Please try to add at least one product to operate this action!!");
                }
                List<string> productNameList = new List<string>();
                foreach (Product product in products)
                {
                    productNameList.Add(product.ProductName);
                }
                cboProduct.ItemsSource = productNameList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loading product list...",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private OrderDetail GetOrderDetailObject() => new OrderDetail
        {
            OrderId = int.Parse(txtOrderId.Text),
            ProductId = productRepository.GetProduct(cboProduct.SelectedItem.ToString()).ProductId,
            UnitPrice = decimal.Parse(txtUnitPrice.Text),
            Quantity = int.Parse(txtQuantity.Text),
            Discount = double.Parse(txtDiscount.Text)
        };

        private void CheckObject()
        {
            if (string.IsNullOrEmpty(txtOrderId.Text) || txtOrderId.Text.Equals("-1"))
            {
                throw new Exception("Order ID cannot be empty or it is invalid!!" +
                    " Please contact the developer for more information...");
            }
            if (string.IsNullOrEmpty(cboProduct.SelectedItem.ToString()) ||
                cboProduct.SelectedItem.ToString().Equals("Add new product.."))
            {
                throw new Exception("Please choose a product for this order detail!!");
            }
            if (!NumberValidation.IsDecimal(txtUnitPrice.Text) ||
                decimal.Parse(txtUnitPrice.Text) <= 0)
            {
                throw new Exception("Unit Price has to be a positive number!");
            }
            if (!NumberValidation.IsInt(txtQuantity.Text) ||
                int.Parse(txtQuantity.Text) < 0)
            {
                throw new Exception("Quantity has to be a positive integer!!");
            }
            if (!NumberValidation.IsDouble(txtDiscount.Text) ||
                double.Parse(txtDiscount.Text) < 0)
            {
                throw new Exception("Discount has to be a positive number!!");
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                if (orderDetailRepository.GetOrderDetail(orderId,
                    productRepository.GetProduct(cboProduct.SelectedItem.ToString()).ProductId) != null)
                {
                    throw new Exception($"An order detail with the selected product '" +
                        $"{cboProduct.SelectedItem.ToString()}' is existed! " +
                        $"Please double click it modify...");
                }
                OrderDetail newOrderDetail = orderDetailRepository.Add(GetOrderDetailObject());
                if (newOrderDetail == null)
                {
                    throw new Exception("An error has occured when adding a new order detail!! Please try again...");
                }
                MessageBox.Show($"Order Detail is added successfully!", "Add new Order detail",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add new Order Detail",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                OrderDetail newOrderDetail = orderDetailRepository.Update(GetOrderDetailObject());
                if (newOrderDetail == null)
                {
                    throw new Exception("An error has occured when updating a new order detail!! Please try again...");
                }
                MessageBox.Show($"Order Detail is updated successfully!", "Update Order detail",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Order Detail",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cboProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string productName = cboProduct.SelectedItem.ToString();
                Product product = productRepository.GetProduct(productName);
                if (product == null)
                {
                    throw new Exception("An error has occured when getting product information!! " +
                        "Please try again...");
                }
                txtUnitPrice.Text = product.UnitPrice.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Get Product information",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
