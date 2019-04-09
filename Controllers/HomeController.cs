using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly HttpClient httpClient;
        public string BASE_URL = "https://api.iextrading.com/1.0/";
        public List<Company> companies;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new
                System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult Index()
        {
            List<Company> CompanyList = GetAllCompanies();
            SaveCompanies(CompanyList);
            return View(CompanyList);
        }

        private List<Company> GetAllCompanies()
        {
            string CompaniesApi_End_Point = BASE_URL + "ref-data/symbols";
            string companyList = string.Empty;
            httpClient.BaseAddress = new Uri(CompaniesApi_End_Point);
            HttpResponseMessage response = httpClient.GetAsync(CompaniesApi_End_Point).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                companyList = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            if (!string.IsNullOrEmpty(companyList))
            {
                companies = JsonConvert.DeserializeObject<List<Company>>(companyList);
            }
            return companies;
        }

        public void SaveCompanies(List<Company> companies)
        {
            var newRecords = from cs in companies
                                 join cd in _appDbContext.Companies
                                     on cs.Symbol equals cd.Symbol into pp
                                 from cd in pp.DefaultIfEmpty()
                                 where cd == null
                                 select cs;
            int count = newRecords.ToList().Count;
            if (count != 0)
            {
                foreach (Company c in newRecords)
                {
                    _appDbContext.Companies.Add(c);
                }
                _appDbContext.SaveChanges();
            }
        }

        [HttpPost]
        public IActionResult GetSharePrices(string symbol)
        {
            TempData["value"] = symbol;
            return RedirectToAction("GetSharePrice");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
