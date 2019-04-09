using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StocksApplication.Models;

namespace StocksApplication.Controllers
{
    public class ProvideFeedbackController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public ProvideFeedbackController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Feedback feedback)
        {
            SaveFeedback(feedback);
            return RedirectToAction("Feedback");
        }

        public IActionResult Feedback()
        {
            return View();
        }

        public bool SaveFeedback(Feedback feedback)
        {
            bool isFeedbackSaved = false;
            if (feedback != null)
            {
                _appDbContext.Feedback.Add(feedback);
                isFeedbackSaved = true;
            }
            return isFeedbackSaved;
        }
    }
}