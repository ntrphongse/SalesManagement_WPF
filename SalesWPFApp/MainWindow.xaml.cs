using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using SalesWPFApp.MemberUI;
using SalesWPFApp.OrderUI;
using SalesWPFApp.ProductUI;
using System.Windows;

namespace SalesWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Member loginMember;
        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        public MainWindow(Member loginMember,
            IMemberRepository memberRepository, IProductRepository productRepository,
            IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.loginMember = loginMember;
            this.memberRepository = memberRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
        }

        private void btnMemberManagement_Click(object sender, RoutedEventArgs e)
        {
            MemberManagementWindow memberManagement = new MemberManagementWindow(loginMember,
                memberRepository,
                productRepository, orderRepository, orderDetailRepository);
            memberManagement.Closed += (s, args) => this.Close();
            this.Hide();
            memberManagement.Show();
        }

        private void btnProductManagement_Click(object sender, RoutedEventArgs e)
        {
            ProductManagementWindow productManagement = new ProductManagementWindow(loginMember, productRepository,
                memberRepository, orderRepository, orderDetailRepository);
            productManagement.Closed += (s, args) => this.Close();
            this.Hide();
            productManagement.Show();
        }

        private void btnOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            OrderManagementWindow orderManagement = new OrderManagementWindow(loginMember,
                orderRepository, orderDetailRepository,
                 memberRepository, productRepository);
            orderManagement.Closed += (s, args) => this.Close();
            this.Hide();
            orderManagement.Show();
        }
    }
}
