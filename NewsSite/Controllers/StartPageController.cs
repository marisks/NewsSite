using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using NewsSite.Models.Pages;
using NewsSite.Models.Views;

namespace NewsSite.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
        private readonly IContentLoader _contentLoader;

        public StartPageController()
        {
            _contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
        }

        public ActionResult Index(StartPage currentPage)
        {
            var view = new StartPageView
            {
                CurrentPage = currentPage,
                Articles = _contentLoader.GetChildren<ArticlePage>(currentPage.ContentLink).ToArray()
            };
            return View(view);
        }
    }
}