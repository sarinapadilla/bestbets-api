namespace NCI.OCPL.Api.BestBets.Tests.CategoryTestData
{
    public class PancoastTumorCategoryTestData : BaseCategoryTestData  
    {

        public override string TestFilePath => "CGBBCategory.PancoastTumor.xml";        
        protected override CancerGovBestBet CoreData => new CancerGovBestBet() 
        {
            ID = "36012",
            Name = "Pancoast Tumor",
            Weight = 20,
            IsExactMatch = false,
            Language = "en",
            Display = true,
            HTML = @"
<div class=""managed list"">
<ul>
<li class=""general-list-item general list-item"">
<!-- cgvSnListItemGeneral -->
<!-- Image -->
<!-- End Image -->
<div class=""title-and-desc title desc container""><a class=""title"" href=""/dictionary?cdrid=45817"">Definition of Pancoast Tumor</a></div><!-- end title & desc container -->
</li><li class=""general-list-item general list-item"">
<!-- cgvSnListItemGeneral -->
<!-- Image -->
<!-- End Image -->
<div class=""title-and-desc title desc container""><a class=""title"" href=""/cancertopics/pdq/treatment/non-small-cell-lung/healthprofessional/page9"">Stage IIIA Non-Small Cell Lung Cancer Treatment</a><div class=""description""><p class=""body"">See the section of this page about ""superior sulcus tumors"" for information about Pancoast tumors.</p></div><!-- end description --></div><!-- end title & desc container -->
</li></ul>
</div>
"
        };
        protected override string[] IncludeSyn => new string[] { "pancoast" };
    }

}