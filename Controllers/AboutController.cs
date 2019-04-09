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
    public class AboutController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public CompanyDetails detailsOfCompany;
        public CompanyDetails tempDetails;
        public string inputSymbol;
        bool isSaved = false;
        public string BASE_URL = "https://api.iextrading.com/1.0/";
        HttpClient httpClient;

        public AboutController(AppDbContext appDbContext)
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
            return RedirectToAction("AboutCompanyDetails");
        }

        public IActionResult AboutCompanyDetails()
        {
            inputSymbol = Convert.ToString(TempData["value"]);
            detailsOfCompany = GetCompanyDetails(inputSymbol);
            SaveCompanyDetails(detailsOfCompany);
            return View(detailsOfCompany);
        }

        private CompanyDetails GetCompanyDetails(string symbol)
        {
            CompanyDetails companyDetails = new CompanyDetails();
            string CompanyDetails_End_Point = BASE_URL + "stock/" + symbol + "/company";
            string apiResponse = string.Empty;
            httpClient.BaseAddress = new Uri(CompanyDetails_End_Point);
            HttpResponseMessage response = httpClient.GetAsync(CompanyDetails_End_Point).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!string.IsNullOrEmpty(apiResponse))
            {
                companyDetails = JsonConvert.DeserializeObject<CompanyDetails>(apiResponse);
            }
            return companyDetails;
        }

        public void SaveCompanyDetails(CompanyDetails companyDetails)
        {
            CompanyDetails companyInfo = new CompanyDetails();

            if (_appDbContext.CompanyDetails.Where(x => x.Symbol == companyDetails.Symbol).Count() == 0)
            {
                _appDbContext.CompanyDetails.Add(companyDetails);
            }

            _appDbContext.SaveChanges();
        }


    }
}