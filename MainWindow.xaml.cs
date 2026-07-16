using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MeatCafeWpf
{
    public partial class MainWindow : Window
    {
        static string[] meatItems = { "Steak", "Ribs", "Wings", "Burger", "Pork Chop", "Beef Cutlet", "Lamb Rack", "Meatball", "Sausage" };
        static double[] meatPrices = { 25.50, 18.00, 12.00, 13.95, 15.00, 14.50, 22.00, 10.00, 9.50 };

        static string[] drinkItems = { "Cola", "Water", "Coffee", "Tea", "Juice", "Lemonade", "Beer", "Wine", "Milkshake" };
        static double[] drinkPrices = { 3.00, 1.50, 4.00, 2.50, 3.50, 3.00, 6.00, 8.00, 5.00 };

        static List<string> orderNames = new List<string>();
        static List<double> orderPrices = new List<double>();
        static List<int> orderQuantities = new List<int>();

        static string currentOrderNumber = "";

        private string[] currentMenuItems;
        private double[] currentMenuPrices;
        private bool isFoodMenu;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TxtOrderNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            string input = txtOrderNumber.Text.Trim();
            if (string.IsNullOrEmpty(input) || input.Length > 10 || !input.All(char.IsDigit))
            {
                MessageBox.Show("Invalid input. Use digits only (max 10).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (input == "0")
            {
                Application.Current.Shutdown();
                return;
            }

            currentOrderNumber = input;
            lblOrderNumber.Text = currentOrderNumber;

            pnlOrderNumber.Visibility = Visibility.Collapsed;
            grdMain.Visibility = Visibility.Visible;
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            HideAllPanels();
            pnlMainMenu.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Collapsed;
        }

        private void HideAllPanels()
        {
            pnlMainMenu.Visibility = Visibility.Collapsed;
            pnlMenuItems.Visibility = Visibility.Collapsed;
            pnlViewOrder.Visibility = Visibility.Collapsed;
            pnlRemoveItem.Visibility = Visibility.Collapsed;
            pnlLoadOrder.Visibility = Visibility.Collapsed;
            pnlPlaceOrder.Visibility = Visibility.Collapsed;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ShowMainMenu();
        }

        private void BtnFoodMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowMenuPanel("Food Menu", meatItems, meatPrices, true);
        }

        private void BtnDrinkMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowMenuPanel("Drink Menu", drinkItems, drinkPrices, false);
        }

        private void ShowMenuPanel(string title, string[] items, double[] prices, bool isFood)
        {
            HideAllPanels();
            lblMenuTitle.Text = title;
            currentMenuItems = items;
            currentMenuPrices = prices;
            isFoodMenu = isFood;

            lstMenuItems.Items.Clear();
            for (int i = 0; i < items.Length; i++)
            {
                lstMenuItems.Items.Add(new { Name = items[i], Info = $"{prices[i]:F2}$" });
            }

            pnlMenuItems.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Visible;
        }

        private void BtnAddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstMenuItems.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an item first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int idx = lstMenuItems.SelectedIndex;
            string name = currentMenuItems[idx];
            double price = currentMenuPrices[idx];

            int index = orderNames.IndexOf(name);
            if (index != -1)
                orderQuantities[index]++;
            else
            {
                orderNames.Add(name);
                orderPrices.Add(price);
                orderQuantities.Add(1);
            }

            MessageBox.Show($"{name} added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnViewOrder_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            lstOrderView.Items.Clear();
            if (orderNames.Count == 0)
            {
                lstOrderView.Items.Add(new { Name = "Order is empty.", Info = "" });
            }
            else
            {
                for (int i = 0; i < orderNames.Count; i++)
                {
                    lstOrderView.Items.Add(new
                    {
                        Name = $"{i + 1}. {orderNames[i]} (x{orderQuantities[i]})",
                        Info = $"{(orderPrices[i] * orderQuantities[i]):F2}$"
                    });
                }
            }
            pnlViewOrder.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Visible;
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            if (orderNames.Count == 0)
            {
                MessageBox.Show("Order is empty, nothing to remove.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowMainMenu();
                return;
            }

            lstRemoveItems.Items.Clear();
            for (int i = 0; i < orderNames.Count; i++)
            {
                lstRemoveItems.Items.Add(new
                {
                    Name = $"{i + 1}. {orderNames[i]}",
                    Info = $"(x{orderQuantities[i]})"
                });
            }
            pnlRemoveItem.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Visible;
        }

        private void BtnRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lstRemoveItems.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an item to remove.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int idx = lstRemoveItems.SelectedIndex;
            if (orderQuantities[idx] > 1)
                orderQuantities[idx]--;
            else
            {
                orderNames.RemoveAt(idx);
                orderPrices.RemoveAt(idx);
                orderQuantities.RemoveAt(idx);
            }

            MessageBox.Show("Item updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            lstRemoveItems.Items.Clear();
            if (orderNames.Count > 0)
            {
                for (int i = 0; i < orderNames.Count; i++)
                {
                    lstRemoveItems.Items.Add(new
                    {
                        Name = $"{i + 1}. {orderNames[i]}",
                        Info = $"(x{orderQuantities[i]})"
                    });
                }
            }
            else
            {
                ShowMainMenu();
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (orderNames.Count == 0)
            {
                MessageBox.Show("Error: Order is already empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            orderNames.Clear();
            orderPrices.Clear();
            orderQuantities.Clear();
            MessageBox.Show("Order cleared successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSaveToFile_Click(object sender, RoutedEventArgs e)
        {
            if (orderNames.Count == 0)
            {
                MessageBox.Show("Error: Cannot save empty order.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string filename = $"Menu-{currentOrderNumber}.txt";
            File.WriteAllLines(filename,
                orderNames.Select((n, i) => $"{n}|{orderQuantities[i]}|{orderPrices[i]}"));
            MessageBox.Show($"Saved to {filename}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            pnlLoadOrder.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Visible;
        }

        private void BtnLoadOrder_Click(object sender, RoutedEventArgs e)
        {
            string num = txtLoadOrderNumber.Text.Trim();
            if (string.IsNullOrEmpty(num) || num.Length > 10 || !num.All(char.IsDigit))
            {
                MessageBox.Show("Invalid order number. Use digits only (max 10).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string filename = $"Menu-{num}.txt";
            if (File.Exists(filename))
            {
                orderNames.Clear();
                orderPrices.Clear();
                orderQuantities.Clear();
                var lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    orderNames.Add(parts[0]);
                    orderQuantities.Add(int.Parse(parts[1]));
                    orderPrices.Add(double.Parse(parts[2]));
                }

                MessageBox.Show("File loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowMainMenu();
            }
            else
            {
                MessageBox.Show("Error: File not found. You haven't saved that order yet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double netTotal;

        private void BtnPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (orderNames.Count == 0)
            {
                MessageBox.Show("Error: Cannot place empty order.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            HideAllPanels();
            pnlPlaceOrder.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Visible;

            pnlPlaceOrderStep1.Visibility = Visibility.Visible;
            pnlPlaceOrderStep2.Visibility = Visibility.Collapsed;
            pnlPlaceOrderStep3.Visibility = Visibility.Collapsed;

            lstPlaceOrderItems.Items.Clear();
            netTotal = 0;
            for (int i = 0; i < orderNames.Count; i++)
            {
                double lineTotal = orderPrices[i] * orderQuantities[i];
                lstPlaceOrderItems.Items.Add(new
                {
                    Name = $"{orderNames[i]} (x{orderQuantities[i]})",
                    Info = $"{lineTotal:F2}$"
                });
                netTotal += lineTotal;
            }
            lblNetTotal.Text = $"Net Total: {netTotal:F2}$";
        }

        private void BtnPlaceOrderProceed_Click(object sender, RoutedEventArgs e)
        {
            pnlPlaceOrderStep1.Visibility = Visibility.Collapsed;
            pnlPlaceOrderStep2.Visibility = Visibility.Visible;
            pnlPlaceOrderStep3.Visibility = Visibility.Collapsed;

            rbTipYes.IsChecked = false;
            rbTipNo.IsChecked = false;
            pnlTipDetails.Visibility = Visibility.Collapsed;
            txtTipValue.Text = "";
        }

        private void TipOptionChanged(object sender, RoutedEventArgs e)
        {
            pnlTipDetails.Visibility = rbTipYes.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnCalculateFinal_Click(object sender, RoutedEventArgs e)
        {
            double tip = 0;

            if (rbTipYes.IsChecked == true)
            {
                if (rbPercent.IsChecked == false && rbFixed.IsChecked == false)
                {
                    MessageBox.Show("Error: Please select tip type (Percent or Fixed).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string tipInput = txtTipValue.Text.Trim();
                if (!double.TryParse(tipInput, out double val) || val <= 0)
                {
                    MessageBox.Show("Error: Invalid input. Tip must be a positive number greater than zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                tip = (rbPercent.IsChecked == true) ? netTotal * (val / 100) : val;
            }
            else if (rbTipNo.IsChecked == true)
            {
                tip = 0;
            }
            else
            {
                MessageBox.Show("Error: Please choose whether to leave a tip.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double gst = netTotal * 0.05;
            double total = netTotal + tip + gst;

            pnlPlaceOrderStep1.Visibility = Visibility.Collapsed;
            pnlPlaceOrderStep2.Visibility = Visibility.Collapsed;
            pnlPlaceOrderStep3.Visibility = Visibility.Visible;

            lstFinalBillItems.Items.Clear();
            for (int i = 0; i < orderNames.Count; i++)
            {
                double lineTotal = orderPrices[i] * orderQuantities[i];
                lstFinalBillItems.Items.Add(new
                {
                    Name = $"{orderNames[i]} (x{orderQuantities[i]})",
                    Info = $"{lineTotal:F2}$"
                });
            }

            lblFinalNet.Text = $"Net: {netTotal,33:F2}$";
            lblFinalTip.Text = $"Tip: {tip,34:F2}$";
            lblFinalGST.Text = $"GST: {gst,33:F2}$";
            lblFinalTotal.Text = $"Total: {total,32:F2}$";

            rbTakeaway.IsChecked = false;
            rbDinein.IsChecked = false;

            Tag = total;
        }

        private void BtnFinalPlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (rbTakeaway.IsChecked == false && rbDinein.IsChecked == false)
            {
                MessageBox.Show("Error: Please select Takeaway or Dine-in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double waitTime = orderNames.Count * 5;
            MessageBox.Show($"Thank you for your order! Wait time: {waitTime} minutes.", "Order Placed",
                MessageBoxButton.OK, MessageBoxImage.Information);

            orderNames.Clear();
            orderPrices.Clear();
            orderQuantities.Clear();

            ShowMainMenu();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}