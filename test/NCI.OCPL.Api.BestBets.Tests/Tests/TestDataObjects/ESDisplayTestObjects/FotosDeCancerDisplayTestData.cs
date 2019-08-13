using System;
namespace NCI.OCPL.Api.BestBets.Tests.ESDisplayTestData
{
    public class FotosDeCancerDisplayTestData : BaseDisplayTestData
    {
        public override string TestFilePath => "FotosDeCancer.json";
        public override BestBetsCategoryDisplay ExpectedData => new BestBetsCategoryDisplay()
        {
            ID = "431121",
            Name = "Fotos de cáncer",
            Weight = 30,
            HTML = "<div class=\"managed list\">\n<ul>\n<li class=\"general-list-item general list-item\">\n<!-- cgvSnListItemGeneral -->\n<!-- Image -->\n<!-- End Image -->\n<div class=\"title-and-desc title desc container\"><a class=\"title\" href=\"http://visualsonline.cancer.gov/\">Visuals Online</a><!-- start description -->\n<div class=\"description\"><p class=\"body\">Base de datos del NCI con fotografías de médicos y científicos dedicados a la investigación del cáncer e imágenes de tratamientos de pacientes con cáncer. También se encuentran imágenes biomédicas y de ciencias, y fotos de los directores y el personal del NCI.</p></div><!-- end description --></div><!-- end title & desc container -->\n</li></ul>\n</div>"
        };
    }
}
