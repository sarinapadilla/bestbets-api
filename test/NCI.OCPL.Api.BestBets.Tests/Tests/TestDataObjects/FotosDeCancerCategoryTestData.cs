namespace NCI.OCPL.Api.BestBets.Tests.CategoryTestData
{
    public class FotosDeCancerCategoryTestData : BaseCategoryTestData  
    {
        public override string TestFilePath => "CGBBCategory.FotosDeCancer.xml";
        protected override CancerGovBestBet CoreData => new CancerGovBestBet() {
            ID = "431121",
            Name = "Fotos de cáncer",
            Weight = 30,
            IsExactMatch = true,
            Language = "es-us",
            Display = true,
            HTML = @"
<div class=""managed list"">
<ul>
<li class=""general-list-item general list-item"">
<!-- cgvSnListItemGeneral -->
<!-- Image -->
<!-- End Image -->
<div class=""title-and-desc title desc container""><a class=""title"" href=""http://visualsonline.cancer.gov/"">Visuals Online</a><!-- start description -->
<div class=""description""><p class=""body"">Base de datos del NCI con fotografías de médicos y científicos dedicados a la investigación del cáncer e imágenes de tratamientos de pacientes con cáncer. También se encuentran imágenes biomédicas y de ciencias, y fotos de los directores y el personal del NCI.</p></div><!-- end description --></div><!-- end title & desc container -->
</li></ul>
</div>
"
        };
        
        protected override string[] IncludeSyn => new string[] {
            "fotos", "imagenes", "imágenes", "imajenes",
            "fotografias"
        };

        protected override string[] IncludeSynExact => new string[] { 
            "fotos de cancer",
            "imagenes de cancer", "imágenes de cáncer", "imagenes de cáncer", 
            "imágenes de cancer"
        };

        protected override string[] ExcludeSyn => new string[] {
            "piel"
        };
        
    }

}