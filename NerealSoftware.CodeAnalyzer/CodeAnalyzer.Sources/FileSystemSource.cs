using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeAnalyzer.Interface;

namespace CodeAnalyzer.Sources
{
    public class FileSystemSource : ISource
    {
        private readonly string _path;

        public FileSystemSource(string path)
        {
            _path = path;
        }

        public IEnumerable<IFileSource> GetFiles()
        {
            return Directory.EnumerateFiles(_path, "*.cs", SearchOption.AllDirectories)
                .Select(f => new FileSystemFileSource(f));
        }
    }

    internal class FileSystemFileSource : IFileSource
    {
        private readonly string _path;

        public FileSystemFileSource(string path)
        {
            _path = path;
        }

        public long GetContentLength()
        {
            return File.ReadAllText(_path).Length;
        }

        public string GetFileName()
        {
            return Path.GetFileName(_path);
        }

        public DateTime GetCreateDate()
        {
            return File.GetCreationTime(_path);
        }

        public string GetData()
        {
            return File.ReadAllText(_path);
        }
    }
}
