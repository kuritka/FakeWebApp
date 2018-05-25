using FakerTest.Entities;
using FormatWith;
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

namespace FakerTest.Controllers
{
    public class GlobalNotesAsHtmlProcessor : IGlobalNotesProcessor
    {
        public string GetGlobalNotes(IEnumerable<GlobalNote> globalNotes)
        {
            string result;
            CultureInfo en = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = en;

            try
            {
                var body = new StringBuilder();

                var layout = File.ReadAllText(Path.Combine("Templates", "GlobalNoteLayout.html"));

                var globalTemplateDE = File.ReadAllText(Path.Combine("Templates", "GlobalNoteDE_BisZu.html"));
                var globalTemplateEN = File.ReadAllText(Path.Combine("Templates", "GlobalNoteEN_BisZu.html"));

                foreach (var globalNote in globalNotes)
                {
                    var template = globalNote.Language == "EN" ? globalTemplateEN : globalTemplateDE;
                    body.Append(
                        template.FormatWith(globalNote)                            
                    );
                }

                var notes = body.ToString();

                result = layout.Replace("{body}", notes);

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Something has occured: {ex.Message}", ex);
            }
            return result;
        }
    }
}
