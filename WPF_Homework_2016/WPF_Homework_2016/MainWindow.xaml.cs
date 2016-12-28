using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;

namespace WPF_Homework_2016
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LogController.Log("程序启动");
            InitializeComponent();
            _Model = new MainModel();
            this.DataContext = _Model;

            try
            {
                _Model.LoadManageList();
                LogController.Log("加载正则表达式管理列表");
            }
            catch (Exception ex)
            {
                LogController.Log($"正则表达式加载错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }

        }

        private MainModel _Model;

        private void OnLoadSampleText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log("加载样本文件");

                OpenFileDialog aDlg = new OpenFileDialog();
                aDlg.Title = "选择样本文件";
                aDlg.Filter = "文本文件 (.txt)|*.txt|所有文件 (*.*)|*.*";
                if (aDlg.ShowDialog() != true) return;
                _Model.SampleText = File.ReadAllText(aDlg.FileName, Encoding.Default);
            }
            catch (Exception ex)
            {
                LogController.Log($"加载样本文件错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LogController.Log("保存文件");
                SaveFileDialog aDlg = new SaveFileDialog();
                if (aDlg.ShowDialog() != true) return;
                if (e.Parameter is string)
                {
                    File.WriteAllText(aDlg.FileName, e.Parameter as string);
                    LogController.Log($"写入样本文件--{aDlg.FileName}");
                }
                else if (e.Parameter is MatchCollection)
                {
                    StringBuilder aStringBuilder = new StringBuilder();
                    MatchCollection mc = e.Parameter as MatchCollection;
                    foreach (Match aMatch in mc)
                    {
                        aStringBuilder.Append(aMatch.Value);
                        aStringBuilder.Append("\r\n");
                    }
                    File.WriteAllText(aDlg.FileName, aStringBuilder.ToString());
                    LogController.Log($"写入匹配结果--{aDlg.FileName}");
                }
            }
            catch (Exception ex)
            {
                LogController.Log($"写入文件错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (e.Parameter is string)
            {
                e.CanExecute = !string.IsNullOrWhiteSpace(e.Parameter as string);
            }
            else if (e.Parameter is MatchCollection)
            {
                e.CanExecute = (e.Parameter as MatchCollection).Count > 0;
            }


        }

        private void OnTextMatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log("文本匹配");
                _Model.TextMatch();
            }
            catch (Exception ex)
            {
                LogController.Log($"文本匹配错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnTextReplace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log($"文本替换");
                _Model.TextReplace();
            }
            catch (Exception ex)
            {
                LogController.Log($"文本替换错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnOpenFileList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log($"文件重名名选择多文件");
                OpenFileDialog aDlg = new OpenFileDialog();
                aDlg.Multiselect = true;
                aDlg.Title = "选择文件列表";
                if (aDlg.ShowDialog() != true) return;
                _Model.ClearFileList();
                foreach (string aFullFileName in aDlg.FileNames)
                {
                    _Model.AddFileItem(aFullFileName);
                }
            }
            catch (Exception ex)
            {
                LogController.Log($"选择多文件错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnFileMatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log($"文件匹配");
                _Model.FileMatch();
            }
            catch(Exception ex)
            {
                LogController.Log($"文件匹配错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
            
        }

        private void OnFileReplace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log($"文件替换");
                _Model.FileReplace();
            }
            catch (Exception ex)
            {
                LogController.Log($"文件替换错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnDoRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogController.Log($"文件重命名");
                _Model.DoRename();
            }
            catch (Exception ex)
            {
                LogController.Log($"文件重命名错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
            
        }

        private void OnAddPattern_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LogController.Log($"添加正则表达式-呼出popup");
            AddPatternCommandParameters parameters=e.Parameter as AddPatternCommandParameters;
            _Model.PopupRegexPattern = parameters.RegexText;
            _Model.PopupRegexType = parameters.RegexType;
            _Model.PopupTime = DateTime.Now.ToString();
            _Model.PopupIntro = null;
            ManagePopup.IsOpen = false;
            ManagePopup.IsOpen = true;
        }

        private void OnAddPattern_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            AddPatternCommandParameters parameters = e.Parameter as AddPatternCommandParameters;
            if (parameters != null)
            {
                if (parameters.RegexText != null&&!_Model.PatternExist(parameters.RegexText)&&parameters.RegexText!="")
                    e.CanExecute = true;
            }
        }

        private void OnAddPattern_Click(object sender, RoutedEventArgs e)
        {
            LogController.Log($"点击添加正则表达式按钮");

            _Model.AddPattern();
            ManagePopup.IsOpen = false;
        }

        private void OnRegexTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            LogController.Log($"表达式类型选择改变");
            _Model.RegexListChange(ManageList);
        }

        private void OnWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                LogController.Log($"程序关闭");
                LogController.Log($"保存表达式管理列表");
                _Model.SaveManageList();
            }
            catch (Exception ex)
            {
                LogController.Log($"保存表达式管理列表错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnDeletePattern_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LogController.Log($"删除表达式");
                _Model.DeletePattern(e.Parameter as RegexItem);
            }
            catch (Exception ex)
            {
                LogController.Log($"删除表达式错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
        }

        private void OnDeletePattern_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter as RegexItem != null;
        }
    }
}
