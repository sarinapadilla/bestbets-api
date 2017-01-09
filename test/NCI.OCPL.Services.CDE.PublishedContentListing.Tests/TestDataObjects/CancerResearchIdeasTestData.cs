namespace NCI.OCPL.Api.BestBets.Tests.CategoryTestData
{
    public class CancerResearchIdeas : BaseCategoryTestData  
    {

        public override string TestFilePath => "CGBBCategory.CancerResearchIdeas.xml";        
        protected override CancerGovBestBet CoreData => new CancerGovBestBet() 
        {
            ID = "1045389",
            Name = "Cancer Research Ideas",
            Weight = 100,
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
<div class=""title-and-desc title desc container""><a class=""title"" href=""https://cancerresearchideas.cancer.gov"">Cancer Research Ideas</a><!-- start description -->
<div class=""description""><p class=""body"">An online platform for cancer researchers to submit their best scientific ideas for bringing about a decade's worth of advances in 5 years, making more therapies available to more patients, and spurring progress in cancer prevention, treatment, and care. </p></div><!-- end description --></div><!-- end title & desc container -->
</li></ul>
</div>
"
        };
        protected override string[] IncludeSyn => new string[] { "Clinical Trial Ideas" };
    }

}