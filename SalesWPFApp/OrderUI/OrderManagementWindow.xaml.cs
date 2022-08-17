using AppSettings;
using BusinessObject;
using DataAccess.Repository;
using SalesWPFApp.MemberUI;
using SalesWPFApp.ProductUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SalesWPFApp.OrderUI
{
    /// <summary>
    /// Interaction logic for OrderManagementWindow.xaml
    /// </summary>
    public partial class OrderManagementWindow : Window
    {
        private Member loginMember;
        private IOrderRepository orderRepository;
        private IMemberRepository memberRepository;
        private IProductRepository productRepository;
        private IOrderDetailRepository orderDetailRepository;
        IEnumerable<Order> dataSource;
        public OrderManagementWindow(Member loginMember, IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IMemberRepository memberRepository, IProductRepository productRepository)
        {
            InitializeComponent();
            Title += $" | {Configuration.AppName}";
            this.loginMember = loginMember;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            this.memberRepository = memberRepository;
            this.productRepository = productRepository;
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
                if (!loginMember.Email.Equals("admin@fstore.com"))
                {
                    btnNew.Visibility = Visibility.Hidden;
                    btnDelete.Visibility = Visibility.Hidden;
                    btnExport.Visibility = Visibility.Hidden;
                    topMenuAdmin.Visibility = Visibility.Hidden;
                    topMenuMember.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Order Management", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void menuProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductManagementWindow productManagement = new ProductManagementWindow(loginMember,
                productRepository, memberRepository,
                orderRepository, orderDetailRepository);
            productManagement.Closed += (s, args) => this.Close();
            this.Hide();
            productManagement.Show();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you really want to exit?", "Order Management",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Order selectedOrder = ((ListViewItem)sender).Content as Order;
            OrderInformationWindow orderInformationWindow = new OrderInformationWindow(
                PopupMode.Update, loginMember,
                orderRepository, orderDetailRepository, memberRepository,
                productRepository, selectedOrder);
            if (orderInformationWindow.ShowDialog() != false)
            {
                LoadOrdersList();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadOrdersList();
        }

        private void LoadOrdersList()
        {
            dataSource = new List<Order>();
            try
            {
                if (!loginMember.Email.Equals("admin@fstore.com"))
                {
                    dataSource = orderRepository.GetOrders(loginMember.MemberId);
                }
                else
                {
                    dataSource = orderRepository.GetOrders();
                }
                listOrders.ItemsSource = null;
                listOrders.ItemsSource = dataSource;
                if (!loginMember.Email.Equals("admin@fstore.com")
                    && !dataSource.Any())
                {
                    MessageBox.Show("You don't have any order yet!",
                        "Load Order", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Order Management", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            OrderInformationWindow orderInformationWindow = new OrderInformationWindow(PopupMode.Add,
               loginMember,
                orderRepository, orderDetailRepository,
                memberRepository, productRepository);
            if (orderInformationWindow.ShowDialog() != false)
            {
                LoadOrdersList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtOrderId.Text))
                {
                    throw new Exception("No order is selected!!");
                }
                MessageBoxResult result = MessageBox.Show($"Do you really want to delete the order with the ID {txtOrderId.Text}??",
                    "Delete Order", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    orderRepository.Delete(int.Parse(txtOrderId.Text));
                    MessageBox.Show("Delete order successfully!!",
                        "Delete Order", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrdersList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Delete order",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? startDate = txtStartDate.Value;
                DateTime? endDate = txtEndDate.Value;
                //IEnumerable<Order> orders = null;
                if (startDate == null && endDate == null)
                {
                    throw new Exception("Please enter at least one value of date to search...");
                }
                if (startDate != null && endDate != null)
                {
                    // Both are enterd
                    if (DateTime.Compare(startDate.Value, endDate.Value) > 0)
                    {
                        DateTime temp = startDate.Value;
                        startDate = endDate;
                        endDate = temp;
                    }
                    dataSource = orderRepository.SearchOrder(startDate.Value, endDate.Value);
                }
                else if (startDate != null)
                {
                    // End if empty
                    dataSource = orderRepository.SearchOrder(startDate.Value, DataAccess.DAO.FilterMode.Min);
                }
                else
                {
                    dataSource = orderRepository.SearchOrder(endDate.Value, DataAccess.DAO.FilterMode.Min);
                }
                if (dataSource == null || !dataSource.Any())
                {
                    MessageBox.Show("No result found!", "Search Order",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    listOrders.ItemsSource = dataSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Search Order", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tableOfData = ToDataTable(ConvertExport(dataSource));
                DataSet ds = new DataSet();
                ds.Tables.Add(tableOfData);
                ExcelLibrary.DataSetHelper.CreateWorkbook("Report.xls", ds);

                string path = Environment.CurrentDirectory + "\\";
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(path + "Report.xls")
                {
                    UseShellExecute = true
                };
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Export data", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        #region Private Methods for exporting data
        private DataTable CreateDataTable<T>()
        {
            DataTable dt = new DataTable();
            PropertyInfo[] piT = typeof(T).GetProperties();
            foreach (PropertyInfo pi in piT)
            {
                Type propertyType = null;
                if (pi.PropertyType.IsGenericType)
                {
                    propertyType = pi.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    propertyType = pi.PropertyType;
                }
                DataColumn dc = new DataColumn(pi.GetCustomAttribute<DisplayNameAttribute>(false).DisplayName, propertyType);
                if (pi.CanRead)
                {
                    dt.Columns.Add(dc);
                }
            }
            return dt;
        }

        private IEnumerable<OrderExportData> ConvertExport(IEnumerable<Order> orders)
        {
            orders = orders.OrderByDescending(order => order.OrderDate);
            IEnumerable<OrderExportData> datas = new List<OrderExportData>();
            foreach (Order order in orders)
            {
                decimal orderTotal = 0;
                foreach (OrderDetail orderDetail in order.OrderDetails)
                {
                    orderTotal += orderDetail.UnitPrice
                        * orderDetail.Quantity
                        * (1 - ((decimal)orderDetail.Discount / 100));
                }
                datas = datas.Append(new OrderExportData()
                {
                    OrderID = order.OrderId,
                    MemberEmail = order.Member.Email,
                    OrderDate = order.OrderDate.ToShortDateString() + " " + order.OrderDate.ToShortTimeString(),
                    RequiredDate = order.RequiredDate.Value.ToShortDateString() + " " + order.RequiredDate.Value.ToShortTimeString(),
                    ShippedDate = order.ShippedDate.Value.ToShortDateString() + " " + order.ShippedDate.Value.ToShortTimeString(),
                    Freight = Math.Round(order.Freight.Value, 2),
                    OrderTotal = Math.Round(orderTotal, 2)
                });
            }
            return datas;
        }

        private DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var table = CreateDataTable<T>();
            PropertyInfo[] piT = typeof(T).GetProperties();
            foreach (var item in items)
            {
                var dr = table.NewRow();
                for (int property = 0; property < table.Columns.Count; property++)
                {
                    if (piT[property].CanRead)
                    {
                        dr[property] = piT[property].GetValue(item, null);
                    }
                }
                table.Rows.Add(dr);
            }
            return table;
        }
        #endregion

        private void menuProfile_Click(object sender, RoutedEventArgs e)
        {
            MemberDetailsWindow memberDetailsWindow = new MemberDetailsWindow(PopupMode.Update,
                            memberRepository, loginMember, loginMember,
                            orderRepository, orderDetailRepository,
                            productRepository);
            this.Hide();
            memberDetailsWindow.Closed += (s, args) => this.Close();
            memberDetailsWindow.Show();
        }
    }

    internal record OrderExportData
    {
        [DisplayName("Order ID")]
        public int OrderID { get; set; }

        [DisplayName("Member Email")]
        public string MemberEmail { get; set; }

        [DisplayName("Order Date")]
        public string OrderDate { get; set; }

        [DisplayName("Shipped Date")]
        public string ShippedDate { get; set; }

        [DisplayName("Required Date")]
        public string RequiredDate { get; set; }

        [DisplayName("Freight")]
        public decimal? Freight { get; set; }

        [DisplayName("Order Total")]
        public decimal OrderTotal { get; set; }
    }
}
