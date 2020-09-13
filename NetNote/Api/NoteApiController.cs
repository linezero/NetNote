using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetNote.Models;
using NetNote.Repository;
using NetNote.ViewModels;

namespace NetNote.Api
{
    [Route("api/note")]
    public class NoteApiController : Controller
    {
        private INoteRepository _noteRepository;
        private INoteTypeRepository _noteTypeRepository;
        public NoteApiController(INoteRepository noteRepository, INoteTypeRepository noteTypeRepository)
        {
            _noteRepository = noteRepository;
            _noteTypeRepository = noteTypeRepository;
        }
        // GET: api/note
        [HttpGet]
        public async Task<IActionResult> Get(int pageindex = 1)
        {
            var pagesize = 10;
            var notes = await _noteRepository.PageListAsync(pageindex, pagesize);
            ViewBag.PageCount = notes.Item2;
            ViewBag.PageIndex = pageindex;
            var result = notes.Item1.Select(r => new NoteDTO
            {
                Id = r.Id,
                Title = string.IsNullOrEmpty(r.Password) ? r.Title : "加密内容",
                Content = string.IsNullOrEmpty(r.Password) ? r.Content : "",
                Attachment = string.IsNullOrEmpty(r.Password) ? r.Attachment : "",
                Type = r.Type.Name
            });
            return Ok(result);
        }

        // GET api/note/5
        // GET api/note/5?password=123
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id, string password)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note == null)
                return NotFound();
            if (!string.IsNullOrEmpty(note.Password) && !note.Password.Equals(password))
                return Unauthorized();
            var result = new NoteDTO
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                Attachment = note.Attachment,
                Type = note.Type.Name
            };
            return Ok(result);
        }

        // POST api/note
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]NoteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string filename = string.Empty;
            await _noteRepository.AddAsync(new Note
            {
                Title = model.Title,
                Content = model.Content,
                Create = DateTime.Now,
                TypeId = model.Type,
                Password = model.Password,
                Attachment = filename
            });
            return CreatedAtAction("Index", "");
        }
    }
}
