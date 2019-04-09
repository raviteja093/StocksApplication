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
    public class SharesController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public CompanyQuote detailsOfCompany;
        public string inputSymbol;
        bool isSaved = false;
        public string BASE_URL = "https://api.iextrading.com/1.0/";
        HttpClient httpClient;

        public SharesController(AppDbContext appDbContext)
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
            return RedirectToAction("AboutCompanyShares");
        }
        public IActionResult AboutCompanyShares()
        {
            inputSymbol = Convert.ToString(TempData["value"]);
            detailsOfCompany = GetCompanyQuote(inputSymbol);
            SaveCompanyQuote(detailsOfCompany);
            return View(detailsOfCompany);
        }


        private CompanyQuote GetCompanyQuote(string symbol)
        {
            CompanyQuote cQ = new CompanyQuote();
            string CompanyQuote_End_Point = BASE_URL + "stock/" + symbol + "/quote";
            string apiResponse = string.Empty;
            httpClient.BaseAddress = new Uri(CompanyQuote_End_Point);
            HttpResponseMessage response = httpClient.GetAsync(CompanyQuote_End_Point).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!string.IsNullOrEmpty(apiResponse))
            {
                cQ = JsonConvert.DeserializeObject<CompanyQuote>(apiResponse);
            }
            return cQ;
        }

        public void SaveCompanyQuote(CompanyQuote companyQuote)
        {
            _appDbContext.CompaniesQuote.Add(companyQuote);
            _appDbContext.SaveChanges();
        }
    }
}