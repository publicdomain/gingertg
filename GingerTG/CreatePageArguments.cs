using System;
namespace GingerTG
{
    /// <summary>
    /// Create page arguments.
    /// </summary>
    public class CreatePageArguments
    {
        public string ShortName { get; set; }
        public string AccessToken { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUrl { get; set; }
        public string Content { get; set; }
        public bool ReturnContent { get; set; }
    }
}
