using System;
using System.IO;
using System.Text;
using StoneAge.FileStore.Domain;

namespace StoneAge.FileStore
{
    public class DocumentBuilder : IDocumentBuilder
    {
        private string _filePath;
        private string _name;
        private byte[] _bytes;
        
        public IDocumentBuilderData With_Name(string name)
        {
            _name = name;
            return this;
        }

        public IDocumentBuilderData Using_Name_From_Path(string path)
        {
            // Normalize all path separators to the current OS style
            var normalized = path
                                    .Replace('\\', Path.DirectorySeparatorChar)
                                    .Replace('/', Path.DirectorySeparatorChar);
            _name = Path.GetFileName(normalized);
            return this;
        }

        public IDocumentBuilderGeneration With_File(string file)
        {
            _filePath = file;
            return this;
        }
        
        public IDocumentBuilderGeneration With_Bytes(byte[] bytes)
        {
            _bytes = bytes;
            return this;
        }

        public IDocumentBuilderGeneration With_String(string data)
        {
            _bytes = Encoding.UTF8.GetBytes(data);
            return this;
        }

        public IDocument Create_Document()
        {
            var data = Get_Data();
            return new Document
            {
                Name = _name,
                Data = data
            };
        }

        private byte[] Get_Data()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                return File.ReadAllBytes(_filePath);
            }

            return _bytes;
        }
    }
}
