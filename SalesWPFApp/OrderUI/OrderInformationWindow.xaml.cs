using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Validation;

namespace SalesWPFApp.OrderUI
{
    /// <summary>
    /// Interaction logic for OrderInformationWindow.xaml
    /// </summary>
    public partial class OrderInformationWindow : Window
    {
        private Order selectedOrder;
        private Member loginMember;
        private PopupMode mode;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private IEnumerable<Member> members;
        public OrderInformationWindow(PopupMode mode, Member loginMember,
            IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository,
            IMemberRepository memberRepository, IProductRepository productRepository,
            Order selectedOrder = null)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.mode = mode;
            this.loginMember = loginMember;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            this.memberRepository = memberRepository;
            this.productRepository = productRepository;
            this.selectedOrder = selectedOrder;
        }
        private void LoadMemberList()
        {
            try
            {
                cboMember.ItemsSource = null;
                members = memberRepository.GetMembers();
                if (members == null || members.Count() == 0)
                {
                    throw new Exception("There is no member to load..." +
                        " Please try to add at least one member to operate this action!");
                }
                List<string> memberEmailList = new List<string>();
                foreach (Member member in members)
                {
                    memberEmailList.Add(member.Email);
                }
                cboMember.ItemsSource = memberEmailList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loading member list...",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMemberList();
            if (mode == PopupMode.Add)
            {
                selectedOrder = new Order()
                {
                    OrderId = orderRepository.GetNextOderId(),
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now,
                    ShippedDate = DateTime.Now,
                    Freight = 0
                };
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
                btnEditOrderItem.IsEnabled = false;
            }
            else if (mode == PopupMode.Update)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
            if (!loginMember.Email.Equals("admin@fstore.com"))
            {
                btnUpdate.Visibility = Visibility.Hidden;
                btnEditOrderItem.Content = "View Order's Items";
                cboMember.IsEnabled = false;
                txtOrderDate.IsReadOnly = true;
                txtRequiredDate.IsReadOnly = true;
                txtShippedDate.IsReadOnly = true;
                txtFreight.IsReadOnly = true;
            }
            LoadOrder();
        }

        private Order GetOrderObject() => new Order
        {
            OrderId = int.Parse(txtOrderId.Text),
            MemberId = memberRepository.GetMember(cboMember.SelectedItem.ToString()).MemberId,
            OrderDate = txtOrderDate.Value.Value,
            RequiredDate = txtRequiredDate.Value,
            ShippedDate = txtShippedDate.Value,
            Freight = decimal.Parse(txtFreight.Text)
        };

        private void CheckObject()
        {
            if (string.IsNullOrEmpty(txtOrderId.Text) ||
                txtOrderId.Text.Equals("-1"))
            {
                throw new Exception("Order ID cannot be empty or it is invalid!!" +
                    " Please contact the developer for more information!");
            }
            if (cboMember.SelectedItem == null ||
                string.IsNullOrEmpty(cboMember.SelectedItem.ToString()))
            {
                throw new Exception("You have to choose a member for this order!!");
            }
            if (txtOrderDate.Value == null)
            {
                throw new Exception("Order Date cannot be empty!!");
            }
            if (txtRequiredDate.Value == null)
            {
                throw new Exception("Required Date cannot be empty!!");
            }
            if (txtShippedDate.Value == null)
            {
                throw new Exception("Shipped Date cannot be empty!!!");
            }
            if (!NumberValidation.IsDecimal(txtFreight.Text) ||
                decimal.Parse(txtFreight.Text) <= 0)
            {
                throw new Exception("Freight has to be a positive number!!");
            }
        }
        private void LoadOrder()
        {
            if (mode == PopupMode.Update)
            {
                selectedOrder = orderRepository.GetOrder(selectedOrder.OrderId);
                if (selectedOrder.Member != null)
                {
                    cboMember.SelectedItem = selectedOrder.Member.Email;
                }
            }
            txtOrderId.Text = selectedOrder.OrderId.ToString();
            txtOrderDate.Value = selectedOrder.OrderDate;
            txtRequiredDate.Value = selectedOrder.RequiredDate;
            txtShippedDate.Value = selectedOrder.ShippedDate;
            txtFreight.Text = selectedOrder.Freight.ToString();

            int orderItem = selectedOrder.OrderDetails.Count;
            lbOrderItems.Content = $"{orderItem} items";
        }
        private void btnEditOrderItem_Click(object sender, RoutedEventArgs e)
        {
            OrderDetailsWindow orderDetailsWindow = new OrderDetailsWindow(
                loginMember, selectedOrder.OrderId,
                orderDetailRepository, productRepository);
            orderDetailsWindow.ShowDialog();

            LoadOrder();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                Order newOrder = orderRepository.Add(GetOrderObject());
                if (newOrder == null)
                {
                    throw new Exception("An error has occured when adding a new order! Please try again...");
                }
                MessageBox.Show($"Order is added succesffully!!", "Add new Order",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add new Order",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                Order newOrder = orderRepository.Update(GetOrderObject());
                if (newOrder == null)
                {
                    throw new Exception("An error has occured when updating the order! Please try again...");
                }
                MessageBox.Show($"Order is updated succesffully!!", "Update Order",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Order",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
