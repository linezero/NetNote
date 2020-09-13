using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetNote.Models;
using NetNote.Repository;

namespace NetNote.Pages
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private INoteRepository _noteRepository;
        public DetailModel(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [BindProperty(SupportsGet =true)]
        public int Id { get; set; }
        public Note Note { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public async Task OnGetAsync()
        {
            var note = await _noteRepository.GetByIdAsync(Id);
            if (string.IsNullOrEmpty(note.Password))
                Note = note;
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var note = await _noteRepository.GetByIdAsync(Id);
            if (!note.Password.Equals(Password))
                return BadRequest("密码错误,返回重新输入");
            Note = note;
            return Page();
        }
    }
}
