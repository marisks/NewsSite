using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace NewsSite.Models.Media
{
    [ContentType(GUID = "DD5E732F-571B-4DE9-B0F2-68F47819B545", DisplayName = "Image")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
    public class ImageFile : ImageData
    {
    }
}