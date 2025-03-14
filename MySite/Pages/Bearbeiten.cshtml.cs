using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySite.Models;

namespace MySite.Pages
{
    public class BearbeitenModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;

        private readonly resumeContext _context;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public bool IsLoggedIn { get; set; }

        public List<SelectListItem> ColorOptions { get; set; }

        [BindProperty]
        public string SelectedStage { get; set; }
        [BindProperty]
        public string ResumeContent { get; set; }
        [BindProperty]
        public string PositionTitle { get; set; }
        [BindProperty]
        public string Company { get; set; }
        [BindProperty]
        public string Place { get; set; }


        public BearbeitenModel(ILogger<IndexModel> logger, resumeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            // Pr�fen, ob schon eine Session existiert
            var userId = HttpContext.Session.GetInt32("UserId");
            IsLoggedIn = userId.HasValue;

            var lifestageOptions = _context.ResumeLifestages.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.LifestageTitle
            }).ToList();

            ColorOptions = lifestageOptions;

            //ColorOptions = new List<SelectListItem>
            //{
            //    new SelectListItem("Rot", "rot"),
            //    new SelectListItem("Blau", "blau"),
            //    new SelectListItem("Gr�n", "gruen")
            //};
        }


        public async Task<IActionResult> OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            // Passwort hashen (oder verschl�sseln) je nach deiner Logik
            // Hier nur Beispiel: string hashed = ComputeHash(Password);
            // Falls du schon gehashte Passw�rter hast, vergleichst du hier den Hash.


            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                string hashed = Password; // Platzhalter: Hier sollte eigentlich dein Hashing hin

                // Datenbank nach passendem Nutzer durchsuchen
                var user = await _context.ResumeLogins.FirstOrDefaultAsync(u => u.Username == Username && u.Pwhash == hashed);

                if (user != null)
                {
                    // Login erfolgreich -> Session setzen
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);

                    // Weiterleitung z. B. auf eine "gesch�tzte" Seite
                    //return RedirectToPage("/Index");

                    IsLoggedIn = true;


                    var lifestageOptions = _context.ResumeLifestages.Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.LifestageTitle
                    }).ToList();

                    ColorOptions = lifestageOptions;
                }
                else
                {
                    // Fehler: Benutzer nicht gefunden oder Passwort falsch
                    ErrorMessage = "Falscher Benutzername oder Passwort!";

                    IsLoggedIn = false;
                }

                return Page();

            }
            else if (!string.IsNullOrEmpty(SelectedStage) && !string.IsNullOrEmpty(ResumeContent))
            {
                // Neues Objekt der Entit�tsklasse anlegen
                var newContent = new ResumeContent
                {
                    IdRl = (short?)int.Parse(SelectedStage),
                    PositionTitle = PositionTitle,
                    FullText = ResumeContent,
                    Company = Company,
                    Place = Place,
                    DateFrom = DateOnly.FromDateTime(DateTime.Now),
                    DateTill = DateOnly.FromDateTime(DateTime.Now)
                };
                
                // Objekt in den Kontext einf�gen
                _context.ResumeContents.Add(newContent);

                // �nderungen in der DB speichern
                await _context.SaveChangesAsync();

                // Weiterleitung (z. B. zur�ck zur Index-Seite oder Danke-Seite)
                return RedirectToPage("/Index");
            }
            //else
            //{
            //    // Fehler: Benutzername oder Passwort fehlt
            //    ErrorMessage = "Bitte Benutzername und Passwort eingeben!";

            //    IsLoggedIn = false;

               return Page();
            //}


        }




    }
}
