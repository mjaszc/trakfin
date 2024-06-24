using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace Trakfin.Controllers
{
    public class ExpensesController : Controller
    {
        // 
        // GET: /Expenses/
        public IActionResult Index()
        {
            return View();
        }
        // TEST ROUTE
        // GET: /Expenses/Welcome/ 
        public string Welcome(string name, int numTimes = 1)
        {
            return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}");
        }
    }
}
