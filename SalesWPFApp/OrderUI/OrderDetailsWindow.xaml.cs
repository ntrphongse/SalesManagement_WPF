using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SalesWPFApp.OrderUI
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        private Member loginMember;
        private int orderId;
        private IOrderDetailRepository orderDetailRepository;
        private IProductRepository productRepository;
        private IEnumerable<OrderDetail> dataSource;

        public OrderDetailsWindow(Member loginMember, int orderId,
            IOrderDetailRepository orderDetailRepository,
            IProductRepository productRepository)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.loginMember = loginMember;
            this.orderId = orderId;
            this.orderDetailRepository = orderDetailRepository;
            this.productRepository = productRepository;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (loginMember == null)
                {
                    throw new Exception("Login information is invalid! Please exit the application and try again...");
                }

                LoadOrderDetails();

                if (!loginMember.Email.Equals("admin@fstore.com"))
                {
                    btnAddProduct.Visibility = Visibility.Hidden;
                    btnDeleteProduct.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Order Details",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderDetails()
        {
            dataSource = new List<OrderDetail>();
            try
            {
                listOrderDetails.ItemsSource = null;
                dataSource = orderDetailRepository.GetOrderDetails(orderId);
                listOrderDetails.ItemsSource = dataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Order Details",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            OrderDetailsInformationWindow orderDetailsInformationWindow = new OrderDetailsInformationWindow(
                PopupMode.Add, orderDetailRepository,
                productRepository, orderId);
            if (orderDetailsInformationWindow.ShowDialog() != false)
            {
                LoadOrderDetails();
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtOrderId.Text))
                {
                    throw new Exception("No order detail is selected!!");
                }
                MessageBoxResult result = MessageBox.Show($"Do you really want to delete this order detail? \n" +
                    $"- Order ID: {orderId}\n" +
                    $"- Product: {txtProduct.Text}", "Delete Order Details",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    orderDetailRepository.Delete(int.Parse(txtOrderId.Text),
                        productRepository.GetProduct(txtProduct.Text).ProductId);
                    MessageBox.Show("Delete order detail successfully!!",
                        "Delete Order", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrderDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete Order Details",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (loginMember.Email.Equals("admin@fstore.com"))
            {

                OrderDetail selectedOrderDetail = ((ListViewItem)sender).Content as OrderDetail;
                OrderDetailsInformationWindow orderDetailsInformationWindow = new OrderDetailsInformationWindow(
                    PopupMode.Update,
                    orderDetailRepository, productRepository,
                    selectedOrderDetail.OrderId, selectedOrderDetail);
                if (orderDetailsInformationWindow.ShowDialog() != false)
                {
                    LoadOrderDetails();
                }
            }
        }

    }
}
