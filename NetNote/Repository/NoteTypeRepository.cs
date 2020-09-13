using Microsoft.EntityFrameworkCore;
using NetNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetNote.Repository
{
    public class NoteTypeRepository : INoteTypeRepository
    {
        private NoteContext context;
        public NoteTypeRepository(NoteContext _context)
        {
            context = _context;
        }

        public Task<List<NoteType>> ListAsync()
        {
            return context.NoteTypes.ToListAsync();
        }
    }
}
