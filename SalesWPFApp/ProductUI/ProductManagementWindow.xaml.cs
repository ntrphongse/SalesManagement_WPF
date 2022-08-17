using AppSettings;
using BusinessObject;
using DataAccess.DAO;
using DataAccess.Repository;
using SalesWPFApp.MemberUI;
using SalesWPFApp.OrderUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SalesWPFApp.ProductUI
{
    /// <summary>
    /// Interaction logic for ProductManagementWindow.xaml
    /// </summary>
    public partial class ProductManagementWindow : Window
    {
        private Member loginMember;
        private IProductRepository productRepository;
        private IMemberRepository memberRepository;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        IEnumerable<Product> dataSource;
        public ProductManagementWindow(Member loginMember, IProductRepository productRepository,
            IMemberRepository memberRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.loginMember = loginMember;
            this.productRepository = productRepository;
            this.memberRepository = memberRepository;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            WindowLoad();
        }

        private void WindowLoad()
        {
            try
            {
                if (loginMember == null)
                {
                    throw new Exception("Login information is invalid! Please exit the application and try again...");
                }
                lblStatus.Text = $"Welcome {loginMember.Email}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Product Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void menuMember_Click(object sender, RoutedEventArgs e)
        {
            MemberManagementWindow memberManagement = new MemberManagementWindow(loginMember,
                memberRepository, productRepository,
                orderRepository, orderDetailRepository);
            memberManagement.Closed += (s, args) => this.Close();
            this.Hide();
            memberManagement.Show();
        }

        private void menuOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderManagementWindow orderManagement = new OrderManagementWindow(loginMember,
                orderRepository, orderDetailRepository,
                memberRepository, productRepository);
            orderManagement.Closed += (s, args) => this.Close();
            this.Hide();
            orderManagement.Show();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to exit?", "Product Management",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsList();
        }

        private void LoadProductsList()
        {
            dataSource = new List<Product>();
            try
            {
                dataSource = productRepository.GetProducts();
                listProducts.ItemsSource = null;
                listProducts.ItemsSource = dataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Product Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(PopupMode.Add, productRepository);
            if (productDetailsWindow.ShowDialog() != false)
            {
                LoadProductsList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtProductId.Text))
                {
                    throw new Exception("No product is selected!!");
                }
                MessageBoxResult result = MessageBox.Show($"Do you really want to delete product with the ID {txtProductId.Text} ?", "Delete product",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    productRepository.Delete(int.Parse(txtProductId.Text));
                    MessageBox.Show("Delete product successfully!!", "Delete product", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    LoadProductsList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete product", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Product selectedProduct = ((ListViewItem)sender).Content as Product;
            ProductDetailsWindow productDetailsWindow = new ProductDetailsWindow(PopupMode.Update, productRepository, selectedProduct);

            if (productDetailsWindow.ShowDialog() != false)
            {
                LoadProductsList();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchValue = txtSearch.Text;
                if (string.IsNullOrEmpty(searchValue))
                {
                    throw new Exception("Please enter search value...");
                }
                if (rdSearchId.IsChecked == true)
                {
                    if (!int.TryParse(searchValue, out _))
                    {
                        throw new Exception("Product ID has to be an integer!!");
                    }
                    int searchId = int.Parse(searchValue);
                    IEnumerable<Product> products = productRepository.SearchProduct(searchId);
                    if (products == null || products.Count() == 0)
                    {
                        MessageBox.Show("No result found!", "Search Product",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        listProducts.ItemsSource = products;
                    }
                }
                else if (rdSearchName.IsChecked == true)
                {
                    IEnumerable<Product> products = productRepository.SearchProduct(searchValue);
                    if (products == null || !products.Any())
                    {
                        MessageBox.Show("No result found!", "Search Product",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        listProducts.ItemsSource = products;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Product",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string start = txtFrom.Text;
                string end = txtTo.Text;
                IEnumerable<Product> products = null;
                if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
                {
                    throw new Exception("Please enter at least one value to filter...");
                }
                if (rdFilterPrice.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                    {
                        // Both values are entered
                        if (!decimal.TryParse(start, out _) || !decimal.TryParse(end, out _))
                        {
                            throw new Exception("Value has to be a number!!");
                        }
                        decimal from = decimal.Parse(start);
                        decimal to = decimal.Parse(end);
                        products = productRepository.SearchProduct(from, to);
                    }
                    else if (!string.IsNullOrEmpty(start))
                    {
                        // End is empty
                        if (!decimal.TryParse(start, out _))
                        {
                            throw new Exception("From value has to be a number!!");
                        }
                        decimal from = decimal.Parse(start);
                        products = productRepository.SearchProduct(from, FilterMode.Min);
                    }
                    else
                    {
                        // Start is empty
                        if (!decimal.TryParse(end, out _))
                        {
                            throw new Exception("To value has to be a number!!");
                        }
                        decimal to = decimal.Parse(end);
                        products = productRepository.SearchProduct(to, FilterMode.Max);
                    }

                    if (products == null || products.Count() == 0)
                    {
                        MessageBox.Show("No result found!", "Search Product",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        listProducts.ItemsSource = products;
                    }

                }
                else if (rdFilterStock.IsChecked == true)
                {
                    if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                    {
                        // Both values are entered
                        if (!int.TryParse(start, out _) || !int.TryParse(end, out _))
                        {
                            throw new Exception("Value has to be an integer!!");
                        }
                        int from = int.Parse(start);
                        int to = int.Parse(end);
                        products = productRepository.SearchProduct(from, to);
                    }
                    else if (!string.IsNullOrEmpty(start))
                    {
                        // End is empty
                        if (!int.TryParse(start, out _))
                        {
                            throw new Exception("From value has to be an integer!!");
                        }
                        int from = int.Parse(start);
                        products = productRepository.SearchProduct(from, FilterMode.Min);
                    }
                    else
                    {
                        // Start is empty
                        if (!int.TryParse(end, out _))
                        {
                            throw new Exception("To value has to be an integer!!");
                        }
                        int to = int.Parse(end);
                        products = productRepository.SearchProduct(to, FilterMode.Max);
                    }

                    if (products == null || products.Count() == 0)
                    {
                        MessageBox.Show("No result found!", "Search Product",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        listProducts.ItemsSource = products;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Product",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
