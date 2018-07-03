using System.Collections.Generic;

namespace ExecutiveOffice.EDT.GlobalNotesService.Controllers
{
    public interface IGlobalNotesProcessor
    {
        string GetGlobalNotes(IEnumerable<Entities.GlobalNote> globalNotes);
    }
}
