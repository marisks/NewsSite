using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using NewsSite.Models.Media;

namespace NewsSite.Models.Pages
{
    [ContentType(GUID = "31C5C92D-707B-4B69-A52E-11256286FE64", DisplayName = "Article page")]
    public class ArticlePage : PageData
    {
        [Display(Name = "Introduction", Order = 1000)]
        public virtual string Intro { get; set; }

        [Display(Name = "Content", Order = 1010)]
        public virtual XhtmlString Content { get; set; }

        [Display(Name = "Image", Order = 1020)]
        [AllowedTypes(typeof(ImageFile))]
        public virtual ContentArea Image { get; set; }
    }
}