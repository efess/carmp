using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CarMP.IO;

namespace CarMP.Background
{
    public class FileCrawler : IEnumerable<FileInfo>
    {
        
        private string[] extensions;
        private string path;
        private List<FileInfo> fileCollection;
        public FileCrawler(string pPath, string[] pSupportedExtensions)
        {
            fileCollection = new List<FileInfo>();
            path = pPath;
            extensions = pSupportedExtensions;
        }

        private void CollectFiles()
        {
            fileCollection = new List<FileInfo>();
            foreach (String _directory in FileSystem.GetAllDirectories(path))
            {
                FileSystem.AppendFiles(_directory, extensions, fileCollection);
            }
        }
        
        public void Load()
        {
            CollectFiles();
        }

        public int Count
        {
            get { return fileCollection.Count; }
        }

        #region IEnumerable<FileInfo> Members
        
        public IEnumerator<FileInfo> GetEnumerator()
        {
            return fileCollection.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return fileCollection.GetEnumerator();
        }

        #endregion
    }
}
