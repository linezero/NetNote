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
    public class AddModel : PageModel
    {
        private INoteRepository _noteRepository;
        private INoteTypeRepository _noteTypeRepository;
        private IWebHostEnvironment _webHostEnvironment;
        public AddModel(INoteRepository noteRepository,
            INoteTypeRepository noteTypeRepository,
            IWebHostEnvironment webHostEnvironment) 
        {
            _noteRepository = noteRepository;
            _noteTypeRepository = noteTypeRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public NoteModel Note { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }

        public async Task OnGetAsync()
        {
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
            var fileName = string.Empty;
            if (Note.Attachment != null)
            {
                var dir = Path.Combine(_webHostEnvironment.WebRootPath, "file");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                fileName = Path.Combine("file", Guid.NewGuid().ToString() + Path.GetExtension(Note.Attachment.FileName));
                using (var stream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath,fileName), FileMode.CreateNew))
                {
                    Note.Attachment.CopyTo(stream);
                }
            }
            await _noteRepository.AddAsync(new Note
            {
                Title = Note.Title,
                Content = Note.Content,
                TypeId=Note.Type,
                Create = DateTime.Now,
                Password=Note.Password,
                Attachment=fileName
            });
            return RedirectToPage("./Index");
        }
    }
}
