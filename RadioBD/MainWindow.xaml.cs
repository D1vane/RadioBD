using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;

namespace RadioBD
{
    /// 
    /// Окно авторизации
    /// 
    public partial class MainWindow : Window
    {
        int userId;
        public MainWindow()
        {
            InitializeComponent();
            ImgLockPass.Visibility = Visibility.Hidden;
            TbPassword.Visibility = Visibility.Hidden;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
         
        }

        private void BtEnter_Click(object sender, RoutedEventArgs e)
        {
            var loginUser = TbLogin.Text;
            var passwordUser = "";
            // Хэширование пароля
            if (PbPassword.Password.ToString() == "")
            {
                passwordUser = MD5_hash.HashPassword(TbPassword.Text);
            }
            else
            {
                passwordUser = MD5_hash.HashPassword(PbPassword.Password.ToString());
            }

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataTable dataTable = new DataTable();
            OleDbCommand command = null;
            string askystring = $"SELECT IDПользователя, Логин, Пароль, Роль FROM register WHERE Логин = '{loginUser}'";

            object result, obj = null;
            Assembly asm;
            Type t = null;
            try
            {
                asm = Assembly.Load("DataBase");
                t = asm.GetType("DataBase.Data");
                obj = t.GetConstructor(new Type[0]).Invoke(new object[0]);
                result = t.GetMethod("Connect").Invoke(obj, null);
                result = t.GetMethod("GetConnect").Invoke(obj, null);
                command = new OleDbCommand(askystring, (OleDbConnection)result);
            }
            catch (Exception)
            {
                MessageBox.Show("Something wrong in Authorization");
            }
             

            adapter.SelectCommand = command;
            adapter.Fill(dataTable);

            if (dataTable.Rows.Count == 1)
            {
                if (dataTable.Rows[0].ItemArray[2].ToString() == passwordUser)
                {
                    checkRights user = new checkRights(dataTable.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(dataTable.Rows[0].ItemArray[3]));
                    MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                    TbLogin.Clear();
                    TbPassword.Clear();
                    PbPassword.Clear();
                    MenuWindow menuWindow = new MenuWindow(user);
                    menuWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Неверный пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Такого аккаунта не существует!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            result = t.GetMethod("Disconnect").Invoke(obj, null);


        }

        private void ImgUnlockPass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImgUnlockPass.Visibility = Visibility.Hidden;
            ImgLockPass.Visibility = Visibility.Visible;
            TbPassword.Text = PbPassword.Password.ToString();
            PbPassword.Clear();
            PbPassword.Visibility = Visibility.Hidden;
            TbPassword.Visibility = Visibility.Visible;

        }

        private void ImgLockPass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImgLockPass.Visibility = Visibility.Hidden;
            ImgUnlockPass.Visibility = Visibility.Visible;
            PbPassword.Password = TbPassword.Text;
            TbPassword.Text = "";
            TbPassword.Visibility = Visibility.Hidden;
            PbPassword.Visibility = Visibility.Visible;
        }

        private void TbRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow regwindow = new RegistrationWindow();
            regwindow.ShowDialog();
        }

        private void Grid_LayoutUpdated(object sender, EventArgs e)
        {
            if (TbLogin.IsFocused || TbPassword.IsFocused || PbPassword.IsFocused)
            {
                LLanguage.Content = "Язык ввода " + InputLanguageManager.Current.CurrentInputLanguage.NativeName.ToString();
                if (Keyboard.IsKeyToggled(Key.CapsLock))
                {
                    LCaps.Content = "Клавиша CapsLock нажата";
                }
                else LCaps.Content = "";
            }
            if (TbLogin.Text != "" && (TbPassword.Text != "" || PbPassword.Password != ""))
            {
                BtEnter.IsEnabled = true;
            }
            else BtEnter.IsEnabled = false;
        }
    }
}
