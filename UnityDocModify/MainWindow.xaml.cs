using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using WinForm = System.Windows.Forms;

namespace UnityDocModify
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string address;
        bool canBeStart;
        public string strContent;
        public string needContent;
        public List<FileInfo> htmlInfos = new List<FileInfo>();

        int count;
        //进度条
        private BackgroundWorker bgworker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();

        }
        private void IniUI()
        {
            strContent = Str.Text;
            needContent = NeedStr.Text;

        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            //using WinForm = System.Windows.Forms; 需要添加引用: System.Windows.Froms
            WinForm.FolderBrowserDialog folderBrowser = new WinForm.FolderBrowserDialog();

            folderBrowser.ShowDialog();
            Address.Text = folderBrowser.SelectedPath;
            htmlInfos = GetAllHtmlFiles();
            count = htmlInfos.Count;
            ProBar.Maximum = count;
            Info.Content = "文件数：" + count;


        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            IniUI();
            if (Str.Text == null || Str.Text == "")
            {
                MessageBox.Show("被替换字符不能为空");
                canBeStart = false;

            }
            else if (htmlInfos.Count == 0)
            {
                canBeStart = false;
            }
            else
            {
                canBeStart = true;
            }
            if (canBeStart)
            {
                InitWork();
                bgworker.RunWorkerAsync();
            }
        }
        void InitWork()
        {
            bgworker.WorkerReportsProgress = true;
            bgworker.DoWork += new DoWorkEventHandler(DoWork);
            bgworker.ProgressChanged += new ProgressChangedEventHandler(BgworkChange);
            //  bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkComplete);
        }

        private void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("修改完成");
        }

        private void BgworkChange(object sender, ProgressChangedEventArgs e)
        {
            this.ProBar.Value = e.ProgressPercentage;
            Info.Content = e.ProgressPercentage + "/" + count;
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < htmlInfos.Count; i++)
            {
                string t = File.ReadAllText(htmlInfos[i].FullName);
                t = t.Replace(strContent, needContent);
                bgworker.ReportProgress(i + 1);
                File.WriteAllText(htmlInfos[i].FullName, t);
            }

        }
        List<FileInfo> GetAllHtmlFiles()
        {

            List<StreamReader> streams = new List<StreamReader>();
            if (Address.Text != null)
            {
                DirectoryInfo dir = new DirectoryInfo(Address.Text);
                FileInfo[] files = dir.GetFiles();
                List<FileInfo> htmlFiles = new List<FileInfo>();//所有的html文件
                foreach (var item in files)
                {
                    if (item.Extension == ".html" || item.Extension == ".htm")
                    {
                        htmlFiles.Add(item);
                    }
                }

                return htmlFiles;
            }
            else
            {
                return null;
            }



        }

        /// <summary>
        /// 将获取到的文件名保存到bin目录下的Name.txt文件中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Save_Click(object sender, RoutedEventArgs e)
        //{

        //    using (StreamWriter sw = new StreamWriter("Name.txt"))
        //    {

        //    }
        //}
    }
}
