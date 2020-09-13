using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NetNote.Models;
using NetNote.Repository;
using NetNote.ViewModels;

namespace NetNote.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private INoteRepository _noteRepository;
        public IndexModel(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public IList<Note> Notes { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }
        public int PageCount { get; set; }

        public async Task OnGetAsync()
        {
            if (PageIndex == 0)
                PageIndex = 1;
            var pageSize = 10;
            var notePages =await _noteRepository.PageListAsync(PageIndex, pageSize);
            Notes = notePages.Item1;
            PageCount = notePages.Item2;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note != null)
                await _noteRepository.RemoveAsync(note);
            return RedirectToPage();
        }
    }
}
