using CMS_Game_Engines.Common;
using CMS_Game_Engines.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CMS_Game_Engines
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string usersFilePath = "../../DataBase/users.xml"; 
        private List<User> users;
        public MainWindow()
        {
            InitializeComponent();
            LoadUsers();
            txbUsername.Text = "Input Username";
            var bc = new BrushConverter();
            txbUsername.Foreground = (Brush)bc.ConvertFrom("#717286");
            
            
        }
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveUsers();
                this.Close();
            }
            
        }
        private void LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                using (FileStream stream = new FileStream(usersFilePath, FileMode.Open))
                {
                    users = (List<User>)serializer.Deserialize(stream);
                }
            }
            else
            {
                
                users = new List<User>
                {
                    new User { Username = "admin", Password = "admin123", Role = UserRole.Admin },
                    new User { Username = "guest", Password = "guest123", Role = UserRole.Visitor }
                };
            }
        }

        private void SaveUsers()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (FileStream stream = new FileStream(usersFilePath, FileMode.Create))
            {
                serializer.Serialize(stream, users);
            }
        }
        

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = txbUsername.Text;
            string password = txbPassword.Password;

            if (ValidateFormData()) { 
                User user = users.Find(u => u.Username == username && u.Password == password);
                if (user != null)
                {
                    
                    if (user.Role == UserRole.Admin)
                    {
                        
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                    }
                    else if (user.Role == UserRole.Visitor)
                    {
                        
                        VisitorWindow visitorWindow = new VisitorWindow();
                        visitorWindow.Show();
                    }

                    
                    Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    UsernameErrorLabel.Content = "Invalid username!";
                    txbUsername.BorderBrush = Brushes.Red;
                    PasswordErrorLabel.Content = "Invalid password!";
                    txbPassword.BorderBrush = Brushes.Red;
                }
            }
        }

        private bool ValidateFormData()
        {
            bool isValid = true;

            if (txbUsername.Text.Trim().Equals(string.Empty) || txbUsername.Text.Trim().Equals("Input Username"))
            {
                isValid = false;
                UsernameErrorLabel.Content = "Field cannot be left empty!";
                txbUsername.BorderBrush = Brushes.Red;
            }
            else
            {
                UsernameErrorLabel.Content = string.Empty;
                txbUsername.BorderBrush = Brushes.Black;
            }

            if (txbPassword.SecurePassword.Length == 0)
            {
                isValid = false;
                PasswordErrorLabel.Content = "Field cannot be left empty!";
                txbPassword.BorderBrush = Brushes.Red;
            }
            else
            {
                PasswordErrorLabel.Content = string.Empty;
                txbPassword.BorderBrush = Brushes.Black;
            }


            return isValid;
        }
            private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void txbUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txbUsername.Text.Trim().Equals("Input Username"))
            {
                txbUsername.Text = "";
                txbUsername.Foreground = Brushes.Black;
            }
            UsernameErrorLabel.Content = "";
            txbUsername.BorderBrush = Brushes.Black;
        }

        private void txbUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txbUsername.Text.Trim().Equals(string.Empty))
            {
                txbUsername.Text = "Input Username";
                var bc = new BrushConverter();
                txbUsername.Foreground = (Brush)bc.ConvertFrom("#717286");
                
            }

        }

        private void txbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordErrorLabel.Content = "";
            txbPassword.BorderBrush = Brushes.Black;

        }

        private void txbPassword_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
