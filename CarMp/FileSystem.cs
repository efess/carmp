using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CarMp
{
    /// <summary>
    /// Methods for accessing filesystem resources
    /// </summary>
    public static class FileSystem
    {
        public static string TopDirectory(string pFullPath)
        {
            return pFullPath.Substring(pFullPath.LastIndexOf('\\'));
        }

        public static List<DriveInfo> GetDrives()
        {
            String[] dr = Environment.GetLogicalDrives();
            List<DriveInfo> driveInfos = new List<DriveInfo>();
            foreach (string drive in dr)
            {
                DriveInfo dInfo = new DriveInfo(drive);
                if(dInfo.IsReady)
                    driveInfos.Add(dInfo);
            }

            return driveInfos;
        }

        /// <summary>
        /// Returns a list of all subdirectories
        /// </summary>
        /// <param name="pPath"></param>
        /// <returns></returns>
        public static List<string> GetDirectories(String pPath)
        {
            List<string> directoryList = new List<string>();

            directoryList.AddRange(Directory.GetDirectories(pPath));

            return directoryList;
        }

        /// <summary>
        /// Scans directory structure for all subdirectories
        /// </summary>
        /// <param name="pPath">Directory to search</param>
        /// <param name="pSupportedExtensions">List of extensions that the directory must contain files for</param>
        /// <returns>List of sub-directory paths</returns>
        public static List<string> GetAllDirectories(String pPath)
        {
            return GetAllDirectories(pPath, null);
        }

        /// <summary>
        /// Scans directory structure for all subdirectories
        /// </summary>
        /// <param name="pPath">Directory to search</param>
        /// <param name="pSupportedExtensions">List of extensions that the directory must contain files for</param>
        /// <returns>List of sub-directory paths</returns>
        public static List<string> GetAllDirectories(String pPath, List<string> pSupportedExtensions)
        {
            // Could use recursive, but we want a local variable
            bool preScanDirectories = pSupportedExtensions != null && pSupportedExtensions.Count > 0;

            List<string> _dirList = new List<string>();

            Stack<String> _dirStack = new Stack<String>();
            _dirStack.Push(pPath);

            while (_dirStack.Count > 0)
            {
                String _dir = _dirStack.Pop();

                if (preScanDirectories)
                {
                    // experimental, check to make sure this directory contains a file to scan
                    foreach (String _file in Directory.GetFiles(_dir))
                    {
                        FileInfo fFile = new FileInfo(_file);

                        string extension = fFile.Extension.Replace(".", "").ToUpper();
                        pSupportedExtensions.Exists(delegate(String str) { return str == extension; });

                        if (pSupportedExtensions.Exists(delegate(String str) { return str == extension; }))
                        {
                            _dirList.Add(_dir);
                            break;
                        }
                    }
                }
                else
                    _dirList.Add(_dir);

                try
                {
                    foreach (String _subDir in Directory.GetDirectories(_dir))
                    {
                        _dirStack.Push(_subDir);
                    }
                }
                catch { } // ignore and continue
            }
            return _dirList;
        }

        public static List<FileInfo> GetFiles(string pDirectory, List<string> pSupportedExtensions)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            foreach (String _file in Directory.GetFiles(pDirectory))
            {
                FileInfo fFile = new FileInfo(_file);

                if (pSupportedExtensions.Count > 0)
                {
                    string extension = fFile.Extension.Replace(".", "").ToUpper();
                    pSupportedExtensions.Exists(delegate(String str) { return str == extension; });

                    if (pSupportedExtensions.Exists(delegate(String str) { return str == extension; }))
                    {
                        fileList.Add(fFile);
                    }
                }
                else
                {
                    fileList.Add(fFile);
                }
            }
            return fileList;
        }

        public static void AppendFiles(string pDirectory, List<string> pSupportedExtensions, List<FileInfo> pFiles)
        {
            foreach (String _file in Directory.GetFiles(pDirectory))
            {
                FileInfo fFile = new FileInfo(_file);

                if (pSupportedExtensions.Count > 0)
                {
                    string extension = fFile.Extension.Replace(".", "").ToUpper();
                    pSupportedExtensions.Exists(delegate(String str) { return str == extension; });

                    if (pSupportedExtensions.Exists(delegate(String str) { return str == extension; }))
                    {
                        pFiles.Add(fFile);
                    }
                }
                else
                {
                    pFiles.Add(fFile);
                }
            }
        }
    }
}
