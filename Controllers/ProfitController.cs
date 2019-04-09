using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
    public class ProfitController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public List<CompanyDividend> detailsOfCompany;
        public string inputSymbol;
        bool isSaved = false;
        public string BASE_URL = "https://api.iextrading.com/1.0/";
        HttpClient httpClient;

        public ProfitController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new
                System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string symbol)
        {
            TempData["value"] = symbol;
            return RedirectToAction("GetCompanyProfit");
        }

        public IActionResult GetCompanyProfit()
        {
            inputSymbol = Convert.ToString(TempData["value"]);
            detailsOfCompany = GetCompanyDividend(inputSymbol);
            SaveCompanyLatestDividend(detailsOfCompany);
            return View(detailsOfCompany);
        }

        private List<CompanyDividend> GetCompanyDividend(string symbol)
        {
            List<CompanyDividend> cDividends = new List<CompanyDividend>();
            string CompanyDividends_End_Point = BASE_URL + "stock/" + symbol + "/dividends/5y";
            string apiResponse = string.Empty;
            httpClient.BaseAddress = new Uri(CompanyDividends_End_Point);
            HttpResponseMessage response = httpClient.GetAsync(CompanyDividends_End_Point).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!string.IsNullOrEmpty(apiResponse))
            {
                cDividends = JsonConvert.DeserializeObject<List<CompanyDividend>>(apiResponse);
            }
            return cDividends;
        }

        public void SaveCompanyLatestDividend(List<CompanyDividend> companyDividend)
        {
            if (companyDividend != null && companyDividend.Count != 0)
            {
                foreach (CompanyDividend c in companyDividend)
                {
                    _appDbContext.CompanyDividend.Add(c);
                }
                _appDbContext.SaveChanges();
            }
        }
    }
}