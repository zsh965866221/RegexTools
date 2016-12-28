using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using WPF_Homework_2016.Annotations;

namespace WPF_Homework_2016
{
    class MainModel:INotifyPropertyChanged
    {
        public MainModel()
        {
            TextMatchRegexList=new ObservableCollection<RegexItem>();
            TextReplaceRegexList= new ObservableCollection<RegexItem>();
            FileMatchRegexList= new ObservableCollection<RegexItem>();
            FileReplaceRegexList= new ObservableCollection<RegexItem>();
            FileList= new ObservableCollection<FileItem>();

            RegexTypeList=new ObservableCollection<string>(new string[] {"TextMatch","TextReplace","FileMatch","FileReplace"});
            ManageRegexTypeSelectedItem = RegexTypeList.FirstOrDefault();
            LogController.Log($"初始化model完成");
        }

        //--文本--
        public ObservableCollection<RegexItem> TextMatchRegexList { get; }
        private RegexItem _TextMatchRegexSelectedItem;
        private string _TextMatchRegexNew;
        private string TextMatchPattern;

        private string _SampleText;

        public ObservableCollection<RegexItem> TextReplaceRegexList { get; }
        private RegexItem _TextReplaceRegexSelectedItem;
        private string _TextReplaceRegexNew;
        private string TextReplacePattern;

        private MatchCollection _TextMatchResult;
        private string _TextReplaceResult;

        public void TextMatch()
        {
            Regex aRegex=new Regex(TextMatchPattern);
            TextMatchResult = aRegex.Matches(SampleText);
            LogController.Log($"文本匹配完成--{TextMatchPattern}");
        }

        public void TextReplace()
        {
            Regex aRegex=new Regex(TextMatchPattern);
            TextReplaceResult = aRegex.Replace(SampleText,TextReplacePattern);
            LogController.Log($"文本替换完成--{TextReplacePattern}");
        }
        //--文本end--

        //--文件--
        public ObservableCollection<RegexItem> FileMatchRegexList { get; }
        private RegexItem _FileMatchRegexSelectedItem;
        private string _FileMatchRegexNew;
        private string FileMatchPattern;

        public ObservableCollection<FileItem> FileList { get; }

        public ObservableCollection<RegexItem> FileReplaceRegexList { get; }
        private RegexItem _FileReplaceRegexSelectedItem;
        private string _FileReplaceRegexNew;
        private string FileReplacePattern;

        public void ClearFileList()
        {
            LogController.Log($"清除文件类表");
            FileList.Clear();
        }

        public void AddFileItem(string aFullFileName)
        {
            LogController.Log($"添加文件--{aFullFileName}");
            FileList.Add(new FileItem(aFullFileName));
        }

        public void FileMatch()
        {
            try
            {
                Regex aRegex = new Regex(FileMatchPattern);
                foreach (FileItem aFileItem in FileList)
                {
                    LogController.Log($"匹配文件名--{aFileItem.ItemFile} by {FileMatchPattern}");
                    aFileItem.Match(aRegex);
                }
                LogController.Log($"匹配文件名完成");
            }
            catch (Exception ex)
            {
                LogController.Log($"匹配文件错误--{ex.Message}");
                MessageBox.Show(ex.Message);
            }
            
        }

        public void FileReplace()
        {
            Regex aRegex=new Regex(FileMatchPattern);
            foreach (FileItem aFileItem in FileList)
            {
                LogController.Log($"替换文件名--{aFileItem.ItemFile} by {FileReplacePattern}");
                aFileItem.Replace(aRegex,FileReplacePattern);
            }
            LogController.Log($"替换文件名完成");
        }

        public void DoRename()
        {
            foreach (FileItem aFileItem in FileList)
            {
                LogController.Log($"执行重命名--{aFileItem.ItemFile}");
                aFileItem.DoRename();
            }
            LogController.Log($"重命名完成");
        }
        //--文件end


        //--popup--
        private string _PopupRegexPattern;
        private string _PopupRegexType;
        private string _PopupTime;
        private string _PopupIntro;

        public void AddPattern()
        {
            RegexItem aRegexItem = new RegexItem();
            aRegexItem.RegexText = PopupRegexPattern;
            aRegexItem.CreateTime = PopupTime;
            aRegexItem.Intro = PopupIntro;
            if (PopupRegexType == "TextMatch")
            {
                aRegexItem.RegexType = "TextMatch";
                TextMatchRegexList.Add(aRegexItem);
            }
            else if (PopupRegexType == "TextReplace")
            {
                aRegexItem.RegexType = "TextReplace";
                TextReplaceRegexList.Add(aRegexItem);
            }
            else if (PopupRegexType == "FileMatch")
            {
                aRegexItem.RegexType = "FileMatch";
                FileMatchRegexList.Add(aRegexItem);
            }
            else if (PopupRegexType == "FileReplace")
            {
                aRegexItem.RegexType = "FileReplace";
                FileReplaceRegexList.Add(aRegexItem);
            }
            else
            {
                return;
            }
            LogController.Log($"添加表达式--{PopupRegexPattern}");
        }

        public bool PatternExist(string aPattern)
        {
            foreach (RegexItem item in TextMatchRegexList)
            {
                if (aPattern == item.RegexText)
                    return true;
            }
            foreach (RegexItem item in TextReplaceRegexList)
            {
                if (aPattern == item.RegexText)
                    return true;
            }
            foreach (RegexItem item in FileMatchRegexList)
            {
                if (aPattern == item.RegexText)
                    return true;
            }
            foreach (RegexItem item in FileReplaceRegexList)
            {
                if (aPattern == item.RegexText)
                    return true;
            }
            return false;
        }
        //--popup end

        //--管理
        private const string ManageFileName = "Regexes.xml";
        private string _ManageRegexTypeSelectedItem;
        public ObservableCollection<string> RegexTypeList { get; }

        public void LoadManageList()
        {
            if (!File.Exists(ManageFileName))
            {
                File.WriteAllText(ManageFileName,"<Regexes>\n</Regexes>");
            }

            XDocument aXDocument=XDocument.Load(ManageFileName);
            TextMatchRegexList.Clear();
            TextReplaceRegexList.Clear();
            FileMatchRegexList.Clear();
            FileReplaceRegexList.Clear();
            foreach (XElement element in from el in aXDocument.Root.Elements("RegexItem") where (string)el.FirstAttribute=="TextMatch" select el)
            {
                TextMatchRegexList.Add(new RegexItem(element));
            }
            foreach (XElement element in from el in aXDocument.Root.Elements("RegexItem") where (string)el.FirstAttribute == "TextReplace" select el)
            {
                TextReplaceRegexList.Add(new RegexItem(element));
            }
        }

        public void SaveManageList()
        {
            XDocument aXDocument=new XDocument(new XElement("Regexes"));
            aXDocument.Root.Add(from r in TextMatchRegexList select r.CreateXElement("RegexItem"));
            aXDocument.Root.Add(from r in TextReplaceRegexList select r.CreateXElement("RegexItem"));
            aXDocument.Root.Add(from r in FileMatchRegexList select r.CreateXElement("RegexItem"));
            aXDocument.Root.Add(from r in FileReplaceRegexList select r.CreateXElement("RegexItem"));
            aXDocument.Save(ManageFileName);
        }

        public void RegexListChange(ListBox manageList)
        {
            if (ManageRegexTypeSelectedItem == "TextMatch")
            {
                manageList.ItemsSource = TextMatchRegexList;
            }
            else if (ManageRegexTypeSelectedItem == "TextReplace")
            {
                manageList.ItemsSource = TextReplaceRegexList;
            }
            else if (ManageRegexTypeSelectedItem == "FileMatch")
            {
                manageList.ItemsSource = FileMatchRegexList;
            }
            else if (ManageRegexTypeSelectedItem == "FileReplace")
            {
                manageList.ItemsSource = FileReplaceRegexList;
            }
            LogController.Log($"更换选择列表--{ManageRegexTypeSelectedItem}");
        }

        public void DeletePattern(RegexItem regexItem)
        {
            if (regexItem.RegexType == "TextMatch")
                TextMatchRegexList.Remove(regexItem);
            else if (regexItem.RegexType == "TextReplace")
                TextReplaceRegexList.Remove(regexItem);
            else if (regexItem.RegexType == "FileMatch")
                FileMatchRegexList.Remove(regexItem);
            else if (regexItem.RegexType == "FileReplace")
                FileReplaceRegexList.Remove(regexItem);
            LogController.Log($"删除表达式完成--{regexItem.RegexText}");
        }
        //--管理 end

        public RegexItem TextMatchRegexSelectedItem
        {
            get
            {
                return _TextMatchRegexSelectedItem;
            }

            set
            {
                if (_TextMatchRegexSelectedItem == value||value==null) return;
                _TextMatchRegexSelectedItem = value;
                OnPropertyChanged("TextMatchRegexSelectedItem");
                TextMatchPattern = value.RegexText;
            }
        }

        public string TextMatchRegexNew
        {
            get
            {
                return _TextMatchRegexNew;
            }

            set
            {
                if (_TextMatchRegexNew == value) return;
                _TextMatchRegexNew = value;
                OnPropertyChanged("TextMatchRegexNew");
                TextMatchPattern = value;
            }
        }

        public string SampleText
        {
            get
            {
                return _SampleText;
            }

            set
            {
                if (_SampleText == value) return;
                _SampleText = value;
                OnPropertyChanged("SampleText");
            }
        }

        public RegexItem TextReplaceRegexSelectedItem
        {
            get
            {
                return _TextReplaceRegexSelectedItem;
            }

            set
            {
                if (_TextReplaceRegexSelectedItem == value||value==null) return;
                _TextReplaceRegexSelectedItem = value;
                OnPropertyChanged("TextReplaceRegexSelectedItem");
                TextReplacePattern = value.RegexText;
            }
        }

        public string TextReplaceRegexNew
        {
            get
            {
                return _TextReplaceRegexNew;
            }

            set
            {
                if (_TextReplaceRegexNew == value) return;
                _TextReplaceRegexNew = value;
                OnPropertyChanged("TextReplaceRegexNew");
                TextReplacePattern = value;
            }
        }

        public MatchCollection TextMatchResult
        {
            get
            {
                return _TextMatchResult;
            }

            set
            {
                if (_TextMatchResult == value) return;
                _TextMatchResult = value;
                OnPropertyChanged("TextMatchResult");
            }
        }

        public string TextReplaceResult
        {
            get
            {
                return _TextReplaceResult;
            }

            set
            {
                if (_TextReplaceResult == value) return;
                _TextReplaceResult = value;
                OnPropertyChanged("TextReplaceResult");
            }
        }

        public RegexItem FileMatchRegexSelectedItem
        {
            get
            {
                return _FileMatchRegexSelectedItem;
            }

            set
            {
                if (_FileMatchRegexSelectedItem == value||value==null) return;
                _FileMatchRegexSelectedItem = value;
                OnPropertyChanged("FileMatchRegexSelectedItem");
                FileMatchPattern = value.RegexText;
            }
        }

        public string FileMatchRegexNew
        {
            get
            {
                return _FileMatchRegexNew;
            }

            set
            {
                if (_FileMatchRegexNew == value) return;
                _FileMatchRegexNew = value;
                OnPropertyChanged("FileMatchRegexNew");
                FileMatchPattern = value;
            }
        }

        public RegexItem FileReplaceRegexSelectedItem
        {
            get
            {
                return _FileReplaceRegexSelectedItem;
            }

            set
            {
                if (_FileReplaceRegexSelectedItem == value||value==null) return;
                _FileReplaceRegexSelectedItem = value;
                OnPropertyChanged("FileReplaceRegexSelectedItem");
                FileReplacePattern = value.RegexText;
            }
        }

        public string FileReplaceRegexNew
        {
            get
            {
                return _FileReplaceRegexNew;
            }

            set
            {
                if (_FileReplaceRegexNew == value) return;
                _FileReplaceRegexNew = value;
                OnPropertyChanged("FileReplaceRegexNew");
                FileReplacePattern = value;
            }
        }

        public string PopupRegexPattern
        {
            get
            {
                return _PopupRegexPattern;
            }

            set
            {
                if (_PopupRegexPattern == value) return;
                _PopupRegexPattern = value;
                OnPropertyChanged("PopupRegexPattern");
            }
        }

        public string PopupRegexType
        {
            get
            {
                return _PopupRegexType;
            }

            set
            {
                if (_PopupRegexType == value) return;
                _PopupRegexType = value;
                OnPropertyChanged("PopupRegexType");
            }
        }

        public string PopupTime
        {
            get
            {
                return _PopupTime;
            }

            set
            {
                if (_PopupTime == value) return;
                _PopupTime = value;
                OnPropertyChanged("PopupTime");
            }
        }

        public string PopupIntro
        {
            get
            {
                return _PopupIntro;
            }

            set
            {
                if (_PopupIntro == value) return;
                _PopupIntro = value;
                OnPropertyChanged("PopupIntro");
            }
        }

        public string ManageRegexTypeSelectedItem
        {
            get
            {
                return _ManageRegexTypeSelectedItem;
            }

            set
            {
                if (_ManageRegexTypeSelectedItem == value) return;
                _ManageRegexTypeSelectedItem = value;
                OnPropertyChanged("ManageRegexTypeSelectedItem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
