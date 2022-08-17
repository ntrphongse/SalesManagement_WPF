using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using SalesWPFApp.OrderUI;
using System;
using System.Windows;
using Validation;

namespace SalesWPFApp.MemberUI
{
    /// <summary>
    /// Interaction logic for MemberDetailsWindow.xaml
    /// </summary>
    public partial class MemberDetailsWindow : Window
    {
        private Member selectedMember;
        private Member loginMember;
        private PopupMode mode;
        private IMemberRepository memberRepository;
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;
        private IProductRepository productRepository;
        public MemberDetailsWindow(PopupMode mode,
            IMemberRepository memberRepository,
            Member loginMember,
            Member selectedMember = null,
            IOrderRepository orderRepository = null,
            IOrderDetailRepository orderDetailRepository = null,
            IProductRepository productRepository = null)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.mode = mode;
            this.memberRepository = memberRepository;
            this.loginMember = loginMember;
            this.selectedMember = selectedMember;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            this.productRepository = productRepository;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mode == PopupMode.Add)
            {
                selectedMember = new Member()
                {
                    MemberId = memberRepository.GetNextMemberId(),
                    Email = "",
                    Password = "",
                    CompanyName = "",
                    City = "",
                    Country = ""
                };
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Hidden;
            }
            else if (mode == PopupMode.Update)
            {
                txtEmail.IsReadOnly = true;
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
            txtMemberId.Text = selectedMember.MemberId.ToString();
            txtEmail.Text = selectedMember.Email;
            txtPassword.Password = selectedMember.Password;
            txtConfirm.Password = selectedMember.Password;
            txtCompany.Text = selectedMember.CompanyName;
            txtCity.Text = selectedMember.City;
            txtCountry.Text = selectedMember.Country;
            if (!loginMember.Email.Equals("admin@fstore.com"))
            {
                topMenuDock.Visibility = Visibility.Visible;
                btnUpdate.Content = "Update your profile";
                btnUpdate.Width = 120;
            }
            else
            {
                topMenuDock.Visibility = Visibility.Hidden;
            }
        }

        private Member GetMemberObject() => new Member
        {
            MemberId = int.Parse(txtMemberId.Text),
            Email = txtEmail.Text.Trim(),
            Password = txtPassword.Password,
            CompanyName = txtCompany.Text.Trim(),
            City = txtCity.Text.Trim(),
            Country = txtCountry.Text.Trim()
        };

        private void CheckObject()
        {
            if (string.IsNullOrEmpty(txtMemberId.Text) || txtMemberId.Text.Equals("-1"))
            {
                throw new Exception("Member ID cannot be empty or it is invalid! This must be a program error... " +
                    "Please contact to the developer for more information");
            }
            if (!StringValidation.IsEmail(txtEmail.Text.Trim()))
            {
                throw new Exception($"The email '{txtEmail.Text}' seems not to be a valid email address!");
            }
            if (!StringValidation.CheckLength(txtPassword.Password, 6, StringValidation.StringMode.MinLength))
            {
                throw new Exception("Password has to be at least 6 characters!");
            }
            if (!txtPassword.Password.Equals(txtConfirm.Password))
            {
                throw new Exception("Confirm and Password has to be the same!");
            }
            if (string.IsNullOrEmpty(txtCompany.Text))
            {
                throw new Exception("Company name cannot be empty!");
            }
            if (string.IsNullOrEmpty(txtCity.Text))
            {
                throw new Exception("City cannot be empty!");
            }
            if (string.IsNullOrEmpty(txtCountry.Text))
            {
                throw new Exception("Country cannot be empty!");
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                Member newMember = memberRepository.Add(GetMemberObject());
                if (newMember == null)
                {
                    throw new Exception("An error has occured when adding a new member! Please try again...");
                }
                MessageBox.Show($"Member {newMember.Email} is added successfully!", "Add new Member", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                //this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add new Member", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckObject();
                Member updatedMember = memberRepository.Update(GetMemberObject());
                if (updatedMember == null)
                {
                    throw new Exception("An error has occured when updating a new member! Please try again...");
                }
                if (!loginMember.Email.Equals("admin@fstore.com"))
                {
                    MessageBox.Show($"Your profile has been updated successfully!", "Update Member",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Member {updatedMember.Email} is updated successfully!",
                        "Update Member", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                }
                //this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Member", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (!loginMember.Email.Equals("admin@fstore.com"))
            {
                Exit();
            }
        }

        private void menuOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderManagementWindow orderManagement = new OrderManagementWindow(selectedMember,
                orderRepository, orderDetailRepository,
                memberRepository, productRepository);
            orderManagement.Closed += (s, args) => this.Close();
            this.Hide();
            orderManagement.Show();
        }
        private void Exit()
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to exit?", "Update profile",
                           MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }
    }
}
