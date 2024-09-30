using Microsoft.AspNetCore.Mvc;
using Calendar.Models;
using Calendar.Services;

namespace Calendar.Controllers
{
    public class CalendarsController : Controller
    {
        private readonly DatabaseService _databaseService;

        public CalendarsController(DatabaseService databaseService) 
        {
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            var events = _databaseService.GetAllEvents();
            return View(events);
        }

        public IActionResult Details(int id)
        {
            var e = _databaseService.GetEvent(id);
            if(e == null)
            {
                return NotFound();
            }
            return View(e);
        }

        public IActionResult Create() 
        { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Calendars calendar)
        {
            if(ModelState.IsValid)
            {
                _databaseService.AddEvent(calendar);
                RedirectToAction(nameof(Index));
            }
            return View(calendar);
        }

        public IActionResult Edit(int id)
        {
            var e = _databaseService.GetEvent(id);
            if (e == null)
            {
                return NotFound();
            }
            return View(e);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Calendars calendar)
        {
            if(id != calendar.CalendarId)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                _databaseService.UpdateEvent(calendar);
                return RedirectToAction(nameof(Index));
            }

            return View(calendar);
        }

        public IActionResult Delete(int id)
        {
            var e = _databaseService.GetEvent(id);
            if (e == null)
            {
                return NotFound();
            }
            return View(e);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _databaseService.DeleteEvent(id);
            return RedirectToAction(nameof(Index));
        }
    }
}