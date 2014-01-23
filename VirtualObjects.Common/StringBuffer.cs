using System;
using System.Text;

namespace VirtualObjects
{
    // TODO: Verify that using this is better than " " + " "
    public class StringBuffer
    {
        readonly StringBuilder _sb = new StringBuilder();

        private StringBuffer(string str)
        {
            _sb.Append(str);
        }

        public StringBuffer() {}

        public static implicit operator StringBuffer(String str)
        {
            return new StringBuffer(str);
        }

        public static implicit operator StringBuffer(StringBuilder str)
        {
            return (str == null) ? null : str.ToString();
        }

        public static implicit operator String(StringBuffer str)
        {
            return (str == null) ? null : str.ToString();
        }

        public static implicit operator StringBuilder(StringBuffer str)
        {
            return str._sb;
        }

        public static StringBuffer operator +(StringBuffer sb1, String sb2)
        {
            if (sb1 == null)
            {
                sb1 = new StringBuffer();
            }

            sb1._sb.Append(sb2);
            return sb1;
        }

        public String Replace(String oldStr, String newStr)
        {
            return _sb.ToString().Replace(oldStr, newStr);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        public void RemoveLast(string fieldSeparator)
        {
            _sb.Remove(_sb.Length - fieldSeparator.Length, fieldSeparator.Length);
        }
    }
}
