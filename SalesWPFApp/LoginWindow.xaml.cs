using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using SalesWPFApp.MemberUI;
using System;
using System.Windows;

namespace SalesWPFApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        public LoginWindow(IMemberRepository memberRepository, IProductRepository productRepository,
            IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            InitializeComponent();
            this.Title += $" | {Configuration.AppName}";
            this.memberRepository = memberRepository;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = txtEmail.Text;
                string password = txtPassword.Password;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("Email and Password should not be empty!!");
                }

                Member member = memberRepository.Login(email, password);

                if (member == null)
                {
                    throw new Exception("Login information is not correct. Please check again!");
                }
                else
                {
                    MessageBox.Show($"Login successfully! Welcome {member.Email}...", "Login",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    if (member.Email.Equals("admin@fstore.com"))
                    {
                        MainWindow mainWindow = new MainWindow(member,
                            memberRepository, productRepository,
                            orderRepository, orderDetailRepository);
                        this.Hide();
                        mainWindow.Closed += (s, args) => this.Close();
                        mainWindow.Show();
                    }
                    else
                    {
                        MemberDetailsWindow memberDetailsWindow = new MemberDetailsWindow(PopupMode.Update,
                            memberRepository, member, member,
                            orderRepository, orderDetailRepository,
                            productRepository);
                        this.Hide();
                        memberDetailsWindow.Closed += (s, args) => this.Close();
                        memberDetailsWindow.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to exit?", "Login",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

    }
}
