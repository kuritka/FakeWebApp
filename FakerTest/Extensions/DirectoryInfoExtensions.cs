using System;
using System.IO;
using System.Linq;

namespace FakerTest.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo Combine(this DirectoryInfo directoryInfo, Uri uri)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            string combinedPath = Path.Combine(directoryInfo.FullName, uri.LocalPath.Trim(Path.DirectorySeparatorChar));
            return new DirectoryInfo(combinedPath);
        }

        public static bool IsEmpty(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));
            return !Directory.EnumerateFileSystemEntries(directoryInfo.FullName).Any();
        }

        /// <summary>
        /// Rename the directory in directoryInfo to a new name.
        /// </summary>
        /// <param name="directoryInfo">Source directory informations.</param>
        /// <param name="newName">New directory name.</param>
        public static void Rename(this DirectoryInfo directoryInfo, string newName)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));
            if (newName == null) throw new ArgumentNullException(nameof(newName));

            directoryInfo.MoveTo(Path.Combine(directoryInfo.Parent.FullName, newName));
        }


        public static void DeleteContent(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }


        public static DirectoryInfo DeleteWithContentIfExists(this DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName))
            {
                target.DeleteContent();
                Directory.Delete(target.FullName);
            }
            return target;
        }


        public static DirectoryInfo CreateNumberedIfExists(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                return directoryInfo;
            }
            string name = directoryInfo.Name;

            var directories = directoryInfo.Parent.GetDirectories($"{name}*");

            return directoryInfo.Parent.CreateSubdirectory($"{name}_{directories.Length}");
        }
    
        public static DirectoryInfo CreateIfNotExists(this DirectoryInfo target)
        {
            if (!Directory.Exists(target.FullName))
            {
                target.Create();
            }
            return target;
        }

        public static void CopyContentTo(this DirectoryInfo directory, DirectoryInfo target)
        {
            if (!Directory.Exists(directory.FullName)) throw new DirectoryNotFoundException(nameof(directory));
            if (!Directory.Exists(target.FullName))
            {
                target.Create();
            }
            foreach (DirectoryInfo dir in directory.GetDirectories())
                CopyContentTo(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in directory.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}
