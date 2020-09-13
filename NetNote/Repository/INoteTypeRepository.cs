using NetNote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetNote.Repository
{
    public interface INoteTypeRepository
    {
        Task<List<NoteType>> ListAsync();
    }
}
