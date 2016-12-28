using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WPF_Homework_2016.Annotations;

namespace WPF_Homework_2016
{
    class RegexItem:INotifyPropertyChanged
    {
        public RegexItem()
        {
            
        }

        public RegexItem(XElement aXElement)
        {
            if (aXElement == null) return;
            RegexType = aXElement.FirstAttribute.Value;
            RegexText = aXElement.Element("regex")?.Value;
            CreateTime = aXElement.Element("time")?.Value;
            Intro = aXElement.Element("intro")?.Value;
        }

        private string _RegexText;
        private string _CreateTime;
        private string _Intro;

        private string _RegexType;


        public string RegexText
        {
            get
            {
                return _RegexText;
            }

            set
            {
                if (_RegexText == value) return;
                _RegexText = value;
                OnPropertyChanged("RegexText");
            }
        }

        public string CreateTime
        {
            get
            {
                return _CreateTime;
            }

            set
            {
                if (_CreateTime == value||value==null) return;
                _CreateTime = value;
                OnPropertyChanged("CreateTime");
            }
        }

        public string Intro
        {
            get
            {
                return _Intro;
            }

            set
            {
                if (_Intro == value||value==null) return;
                _Intro = value;
                OnPropertyChanged("Intro");
            }
        }

        public string RegexType
        {
            get
            {
                return _RegexType;
            }

            set
            {
                if (_RegexType == value) return;
                _RegexType = value;
                OnPropertyChanged("RegexType");
            }
        }


        public XElement CreateXElement(string aXmlNodeName)
        {
            XElement item=new XElement(aXmlNodeName,new XElement("regex",RegexText),new XElement("time",CreateTime.ToString()),new XElement("intro",Intro));
            item.SetAttributeValue("type",RegexType);
            return item;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return RegexText;
        }
    }
}
