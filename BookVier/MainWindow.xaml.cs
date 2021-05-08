
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookVier
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private string file;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // //选择文件
            // OpenFileDialog dialog = new OpenFileDialog();
            // dialog.Multiselect = true;//该值确定是否可以选择多个文件
            // dialog.Title = "请选择文件夹";
            // dialog.Filter = "*.txt|*.txt";
            // if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            // {
            //     file = dialog.FileName;
            //     RedeForm rede = new RedeForm(file);
            //     rede.ShowDialog();
            // 
            // 
            // }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "*.txt|*.txt";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
                ReadForm rede = new ReadForm(file);
                rede.ShowDialog();
            }
        }
    }
}
