using System;
using System.Text;

namespace VirtualObjects
{
    // TODO: Verify that using this is better than " " + " "
    public class StringBuffer
    {
        protected readonly StringBuilder Sb = new StringBuilder();

        private StringBuffer(string str)
        {
            Sb.Append(str);
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
            return str.Sb;
        }

        public static StringBuffer operator +(StringBuffer sb1, String sb2)
        {
            if (sb1 == null)
            {
                sb1 = new StringBuffer();
            }

            sb1.Sb.Append(sb2);
            return sb1;
        }

        public String Replace(String oldStr, String newStr)
        {
            return Sb.ToString().Replace(oldStr, newStr);
        }

        public override string ToString()
        {
            return Sb.ToString();
        }

        public void RemoveLast(string fieldSeparator)
        {
            // _sb.Remove(_sb.Length - fieldSeparator.Length, fieldSeparator.Length);
            RemoveLast(fieldSeparator.Length);
        }

        public void RemoveLast(int nChars)
        {
            Sb.Remove(Sb.Length - nChars, nChars);
        }
    }

    public class StubBuffer : StringBuffer
    {
        public static implicit operator StubBuffer(String str)
        {
            return new StubBuffer();
        }

        public static implicit operator StubBuffer(StringBuilder str)
        {
            return (str == null) ? null : str.ToString();
        }

        public static implicit operator String(StubBuffer str)
        {
            return (str == null) ? null : str.ToString();
        }

        public static implicit operator StringBuilder(StubBuffer str)
        {
            return str.Sb;
        }

        public static StringBuffer operator +(StubBuffer sb1, String sb2)
        {
            return sb1;
        }
    }
}
