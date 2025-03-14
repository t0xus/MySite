using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySite.Models;

namespace MySite.Pages
{
    public class KontaktModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Nachricht { get; set; }

        private readonly ILogger<IndexModel> _logger;

        private readonly resumeContext _context;

        // Property für Erfolgsmeldung
        public string SuccessMessage { get; set; }
        public string UnSuccessMessage { get; set; }

        public KontaktModel(ILogger<IndexModel> logger, resumeContext context)
        {
            _logger = logger;
            _context = context;
        }


        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //return RedirectToPage("/Index");

            // Optional: Validierung prüfen
            if (!ModelState.IsValid)
            {
                // Falls etwas fehlt, zeige das Formular erneut

                UnSuccessMessage = "Ihre Nachricht konnte leider nicht gespeichert werden!";

                return Page();
            }

            // Neues Objekt der Entitätsklasse anlegen
            var newContact = new ResumeContact
            {
                Name = Name,
                Email = Email,
                Nachricht = Nachricht,
                Timestamp = DateTime.Now
                // Falls deine Tabelle weitere Felder hat (z. B. Timestamp),
                // kannst du sie hier auch befüllen.
            };

            // Objekt in den Kontext einfügen
            _context.ResumeContacts.Add(newContact);

            // Änderungen in der DB speichern
            _context.SaveChangesAsync();

            SuccessMessage = "Ihre Nachricht wurde erfolgreich gespeichert!";


            // Weiterleitung (z. B. zurück zur Index-Seite oder Danke-Seite)
            //return RedirectToPage("/Index");
            return Page();
        }
    }
}
