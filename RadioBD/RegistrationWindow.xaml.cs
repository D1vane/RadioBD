using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Data.OleDb;

namespace RadioBD
{
    /// 
    /// Окно регистрации
    /// 
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            ImgLockPass.Visibility = Visibility.Hidden;
            TbPassword.Visibility = Visibility.Hidden;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
            TbPassword.Clear();
            TbPassword.Visibility = Visibility.Hidden;
            PbPassword.Visibility = Visibility.Visible;
        }

        private void BtRegister_Click(object sender, RoutedEventArgs e)
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
            bool checkUser = check();
            if (checkUser == false)
            {
                OleDbCommand command = null;
                string askystring = $"INSERT INTO register(Логин,Пароль,Роль) VALUES ('{loginUser}','{passwordUser}',FALSE)";
                object result,obj = null;
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

                    MessageBox.Show("SomethingWrong in registration");
                }

                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Аккаунт успешно создан!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Произошла ошибка при создании аккаунта!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                result = t.GetMethod("Disconnect").Invoke(obj, null);
                TbLogin.Clear();
                TbPassword.Clear();
                PbPassword.Clear();
            }
        }
        // Проверка на существование пользователя
        private Boolean check()
        {
            var loginUser = TbLogin.Text;
            var passwordUser = "";
            if (PbPassword.Password.ToString() == "")
            {
                passwordUser = MD5_hash.HashPassword(TbPassword.Text);
            }
            else
            {
                passwordUser = MD5_hash.HashPassword(PbPassword.Password.ToString());
            }

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataTable table = new DataTable();

            string askystring = $"SELECT IDПользователя, Логин, Пароль, Роль FROM register where Логин = '{loginUser}' and Пароль = '{passwordUser}'";

            OleDbCommand command = null;
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

                MessageBox.Show("SomethingWrong in registration");
            }

            adapter.SelectCommand = command;
            adapter.Fill(table);
            result = t.GetMethod("Disconnect").Invoke(obj, null);
            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Пользователь уже существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            else
            {
                askystring = $"SELECT IDПользователя, Логин, Пароль, Роль FROM register where Логин = '{loginUser}'";
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

                    MessageBox.Show("SomethingWrong in registration");
                }
                adapter.SelectCommand = command;
                adapter.Fill(table);
                result = t.GetMethod("Disconnect").Invoke(obj, null);
                if (table.Rows.Count > 0)
                {
                    MessageBox.Show("Логин уже занят, введите другой", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
                else return false;
            };
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
                
                BtRegister.IsEnabled = true;
            }
            else BtRegister.IsEnabled = false;
        }
    }
}
