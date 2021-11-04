using MVVMStart.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace MVVMStart.ViewModel
{
    public class NewsViewModel : Bindable
    {
        public DelegateCommand getArticles { get; set; }
        public string SelectedNewsServer { get; set; }

        public static ObservableCollection<NewsServerModel> newsServerList = new ObservableCollection<NewsServerModel>();
        public static ObservableCollection<NewsServerModel> NewsServerList
        {
            get { return newsServerList; }
            set { newsServerList = value;  }
        }
        public NewsViewModel()
        {
            getArticles = new DelegateCommand(o =>
            {
                openArticle();
            });
        }

        private void openArticle()
        {
            MessageBox.Show(SelectedNewsServer);
            
    }
    }
}

