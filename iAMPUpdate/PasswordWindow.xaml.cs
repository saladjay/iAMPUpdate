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

        private string _password;

        /*******************************static method*******************************/
        public bool OpenPassword(string password,Window owner=null)
        {
            bool Result = true;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PasswordWindow TempWin = new PasswordWindow(password);
                TempWin.Owner = owner;
                TempWin.ShowDialog();
            }));
            return Result;
        }
    }
}
