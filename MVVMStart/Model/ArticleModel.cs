using System;
using System.Collections.Generic;
using System.Text;

namespace MVVMStart.Model
{
    public class ArticleModel : Bindable
    {
        private string articleHeadline;

        public string ArticleHeadline
        {
            get { return articleHeadline; }
            set { articleHeadline = value; propertyIsChanged(); }
        }

    }
}
