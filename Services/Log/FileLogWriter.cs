using System.IO;
using System.Text;

namespace Services.Log
{
    /// <summary>
    /// Escribe el log del sistema en un archivo de texto
    /// </summary>
    public class FileLogWriter : ILogWriter
    {
        public virtual void Write(string path, string contents, Encoding encoding)
        {
            File.AppendAllText(path,contents,encoding);
        }
    }
}