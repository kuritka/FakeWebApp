using System.Collections.Generic;

namespace FakerTest.Controllers
{
    public interface IGlobalNotesProcessor
    {
        string GetGlobalNotes(IEnumerable<Entities.GlobalNote> globalNotes);
    }
}
