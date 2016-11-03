using System.Collections.Generic;
using System.Linq;

using Xunit;

using NCI.OCPL.Utils.Testing;

using NCI.OCPL.Services.BestBets.Tests.CategoryTestData;

namespace NCI.OCPL.Services.BestBets.Tests
{
    public class CancerGovBestBetCategoryTest
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
                HTML = ""
            };
            protected override string[] IncludeSyn => new string[] { "pancoast" };
        }

        public class BreastCancerCategoryTestData : BaseCategoryTestData  
        {
            public override string TestFilePath => "CGBBCategory.BreastCancer.xml";
            protected override CancerGovBestBet CoreData => new CancerGovBestBet() {
                ID = "36408",
                Name = "Breast Cancer",
                Weight = 19,
                IsExactMatch = false,
                Language = "en",
                Display = true,
                HTML = ""
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

        public static IEnumerable<object[]> XmlDeserializingData => new[] {
            new object[] { new PancoastTumorCategoryTestData() },
            new object[] { new BreastCancerCategoryTestData() }
        };

        [Theory, MemberData("XmlDeserializingData")]
        public void Can_Deserialize_XML(BaseCategoryTestData data) 
        {
            //Setup the expected object.
            CancerGovBestBet actCat = TestingTools.DeserializeXML<CancerGovBestBet>(data.TestFilePath);

            //Compare Object
            Assert.Equal(data.ExpectedData, actCat, new IBestBetCategoryComparer());
        }
    }
}