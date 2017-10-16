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

namespace iAMPUpdate
{
    /// <summary>
    /// PasswordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public PasswordWindow(string password)
        {
            InitializeComponent();
            _password = password;
        }

        public bool Result { get; private set; }

        private string _password;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(passwordbox.Password==_password)
            {
                Result = true;
                this.Close();
                e.Handled = true;
            }
            else
            {
                MessageBox.Show("Password is incorrect");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(passwordbox);
        }

        /*******************************static method*******************************/
        public static bool OpenPassword(string password,Window owner=null)
        {
            bool Result = false;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PasswordWindow TempWin = new PasswordWindow(password);
                TempWin.Owner = owner ?? ControlHelper.GetTopWindow();
                TempWin.ShowDialog();
                Result = TempWin.Result;
            }));
            return Result;
        }


    }
}
