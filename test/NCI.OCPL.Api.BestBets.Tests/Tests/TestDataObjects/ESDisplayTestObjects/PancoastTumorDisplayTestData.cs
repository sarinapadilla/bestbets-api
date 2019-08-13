using System;
namespace NCI.OCPL.Api.BestBets.Tests.ESDisplayTestData
{
    public class PancoastTumorDisplayTestData : BaseDisplayTestData
    {
        public override string TestFilePath => "PancoastTumor.json";
        public override BestBetsCategoryDisplay ExpectedData => new BestBetsCategoryDisplay()
        {
            ID = "36012",
            Name = "Pancoast Tumor",
            Weight = 20,
            HTML = "<div class=\"managed list\">\n<ul>\n<li class=\"general-list-item general list-item\">\n<!-- cgvSnListItemGeneral -->\n<!-- Image -->\n<!-- End Image -->\n<div class=\"title-and-desc title desc container\"><a class=\"title\" href=\"/types/lung/patient/non-small-cell-lung-treatment-pdq\">Non-Small Cell Lung Cancer Treatment (PDQ®)-Patient Version</a><!-- start description -->\n<div class=\"description\"><p class=\"body\">Information about Pancoast Tumor can be found in this treatment summary under Stage IIIA.</p></div><!-- end description --></div><!-- end title & desc container -->\n</li></ul>\n</div>"
        };
    }
}
