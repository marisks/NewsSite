using System.Web.Mvc;
using EPiServer.Web.Mvc;
using NewsSite.Models.Pages;

namespace NewsSite.Controllers
{
    public class ArticlePageController : PageController<ArticlePage>
    {
        public ActionResult Index(ArticlePage currentPage)
        {
            return View(currentPage);
        }
    }
}