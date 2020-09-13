using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetNote.Models;
using NetNote.Repository;
using NetNote.ViewModels;

namespace NetNote.Pages
{
    [Authorize]
    public class EditModel : PageModel
    {
        private INoteRepository _noteRepository;
        private INoteTypeRepository _noteTypeRepository;
        public EditModel(INoteRepository noteRepository,
            INoteTypeRepository noteTypeRepository)
        {
            _noteRepository = noteRepository;
            _noteTypeRepository = noteTypeRepository;
        }

        [BindProperty]
        public NoteModel Note { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }

        public async Task OnGetAsync(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            Note = new NoteModel { 
                Id=note.Id,
                Title=note.Title,
                Content = note.Content,
                Type = note.TypeId,
                Password=note.Password
            };
            var types = await _noteTypeRepository.ListAsync();
            Types = types.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var note = await _noteRepository.GetByIdAsync(Note.Id);
            note.Title = Note.Title;
            note.Content = Note.Content;
            note.TypeId = Note.Type;
            note.Password = Note.Password;
            await _noteRepository.UpdateAsync(note);
            return RedirectToPage("./Index");
        }
    }
}
