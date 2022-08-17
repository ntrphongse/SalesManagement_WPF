using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using SalesWPFApp.OrderUI;
using SalesWPFApp.ProductUI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SalesWPFApp.MemberUI
{
    /// <summary>
    /// Interaction logic for MemberManagementWindow.xaml
    /// </summary>
    public partial class MemberManagementWindow : Window
    {
        private Member loginMember;
        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        IEnumerable<Member> dataSource;
        public MemberManagementWindow(Member loginMember,
            IMemberRepository memberRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.loginMember = loginMember;
            this.memberRepository = memberRepository;
            this.productRepository = productRepository;
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
                MessageBox.Show(ex.Message, "Member Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadMemberList();
        }

        private void LoadMemberList()
        {
            dataSource = new List<Member>();
            try
            {
                dataSource = memberRepository.GetMembers();

                listMembers.ItemsSource = null;
                listMembers.ItemsSource = dataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Member Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            MemberDetailsWindow memberDetailsWindow = new MemberDetailsWindow(PopupMode.Add, memberRepository, loginMember);

            if (memberDetailsWindow.ShowDialog() != false)
            {
                LoadMemberList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMemberId.Text))
                {
                    throw new Exception("No member is selected!!");
                }
                MessageBoxResult result = MessageBox.Show($"Do you really want to delete member with the ID {txtMemberId.Text} ?", "Delete member",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    memberRepository.Delete(int.Parse(txtMemberId.Text));
                    MessageBox.Show($"Delete member successfully!!", "Delete member", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMemberList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete Member", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Exit()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to exit?", "Member Management",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e) => Exit();

        private void menuProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductManagementWindow productManagement = new ProductManagementWindow(loginMember,
                productRepository, memberRepository,
                orderRepository, orderDetailRepository);
            productManagement.Closed += (s, args) => this.Close();
            this.Hide();
            productManagement.Show();
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

        private void listMembers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Member selectedItem = ((ListViewItem)sender).Content as Member;
            MemberDetailsWindow memberDetailsWindow = new MemberDetailsWindow(PopupMode.Update, memberRepository, loginMember, selectedItem);

            if (memberDetailsWindow.ShowDialog() != false)
            {
                LoadMemberList();
            }
        }
    }
}
