using System;
namespace GingerTG
{
    public class EditPageArguments
    {
        public string ShortName { get; set; }
        public string AccessToken { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUrl { get; set; }
        public bool ReturnContent { get; set; }
    }
}
