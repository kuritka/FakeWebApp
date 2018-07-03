using ExecutiveOffice.EDT.GlobalNotesService.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ExecutiveOffice.EDT.GlobalNotesService.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> AsOneItemList<T>(this T item)
        {
            return item == null ? new List<T>() : new List<T> { item };
        }


        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }


        public static IEnumerable<GlobalNoteAzureEntity> AsGlobalNoteAzureEntities(this IEnumerable<GlobalNote> globalNotes)
        {
            return globalNotes.Select(d => new GlobalNoteAzureEntity(d));
        }

    }

}
