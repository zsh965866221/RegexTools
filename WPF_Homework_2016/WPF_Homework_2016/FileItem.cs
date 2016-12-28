using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPF_Homework_2016.Annotations;

namespace WPF_Homework_2016
{
    class FileItem:INotifyPropertyChanged
    {
        public FileItem()
        {
        }

        public FileItem(string fullFileName)
        {
            FullFileName = fullFileName;
            BasePath = Path.GetDirectoryName(fullFileName);
            ItemFile = Path.GetFileName(fullFileName);
        }

        private string _fullFileName;

        private string _BasePath;

        private string _ItemFile;
        private string _RenamedResult;

        private MatchCollection _FileMatchResult;

        private string _RenamedDisplay;

        public string ItemFile
        {
            get
            {
                return _ItemFile;
            }

            set
            {
                if (_ItemFile == value) return;
                _ItemFile = value;
                OnPropertyChanged("ItemFile");
            }
        }

        public string RenamedResult
        {
            get
            {
                return _RenamedResult;
            }

            set
            {
                if (_RenamedResult == value) return;
                _RenamedResult = value;
                OnPropertyChanged("RenamedResult");
            }
        }

        public MatchCollection FileMatchResult
        {
            get
            {
                return _FileMatchResult;
            }

            set
            {
                if (_FileMatchResult == value) return;
                _FileMatchResult = value;
                OnPropertyChanged("FileMatchResult");
            }
        }

        public string FullFileName
        {
            get
            {
                return _fullFileName;
            }

            set
            {
                if (_fullFileName == value) return;
                _fullFileName = value;
            }
        }

        public string BasePath
        {
            get
            {
                return _BasePath;
            }

            set
            {
                if (_BasePath == value) return;
                _BasePath = value;
            }
        }

        public string RenamedDisplay
        {
            get
            {
                return _RenamedDisplay;
            }

            set
            {
                if (_RenamedDisplay == value) return;
                _RenamedDisplay = value;
                OnPropertyChanged("RenamedDisplay");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return ItemFile;
        }

        public void Match(Regex aRegex)
        {
            FileMatchResult = aRegex.Matches(ItemFile);
        }

        public void Replace(Regex aRegex,string aReplacePattern)
        {
            RenamedResult = aRegex.Replace(ItemFile, aReplacePattern);
            RenamedDisplay = ItemFile + "  ->  " + RenamedResult;
        }

        public void DoRename()
        {
            FileInfo file=new FileInfo(FullFileName);
            file.MoveTo(Path.Combine(BasePath,RenamedResult));
        }
    }
}
