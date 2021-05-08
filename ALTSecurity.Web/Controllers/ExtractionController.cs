using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.Mvc;
using ALTSecurity.Web.Models;
using ALTSecurity.Web.Utility;
using ALTSecurity.Web.Models.Extraction;
using ALTSecurity.Web.Models.Validation;
using SaltwaterTaffy;
using SaltwaterTaffy.Utility;
using SaltwaterTaffy.Container;

namespace ALTSecurity.Web.Controllers
{
    [Authorize]
    public class ExtractionController : Controller
    {
        // GET: Extraction
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult IsExists(string domainName)
        {
            return new JsonResponse(1, string.Empty, DbQueries.IsExists(domainName));
        }

        [HttpPost]
        public JsonResult ExtractMetadata(string domainName)
        {
            MetadataExtraction metadata = new MetadataExtraction();
            EmailValidation emailValidation = new EmailValidation();
            List<DorkFile> dorks = new List<DorkFile>();
            string dorkUrl = string.Format("site:{0} ext:pdf", domainName);

            var docs = NetworkHelper.ParseGoogle(dorkUrl, 1, 2);
            foreach (var item in docs)
            {
                DorkFile dork = GoogleHacking.LoadFile(item, out HttpStatusCode code);
                if (code != HttpStatusCode.OK)
                {
                    continue;
                }

                Dictionary<string, string> values = metadata.GetFileMetadata(dork.FileName, dork.FileData);

                if (values.ContainsKey("Author") || values.ContainsKey("Producer"))
                {
                    bool actual = false;

                    DateTime creationDate = DateTime.MinValue;
                    if (values.ContainsKey("CreationDate"))
                    {
                        if (DateTime.TryParse(values["CreationDate"], CultureInfo.InvariantCulture, DateTimeStyles.None, out creationDate))
                        {
                            if (creationDate > DateTime.Now.AddMonths(-3))
                            {
                                actual = true;
                            }
                        };
                    }

                    Metadata property = null;
                    if (values.ContainsKey("Author"))
                    {
                        property = new Metadata();

                        property.Value = values["Author"];
                        property.Type = MetadataType.author;
                        property.IsActual = actual;

                        if (emailValidation.EmailExists(property.Value + "@" + domainName))
                        {
                            property.IsAuth = true;
                        }
                        else
                        {
                            property.IsAuth = false;
                        }

                        dork.Metadata.Add(property);
                    }

                    if (values.ContainsKey("Producer"))
                    {
                        property = new Metadata();

                        property.Value = values["Producer"];
                        property.Type = MetadataType.service;
                        property.IsAuth = true;
                        property.IsActual = actual;

                        List<CveItem> vulnInfo = CveApiHelper.GetVulnData(new CveApiQuery
                        {
                            SearchType = ApiSearchType.keyword,
                            SoftName = property.Value

                        });

                        if (vulnInfo.Count > 0)
                        {
                            string reference = $"<a href=\"{ vulnInfo[0].cve.references.reference_data[0].url }\"> { vulnInfo[0].cve.metadata.id } </a>";
                            property.Notice = reference;
                        }

                        dork.Metadata.Add(property);
                    }
                }

                dorks.Add(dork);
            }

            int companyId = DbQueries.SaveDorkFiles(domainName, dorks);

            return new JsonResponse(1, string.Empty, companyId);
        }


        [HttpPost]
        public JsonResult CheckSubdomains(int companyId, string domainName)
        {
            DnsEnumeration dns = new DnsEnumeration();
            List<ServiceResult> resources = new List<ServiceResult>();

            var subdomains = dns.RecieveIps(domainName);

            Parallel.ForEach(subdomains, subdomain =>
            {
                ServiceResult service = new ServiceResult();

                if (subdomain.Value == DomainState.notSecured)
                {
                    Target target = new Target(subdomain.Key);
                    Scanner scanner = new Scanner(target);
                    scanner.PersistentOptions = new NmapOptions
                    {
                        {NmapFlag.PortSpecification, "21,22,25,80,110,143,443,445 "}
                    };

                    ScanResult scan = scanner.PortScan();

                    service.Services = scan.Hosts.First().Ports.ToList();
                }

                service.DomainName = subdomain.Key;
                service.State = subdomain.Value;

                resources.Add(service);

            });

            List<CveItem> vulnInfo = null;
            foreach (var resource in resources.Where(x=> (x.Services != null && x.Services.Count > 0)))
            {
                resource.Cve = new List<string>();
                foreach(var service in resource.Services)
                {
                    if (service.Service.Product != null)
                    {
                        vulnInfo = CveApiHelper.GetVulnData(new CveApiQuery
                        {
                            SearchType = ApiSearchType.keyword,
                            SoftName = service.Service.Product + " " + (service.Service.Version == null? string.Empty: service.Service.Version) 

                        });

                        if (vulnInfo.Count > 0)
                        {
                            resource.Cve.Add(vulnInfo[0].cve.metadata.id);
                        }
                        else
                        {
                            resource.Cve.Add(string.Empty);
                        }
                    }
                    else
                    {
                        resource.Cve.Add(string.Empty);
                    }
                }
            }

            DbQueries.SaveServiveInfo(companyId, resources);

            return new JsonResponse(1, string.Empty);
        }


        [HttpPost]
        public ActionResult GetRiskData(int companyId, string domainName)
        {
            var riskModel = DbQueries.FillRisks(companyId);

            foreach (var item in riskModel.TriggeredRules)
            {
                riskModel.TotalProbability += item.ActionProbability * ((double)item.SourceCount / (double)riskModel.SourceCount);
            }

            riskModel.TotalProbability = Math.Round(riskModel.TotalProbability, 2);

            foreach (var item in riskModel.TriggeredRules)
            {
                item.Probability = Math.Round((item.ActionProbability * ((double)item.SourceCount / (double)riskModel.SourceCount)) / riskModel.TotalProbability, 2);
            }

            riskModel.TriggeredRules = riskModel.TriggeredRules.OrderByDescending(x => x.Probability).ToList();
            ViewBag.DomainName = domainName;

            return PartialView("_Info", riskModel);
        }


        [HttpPost]
        public ActionResult GetDorksInfo(int companyId)
        {
            return PartialView("_Metadata", DbQueries.GetDorksList(companyId));
        }

        [HttpPost]
        public ActionResult Subdomains(int companyId)
        {
            return PartialView("_IpList", DbQueries.GetOpenSubdomains(companyId));
        }

        [HttpPost]
        public ActionResult GetCve(int companyId)
        {
            return PartialView("_CveList", DbQueries.GetCve(companyId));
        }

    }
}