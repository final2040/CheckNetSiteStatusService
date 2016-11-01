using System.Text;

namespace Services
{
    public interface ILogWriter
    {
        void Write(string path, string contents, Encoding encoding);
    }
}