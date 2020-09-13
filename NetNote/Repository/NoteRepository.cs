using Microsoft.EntityFrameworkCore;
using NetNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetNote.Repository
{
    public class NoteRepository : INoteRepository
    {
        private NoteContext context;
        public NoteRepository(NoteContext _context)
        {
            context = _context;
        }
        public Task AddAsync(Note note)
        {
            context.Notes.Add(note);
            return context.SaveChangesAsync();
        }

        public Task<Note> GetByIdAsync(int id)
        {
            return context.Notes.Include(type => type.Type).FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<List<Note>> ListAsync()
        {
            return context.Notes.Include(type => type.Type).ToListAsync();
        }

        public Task UpdateAsync(Note note)
        {
            context.Entry(note).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }

        public Task RemoveAsync(Note note)
        {
            context.Entry(note).State = EntityState.Deleted;
            return context.SaveChangesAsync();
        }

        public async Task<Tuple<List<Note>, int>> PageListAsync(int pageIndex, int pageSize)
        {
            var query = context.Notes
                .Include(type => type.Type).AsQueryable();
            var count = await query.CountAsync();
            var pagecount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            var notes = await query.OrderByDescending(r => r.Create)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new Tuple<List<Note>, int>(notes, pagecount);
        }
    }
}
