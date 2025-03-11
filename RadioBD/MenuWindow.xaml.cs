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
using System.Data.OleDb;

namespace RadioBD
{
    /// 
    /// Меню базы данных
    /// 
    public partial class MenuWindow : Window
    {
        // Порядок вкладок меню
        private int order;
        // Проверка, какой сейчас пользователь зашел
        private readonly checkRights _user;
        public MenuWindow(checkRights user)
        {
            _user = user;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
        }
        // Прикрепление события ко вкладке
        private void ShowDllWindow_Click(object sender, RoutedEventArgs e)
        {
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataTable dataTable = new DataTable();
            OleDbCommand command = null;
            // Текущая вкладка
            MenuItem menuItem = e.Source as MenuItem;
            string askystring = $"SELECT ИмяDLL, ИмяФункции FROM Menu WHERE ИмяПункта = '{menuItem.Header.ToString()}'";

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
                MessageBox.Show($"Something wrong in ");
            }
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);
            if (dataTable.Rows[0].ItemArray[0].ToString() != "" && dataTable.Rows[0].ItemArray[1].ToString() != "")
            {
                try
                {
                    // Открытие загруженной вкладки
                    asm = Assembly.Load(dataTable.Rows[0].ItemArray[0].ToString());
                    t = asm.GetType(dataTable.Rows[0].ItemArray[0].ToString() + "." + dataTable.Rows[0].ItemArray[1].ToString());
                    if (menuItem.Header.ToString() != "Клиент" && menuItem.Header.ToString() != "Справочники" && menuItem.Header.ToString() != "Справка" && menuItem.Header.ToString() != "Содержание"
                        && menuItem.Header.ToString() != "О программе" && menuItem.Header.ToString() != "Разное" && menuItem.Header.ToString() != "Настройки" && menuItem.Header.ToString() != "Настройки")
                    {
                        obj = t.GetConstructor(new Type[1] { typeof(string) }).Invoke(new object[1] { _user.Login });
                    }
                    else 
                    {
                        obj = t.GetConstructor(new Type[0]).Invoke(new object[0]);
                    }
                    result = t.GetMethod("ShowDialog").Invoke(obj, null);
                }
                catch (Exception)
                {
                    MessageBox.Show("Something wrong in AdminLibrary");
                }
            }
           
        }
        // Чтение из базы данных строки
        private void ReadRowFromDB(Menu menu, IDataRecord record)
        {
            int parent = record.GetInt32(1);
            MenuItem menuItem = new MenuItem();
            menuItem.Header = record.GetString(2);
            bool isAdmin = false;
            if (menuItem.Header.ToString() == "Настройки")
            {
                if (LabelLogin.Content.ToString().Contains("Администратор"))
                {
                    menuItem.Click += ShowDllWindow_Click;
                    isAdmin = true;
                }
            }
            else
            {
                if (!record.IsDBNull(3) && !record.IsDBNull(4)) menuItem.Click += ShowDllWindow_Click;
            }
            if (parent == 0)
            {
                menu.Items.Add(menuItem);
                order = record.GetInt32(5);
            }
            
            else
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                DataTable dataTable = new DataTable();
                string askystring = $"SELECT IDРодительскогоПункта FROM Menu WHERE IDПунктаМеню = {record.GetInt32(0)}";
                OleDbCommand command = null;

                object result = null, obj = null;
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
                    MessageBox.Show("Something wrong in Menu");
                }
                adapter.SelectCommand = command;
                adapter.Fill(dataTable);
                askystring = $"SELECT Порядок FROM Menu WHERE IDПунктаМеню = {Convert.ToInt32(dataTable.Rows[0].ItemArray[0])}";
                try
                {

                    command = new OleDbCommand(askystring, (OleDbConnection)result);
                }
                catch (Exception)
                {
                    MessageBox.Show("Something wrong in Menu");
                }
                result = t.GetMethod("Disconnect").Invoke(obj, null);
                dataTable = new DataTable();
                adapter.SelectCommand = command;
                adapter.Fill(dataTable);
                MenuItem kid = (MenuItem)menu.Items[Convert.ToInt32(dataTable.Rows[0].ItemArray[0])];
                if (menuItem.Header.ToString() == "Настройки")
                {
                    if (isAdmin) kid.Items.Add(menuItem);
                    isAdmin = false;
                }
                else kid.Items.Add(menuItem);
            }
        }
        // Заполнение меню из базы данных
        private void FillMenu(Menu menu)
        {
            
            string askystring = $"SELECT * FROM Menu";
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
                MessageBox.Show("Something wrong in Menu");
            }

            OleDbDataReader reader = command.ExecuteReader();
            // Чтение из базы данных
            while (reader.Read())
            {
                ReadRowFromDB(menu, reader);
            }
            reader.Close();
            result = t.GetMethod("Disconnect").Invoke(obj, null);
        }

        private void CheckUser()
        {
            LabelLogin.Content = $"{_user.Login} : {_user.Rights()}";
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckUser();
            FillMenu(MenuCategories);
        }
    }
}
