using BookVier.DBHelper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Shapes;

namespace BookVier
{
    /// <summary>
    /// ReadForm.xaml 的交互逻辑
    /// </summary>
    public partial class ReadForm : Window
    {
        private List<string> mTextList = new List<string>();

        private static string dbPath = @"D:\DB.db3";

        private static string path = "";

        private int nCrtNum = 0;

        private void titalLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        

        public ReadForm(string file)
        {
            InitializeComponent();
            path = file;
            SolidColorBrush NewColor = new SolidColorBrush();
            // NewColor.Color = Color.FromArgb(255, 230, 231, 232);
            textBox.Background = NewColor;
            SolidColorBrush NewColor2 = new SolidColorBrush();
            // NewColor2.Color = Color.FromArgb(255, 207, 214, 229);
            FileStream fs = new FileStream(file, FileMode.Open);
            Encoding encoding = GetType(fs);
            string str = File.ReadAllText(file, encoding);
            int nNum = str.Length / 3000;
            if (str.Length % 3000 != 0)
            {
                nNum++;
            }
            for (int i = 0; i < nNum; i++)
            {
                if (i == nNum - 1)
                {
                    int nlen = str.Length - i * 3000;
                    mTextList.Add(str.Substring(i * 3000, nlen));
                }
                else
                {
                    mTextList.Add(str.Substring(i * 3000, 3000));
                }
            }
            // 创建数据库文件
            SQLiteHelper.NewDbFile(dbPath);
            SQLiteHelper.NewTable(dbPath, "CREATE TABLE tableIndex (path varchar,num int)", "tableIndex");
            SQLiteHelper sqlhelp = new SQLiteHelper(dbPath);
            Boolean flag = sqlhelp.OpenDb();
            if (!flag)
            {
                MessageBox.Show("读取历史数据失败");
            }
            else
            {
                SQLiteHelper sQLiteHelper = new SQLiteHelper(dbPath);
                string Str = "select num FROM tableIndex where path='" + System.IO.Path.GetFileName(file) + "'";
                SQLiteCommand sQLiteCommand = sQLiteHelper.getCommand(Str);
                int num = Convert.ToInt32(sQLiteCommand.ExecuteScalar());
                nCrtNum = num;
                textBox.Text = mTextList.ElementAt(num);
                return;
            }
            textBox.Text = mTextList.ElementAt(0);
        }



        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        //public System.Text.Encoding GetFileEncodeType(string filename)
        //{
        //    System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
        //    Byte[] buffer = br.ReadBytes(2);
        //    if (buffer[0] >= 0xEF)
        //    {
        //        if (buffer[0] == 0xEF && buffer[1] == 0xBB)
        //        {
        //            return System.Text.Encoding.UTF8;
        //        }
        //        else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
        //        {
        //            return System.Text.Encoding.BigEndianUnicode;
        //        }
        //        else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
        //        {
        //            return System.Text.Encoding.Unicode;
        //        }
        //        else
        //        {
        //            return System.Text.Encoding.Default;
        //        }
        //    }
        //    else
        //    {
        //        return System.Text.Encoding.Default;
        //    }
        //}
        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name=“fs“>文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name=“data“></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        private void CloseWnd_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight && e.ViewportHeight != 0)//到底了
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight && e.VerticalOffset != 0)
            {
                scrollView.ScrollToTop();
                nCrtNum++;
                if (nCrtNum > (mTextList.Count() - 1))
                    return;
                SQLiteHelper sQLiteHelper = new SQLiteHelper(dbPath);
                // 判断数据是否存在
                string Str = "select count(0) FROM tableIndex where path='" + System.IO.Path.GetFileName(path) + "'";
                SQLiteCommand sQLiteCommand = sQLiteHelper.getCommand(Str);
                Object ob = sQLiteCommand.ExecuteScalar();
                if (Convert.ToInt32(ob) == 0)
                {
                    // 不存在 新增记录
                    string sql = "insert into tableIndex(path,num) values('" + System.IO.Path.GetFileName(path) + "','" + nCrtNum + "')";
                    sQLiteHelper.ExecuteNonQuery(sql);
                }
                else
                {
                    // 存在修改
                    string sql = "update tableIndex set num='" + nCrtNum + "' where path='" + System.IO.Path.GetFileName(path) + "'";
                    sQLiteHelper.ExecuteNonQuery(sql);
                }

                textBox.Text = mTextList.ElementAt(nCrtNum);

            }

        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                // Ctrl+P 页码跳转
                if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.KeyboardDevice.IsKeyDown(Key.P))
                {
                    if (pageText.Visibility == Visibility.Visible) {
                        pageText.Visibility = Visibility.Hidden;
                    }
                    else {
                        pageText.Visibility = Visibility.Visible;
                    }

                    if (skipBtn.Visibility == Visibility.Visible)
                    {
                        skipBtn.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        skipBtn.Visibility = Visibility.Visible;
                    }
                }
                ////Ctrl+C 全选
                //if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.KeyboardDevice.IsKeyDown(Key.C))
                //{
                //    if (Keyboard.FocusedElement != null && Keyboard.FocusedElement.GetType().Name == "TextBox") return;
                //    CommandBinding_Copy(null, null);
                //}

                ////Ctrl+X 全选
                //if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.KeyboardDevice.IsKeyDown(Key.X))
                //{
                //    if (Keyboard.FocusedElement != null && Keyboard.FocusedElement.GetType().Name == "TextBox") return;
                //    CommandBinding_Cut(null, null);
                //}
                ////Ctrl+V 全选
                //if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.KeyboardDevice.IsKeyDown(Key.V))
                //{
                //    if (Keyboard.FocusedElement != null && Keyboard.FocusedElement.GetType().Name == "TextBox") return;
                //    CommandBinding_Paste(null, null);
                //}

                ////Ctrl+A 全选
                //if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && e.KeyboardDevice.IsKeyDown(Key.A))
                //{
                //    SelectAllCheck.IsChecked = true;
                //    SelectAll_Click(SelectAllCheck, e);
                //}
                ////Shift+D 删除
                //if ((e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) && e.KeyboardDevice.IsKeyDown(Key.Delete))
                //{
                //    DeleteBtn_Click(null, e);
                //}
            }
            catch (Exception)
            {
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {   
            if (pageText.Text != null && pageText.Text != "") {
                try{
                    if (Convert.ToInt32(pageText.Text) < 0) {
                        MessageBox.Show("请输入正整数,你个傻叉");
                    }
                    nCrtNum = Convert.ToInt32(pageText.Text);
                }catch(Exception ex) {
                    MessageBox.Show("请输入正整数,你个傻叉");
                    return;
                }
                textBox.Text = mTextList.ElementAt(nCrtNum);
            }
        }
    }
}
