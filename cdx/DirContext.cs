using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dirx
{
    public class DirContext
    {
        public DirContext(string activeDir)
        {
            ActiveDir = activeDir;
            SearchTerm = string.Empty;
            AltTargetFolders = Directory.GetDirectories(ActiveDir)
                .Select(e => Path.GetFileName(e))
                .Where(e => string.IsNullOrEmpty(SearchTerm) ? true : e.StartsWith(SearchTerm))
                .ToList();
            TargetFolder = AltTargetFolders.FirstOrDefault();
        }

        public string ActiveDir { get; set; }

        public string TargetFolder { get; set; }

        public List<string> AltTargetFolders { get; set; }

        public string SearchTerm { get; set; }

        public void MoveBack()
        {
            var dirName = Path.GetFileName(ActiveDir);
            var parent = Directory.GetParent(ActiveDir);
            if (parent == null)
            {
                return;
            }

            ActiveDir = parent.FullName;
            TargetFolder = dirName;
            AltTargetFolders = Directory.GetDirectories(ActiveDir)
                .Select(e => Path.GetFileName(e))
                .Where(e => string.IsNullOrEmpty(SearchTerm) || e.StartsWith(SearchTerm))
                .ToList();
            SearchTerm = string.Empty;
        }

        public void MoveForward()
        {
            SearchTerm = string.Empty;
            ActiveDir = Path.Combine(ActiveDir, TargetFolder);
            AltTargetFolders = Directory.GetDirectories(ActiveDir)
                .Select(e => Path.GetFileName(e))
                .Where(e => string.IsNullOrEmpty(SearchTerm) || e.StartsWith(SearchTerm))
                .ToList();
            TargetFolder = AltTargetFolders.FirstOrDefault() ?? string.Empty;
        }

        public void SelectPrev()
        {
            var index = AltTargetFolders.IndexOf(TargetFolder);
            TargetFolder = AltTargetFolders.ElementAt(Math.Clamp(index - 1, 0, AltTargetFolders.Count - 1));
        }

        public void SelectNext()
        {
            var index = AltTargetFolders.IndexOf(TargetFolder);
            TargetFolder = AltTargetFolders.ElementAt(Math.Min(AltTargetFolders.Count - 1, index + 1));
        }

        public void SetSearchTerm(string newSearchTerm)
        {
            SearchTerm = newSearchTerm;
            AltTargetFolders = Directory.GetDirectories(ActiveDir)
                .Select(e => Path.GetFileName(e))
                .Where(e => string.IsNullOrEmpty(SearchTerm) || e.StartsWith(SearchTerm))
                .ToList();
            TargetFolder = AltTargetFolders.FirstOrDefault() ?? string.Empty;
        }
    }
}
