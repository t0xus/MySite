using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MySite.Models;

namespace MySite.Pages
{
    public class IndexModel : PageModel
    {
        public string my_message { get; set; }

        private readonly ILogger<IndexModel> _logger;

        private readonly resumeContext _context;

        public List<ResumeLifestage> oRL;
        
        public List<ResumeContent> oRC = new List<ResumeContent>();

        

        public IndexModel(ILogger<IndexModel> logger, resumeContext context)
        {
            _logger = logger;
            _context = context;
        }

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
            
        //}

        public void OnGet()
        {

            oRL = _context.ResumeLifestages
                .OrderBy(p => p.Order)
                .ToList();

            

            for (int i = 0; i < oRL.Count; i++)
            {
                my_message += oRL[i].LifestageTitle + "<br />" + "\n";

                //oRC = _context.ResumeContents.Where(p=> p.IdRl == oRL[i].Id).ToList();

                oRC.AddRange(_context.ResumeContents.Where(p => p.IdRl == oRL[i].Id).ToList());

                foreach (var item in oRC)
                {
                    my_message += item.PositionTitle + "<br />" + "\n";
                }

                my_message += "<br />" + "\n";
            }


            for (int i = 0; i < 10; i++)
            {
                my_message += i + ") <br />";
            }


        }
    }
}
