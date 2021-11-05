using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MVVMStart.ViewModel
{
    class PostArticleViewModel
    {
        public string newsServerChosen { get; set; }
        public string userName { get; set; }
        public string articleSubject { get; set; }
        public string articleMessage { get; set; }
        public NewsViewModel TDFVM { get; set; }
        public DelegateCommand backToNewsView { get; set; }
        public DelegateCommand postArticleCommand { get; set; }

        public PostArticleViewModel()
        {
            newsServerChosen = NewsViewModel.newsServerName;
            userName = ConnectViewModel.userName;

            backToNewsView = new DelegateCommand(o =>
            {
                goBackToNewsView();
            });

            postArticleCommand = new DelegateCommand(o =>
            {
                postArticle();
            });

        }

        //Changes the view
        public void goBackToNewsView()
        {
            MainViewModel current = MainViewModel.current;
            TDFVM = new NewsViewModel();
            current.CurrentView = TDFVM;
        }


        //Will call the method and give the parameters, which will post an article
        public void postArticle()
        {
            ConnectionModel.postArticle(userName, newsServerChosen, articleSubject, articleMessage);
            //edit newsServerChosen to dk.test if you want to test
        }

    }

}
