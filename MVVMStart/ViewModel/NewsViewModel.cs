using MVVMStart.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace MVVMStart.ViewModel
{
     class NewsViewModel : Bindable
    {
        public PostArticleViewModel TDFVM { get; set; }
        public DelegateCommand getArticles { get; set; }
        public DelegateCommand getArticleText { get; set; }
        public DelegateCommand postArticle { get; set; }
        public NewsServerModel SelectedNewsServer { get; set; }
        public ArticleModel SelectedArticleHeadline { get; set; }

        public static string newsServerName;

        //Holds all the use groups
        public static ObservableCollection<NewsServerModel> newsServerList = new ObservableCollection<NewsServerModel>();
        public static ObservableCollection<NewsServerModel> NewsServerList
        {
            get { return newsServerList; }
            set { newsServerList = value; }
        }

        //Holds all the headlines
        public static ObservableCollection<ArticleModel> articleList = new ObservableCollection<ArticleModel>();
        public static ObservableCollection<ArticleModel> ArticleList
        {
            get { return articleList; }
            set { articleList = value; }
        }
        public NewsViewModel()
        {
            getArticles = new DelegateCommand(o =>
            {
                openArticle();
            });

            getArticleText = new DelegateCommand(o =>
            {
                openArticleText();
            });

            postArticle = new DelegateCommand(o =>
            {
                PostArticle();
            });
        }

        //Loads all the headlines
        private void openArticle()
        {

              newsServerName = SelectedNewsServer.NewsServerName.Split(' ')[0].Replace(" ", string.Empty);
        
           if(!newsServerName.Equals(string.Empty))
            {
                ConnectionModel.getArticles(newsServerName);
            }
        }

        //Loads the text from the choosed headline
        private void openArticleText()
        {
            string articleName = SelectedArticleHeadline.ArticleHeadline.Split('\t')[0];
            int articleNumber = Int32.Parse(articleName);
        
            ConnectionModel.getArticleText(articleNumber);
          
        }

        //Changes the view
        private void PostArticle()
        {
            MainViewModel current = MainViewModel.current;
            TDFVM = new PostArticleViewModel();
            current.CurrentView = TDFVM;
        }
    }
}

