using System;
using System.Collections.Generic;
using System.Text;

namespace StoneAge.FileSystem.DotNetCore
{
    public class WriteFileResult
    {
        public bool HadError => ErrorMessages.Count > 0;

        public List<string> ErrorMessages { get; set; }
    }
}
