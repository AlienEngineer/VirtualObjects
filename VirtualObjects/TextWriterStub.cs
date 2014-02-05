using System;
using System.IO;
using System.Text;

namespace VirtualObjects.Core
{
    class TextWriterStub : TextWriter
    {
        public override Encoding Encoding
        {
            get { return null; }
        }
    }
}
