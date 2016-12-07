using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.BestBets.Tests.CategoryTestData
{
    public class BreastCancerSynonymTestData : BaseSynonymTestData
    {
        // These object contents are copied from the BreastCancerCategoryTestData class
        public override string TestFilePath => "CGBBCategory.BreastCancer.xml";
        protected override CancerGovBestBet CoreData => new CancerGovBestBet()
        {
            ID = "36408",
            Name = "Breast Cancer",
            Weight = 19,
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
<div class=""title-and-desc title desc container""><a class=""title"" href=""/types/breast"">Breast Cancer—Patient Version</a><div class=""description""><p class=""body"">NCI's gateway for information about breast cancer.</p></div><!-- end description --></div><!-- end title & desc container -->
</li><li class=""general-list-item general list-item"">
<!-- cgvSnListItemGeneral -->
<!-- Image -->
<!-- End Image -->
<div class=""title-and-desc title desc container""><a class=""title"" href=""/research/progress/snapshots/breast"">A Snapshot of Breast Cancer</a><div class=""description""><p class=""body"">Information on trends in incidence, mortality, and NCI funding for breast cancer; examples of related NCI initiatives and selected research advances.</p></div><!-- end description --></div><!-- end title & desc container -->
</li></ul>
</div>
"
        };

        protected override string[] IncludeSyn => new string[] {
            "brest", "breastcancer", "bresat",
            "breast", "brast", "ductal carcinoma",
            "breat", "braest", "beast",
            "breats", "ductual carcinoma", "ductile carcinoma",
            "lobular carcinoma"
        };

        protected override string[] IncludeSynExact => new string[] {
            "mammary", "mammary carcinoma",
        };

        protected override string[] ExcludeSyn => new string[] {
            "wyntk", "nodule", "symtoms", "fibrocystic", "insitu",
            "cyst", "DVD", "symptoms", "bands", "common", "risk",
            "lavage", "sister", "ultrasound", "size", "survey",
            "milk", "surgery", "colloid", "picture", "tool", "exam",
            "wristbands", "testing", "braclets", "calculator", "implant",
            "test", "inflammitory", "symptons", "Consortium", "disparities",
            "pregnancy", "memory", "atypical", "bracelet", "reduction",
            "Long Island", "Avastin", "inflamitory", "braclet", "hyperplasia",
            "endoscopy", "angiosarcoma", "augmentation", "month", "mri", "Group",
            "feeding", "mucinous", "calcifications", "changes", "schwanoma",
            "papillary", "mass", "tenderness", "history", "nutrition", "cells",
            "calcium", "Moving Beyond", "photo", "syptoms", "cancer center",
            "brachytherapy", "examination", "slides", "inflammatory", "bilateral",
            "inflamatory", "thermography", "alcohol", "itchy", "symtems",
            "triplenegative", "DCIS", "rash", "screening", "imflamatory", "elderly",
            "tubular", "reconstruction", "self-examination", "tea", "itching", "symtons",
            "survivors", "Paget", "HRT", "hormone therapy", "soy", "HER2", "obesity",
            "density", "osteosarcoma", "marker", "myths", "vaccine", "occult", "imflammatory",
            "infiltrante", "Living Each Day", "swelling", "detection", "metaplastic", "dense",
            "How do you get", "cost", "pain", "young women", "stereotactic", "diet", "biopsy",
            "discharge", "basal", "multifocal", "quiz", "schwannoma", "adenoid cystic",
            "advanced", "prosthesis", "IMRT", "massage", "walk", "microcalcifications",
            "asymmetry", "reconstructive", "stamp", "video", "anatomy", "exercise",
            "surveillance", "ribbon", "signs", "African", "lump", "genetics of breast ovarian",
            "lupron", "vitamin", "lymphedema", "support", "specialists", "pathophysiology",
            "premenopausal", "abortion", "assessment", "self-exam", "smoking", "fibroids",
            "medullary", "infection", "probability", "pathology", "benign", "in situ",
            "epidemiology", "partial", "adjuvant", "questionnaire", "excisional", "lymphoma",
            "contralateral", "multicentric", "papilloma", "gene", "sarcoma", "melanoma",
            "triple-negative", "triple negative", "fna", "imaging"
        };
    }
}
