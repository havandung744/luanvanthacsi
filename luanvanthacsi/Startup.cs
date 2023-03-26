using luanvanthacsi.Ultils;

namespace luanvanthacsi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            WordUltil.DocxTemplateFolder = Path.Combine(location, "Templates");

        }
    }
}
