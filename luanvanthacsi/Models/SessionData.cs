using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Models
{
    public class SessionData
    {
        public User CurrentUser { get; set; }
        public BrowserDimension BrowserDimension { get; set; } = new BrowserDimension { Height = 600, Width = 1200 };
    }

    public class BrowserDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
