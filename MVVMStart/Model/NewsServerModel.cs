using System;
using System.Collections.Generic;
using System.Text;

namespace MVVMStart.Model
{
    public class NewsServerModel : Bindable
    {
        private string newsServerName;

        public string NewsServerName
        {
            get { return newsServerName; }
            set { newsServerName = value; propertyIsChanged(); }
        }
       
    }
}
