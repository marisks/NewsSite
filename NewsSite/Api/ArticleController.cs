using System.Linq;
using System.Web.Http;
using Contracts;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using NewsSite.Models.Pages;

namespace NewsSite.Api
{
    public class ArticleController : ApiController
    {
        private readonly IContentRepository _contentRepository;

        public ArticleController()
        {
            _contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        }

        public void Post(Article article)
        {
            var existing = _contentRepository
                                .GetChildren<ArticlePage>(ContentReference.StartPage)
                                .FirstOrDefault(x => x.Name == article.Name);
            if (existing != null)
            {
                return;
            }

            var newArticlePage = _contentRepository
                                    .GetDefault<ArticlePage>(ContentReference.StartPage);

            newArticlePage.Name = article.Name;
            newArticlePage.Intro = article.Intro;
            newArticlePage.Content = new XhtmlString(article.Content);

            _contentRepository.Save(newArticlePage, SaveAction.Publish, AccessLevel.NoAccess);
        }
    }
}