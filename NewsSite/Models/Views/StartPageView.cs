using NewsSite.Models.Pages;

namespace NewsSite.Models.Views
{
    public class StartPageView
    {
        public StartPage CurrentPage { get; set; }
        public ArticlePage[] Articles { get; set; }
    }
}