using Microsoft.AspNetCore.Mvc;

namespace BidMarket.Controllers
{
    [Route("Error/{statusCode}")]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Извините, запрошенный ресурс не найден";
                    break;
                case 400:
                    ViewBag.ErrorMessage = "Извините, скорее всего запрос некорректен";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Извините, мы не можем обработать этот запрос. Но уже в процессе";
                    break;
                default:
                    ViewBag.ErrorMessage = "Извините, неизвестная ошибка";
                    break;
            }
            return View("Error");
        }
    }
}
