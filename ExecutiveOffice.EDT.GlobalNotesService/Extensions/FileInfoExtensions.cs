using System;
using System.IO;
using System.Linq;
using static System.String;

namespace ExecutiveOffice.EDT.GlobalNotesService.Extensions
{
    public static class FileInfoExtensions
    {
        public static FileInfo ThrowExceptionIfNullOrDoesntExists(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            if (!System.IO.File.Exists(file.FullName)) throw new FileNotFoundException($"{file.Name} doesn't exists");

            return file;
        }

        public static FileInfo ThrowExceptionIfExtensionIsDifferentFrom(this FileInfo file, string extension, bool ignoreCase = true)
        {
            return ThrowExceptionIfExtensionIsDifferentFrom(file, new[] { extension }, ignoreCase);
        }


        public static FileInfo ThrowExceptionIfFileSizeExceedsMB(this FileInfo file, uint megaBytes)
        {
            if (file.Length > megaBytes * 1024 * 1024)
            {
                throw new ArgumentException($"File size {file.Name} exceeds {megaBytes}MB limit");
            }
            return file;
        }

        public static FileInfo ThrowExceptionIfExtensionIsDifferentFrom(this FileInfo file, string[] extensions, bool ignoreCase = true)
        {
            if (extensions.Any(extension => Compare(file.Extension, extension, ignoreCase) == 0))
            {
                return file;
            }
            throw new ArgumentException($"Expected extension : {Join(',', extensions)}");
        }


        public static bool HasSamePath(this FileInfo file1, FileInfo file2)
        {
            return Compare(file1.Directory.FullName, file2.Directory.FullName, StringComparison.OrdinalIgnoreCase) == 0;
        }

    }
}
