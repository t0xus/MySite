using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
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
        [BindProperty]
        public string DateFromStr { get; set; }
        [BindProperty]
        public string DateTillStr { get; set; }

        [BindProperty(SupportsGet = true)]
        public string id_content { get; set; }
        [BindProperty(SupportsGet = true)]
        public string logout { get; set; }

        public BearbeitenModel(ILogger<IndexModel> logger, resumeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            // Prüfen, ob schon eine Session existiert
            var userId = HttpContext.Session.GetInt32("UserId");
            IsLoggedIn = userId.HasValue;

            var lifestageOptions = _context.ResumeLifestages.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.LifestageTitle
            }).ToList();

            ColorOptions = lifestageOptions;

            
            if (!string.IsNullOrEmpty(id_content))
            {
                // Objekt aus der DB laden
                var content = _context.ResumeContents.Find(int.Parse(id_content));

                // Werte in die BindProperties schreiben
                string? v = content.IdRl.ToString();
                SelectedStage = v;
                ResumeContent = content.FullText;
                PositionTitle = content.PositionTitle;
                Company = content.Company;
                Place = content.Place;
                DateFromStr = content.DateFrom.Value.ToString("dd.MM.yyyy");
                DateTillStr = content.DateTill.Value.ToString("dd.MM.yyyy");
            }
            else if (!string.IsNullOrEmpty(logout))
            {
                if (logout == "1")
                {
                    // Session löschen
                    HttpContext.Session.Remove("UserId");
                    HttpContext.Session.Remove("Username");

                    IsLoggedIn = false;
                }
            }

        }


        public async Task<IActionResult> OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            // Passwort hashen (oder verschlüsseln) je nach deiner Logik
            // Hier nur Beispiel: string hashed = ComputeHash(Password);
            // Falls du schon gehashte Passwörter hast, vergleichst du hier den Hash.


            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                string hashed = ComputeSha256Hash(Password); // Platzhalter: Hier sollte eigentlich dein Hashing hin

                // Datenbank nach passendem Nutzer durchsuchen
                var user = await _context.ResumeLogins.FirstOrDefaultAsync(u => u.Username == Username && u.Pwhash == hashed);

                if (user != null)
                {
                    // Login erfolgreich -> Session setzen
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);

                    // Weiterleitung z. B. auf eine "geschützte" Seite
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
            else if (!string.IsNullOrEmpty(SelectedStage) && !string.IsNullOrEmpty(ResumeContent) && !string.IsNullOrEmpty(id_content))
            {
                var content = _context.ResumeContents.Find(int.Parse(id_content));

                content.IdRl = (short?)int.Parse(SelectedStage);
                content.PositionTitle = PositionTitle;
                content.FullText = ResumeContent;
                content.Company = Company;
                content.Place = Place;
                content.DateFrom = DateOnly.ParseExact(DateFromStr, "dd.MM.yyyy");
                content.DateTill = DateOnly.ParseExact(DateTillStr, "dd.MM.yyyy");


                await _context.SaveChangesAsync();

                return RedirectToPage("/Index");
            }
            else if (!string.IsNullOrEmpty(SelectedStage) && !string.IsNullOrEmpty(ResumeContent))
            {
                // Neues Objekt der Entitätsklasse anlegen
                var newContent = new ResumeContent
                {
                    IdRl = (short?)int.Parse(SelectedStage),
                    PositionTitle = PositionTitle,
                    FullText = ResumeContent,
                    Company = Company,
                    Place = Place,
                    DateFrom = DateOnly.ParseExact(DateFromStr, "dd.MM.yyyy"),
                    DateTill = DateOnly.ParseExact(DateTillStr, "dd.MM.yyyy")
                };
                
                // Objekt in den Kontext einfügen
                _context.ResumeContents.Add(newContent);

                // Änderungen in der DB speichern
                await _context.SaveChangesAsync();

                // Weiterleitung (z. B. zurück zur Index-Seite oder Danke-Seite)
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

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - gibt ein Byte-Array zurück
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Byte-Array in einen hexadezimalen String umwandeln
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }




    }
}
