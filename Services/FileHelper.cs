﻿using System.IO;
using System.Text;

namespace Services
{
    public class FileHelper
    {
        public virtual void AppendAllText(string path, string contents, Encoding encoding)
        {
            File.AppendAllText(path,contents,encoding);
        }
    }
}