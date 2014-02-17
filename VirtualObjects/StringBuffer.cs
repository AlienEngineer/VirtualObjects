using System;
using System.Text;

namespace VirtualObjects
{
    // TODO: Verify that using this is better than " " + " "
    /// <summary>
    /// 
    /// </summary>
    public class StringBuffer
    {
        /// <summary>
        /// The StringBuilder
        /// </summary>
        protected readonly StringBuilder Sb = new StringBuilder();

        private StringBuffer(string str)
        {
            Sb.Append(str);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class.
        /// </summary>
        public StringBuffer() {}

        /// <summary>
        /// Casts a String as a StringBuffer.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator StringBuffer(String str)
        {
            return new StringBuffer(str);
        }

        /// <summary>
        /// Casts a StringBuilder as a StringBuffer.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator StringBuffer(StringBuilder str)
        {
            return (str == null) ? null : str.ToString();
        }

        /// <summary>
        /// Casts a StringBuffer as a String.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator String(StringBuffer str)
        {
            return (str == null) ? null : str.ToString();
        }

        /// <summary>
        /// Casts a StringBuffer as a StringBuilder.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator StringBuilder(StringBuffer str)
        {
            return str.Sb;
        }

        /// <summary>
        /// Appends sb2 into sb1.
        /// </summary>
        /// <param name="sb1">The SB1.</param>
        /// <param name="sb2">The SB2.</param>
        /// <returns></returns>
        public static StringBuffer operator +(StringBuffer sb1, String sb2)
        {
            if (sb1 == null)
            {
                sb1 = new StringBuffer();
            }

            sb1.Sb.Append(sb2);
            return sb1;
        }

        /// <summary>
        /// Replaces the specified old string.
        /// </summary>
        /// <param name="oldStr">The old string.</param>
        /// <param name="newStr">The new string.</param>
        /// <returns></returns>
        public String Replace(String oldStr, String newStr)
        {
            return Sb.ToString().Replace(oldStr, newStr);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Sb.ToString();
        }

        /// <summary>
        /// Removes the last nChars calced by fieldSeparator length.
        /// </summary>
        /// <param name="fieldSeparator">The field separator.</param>
        public void RemoveLast(string fieldSeparator)
        {
            // _sb.Remove(_sb.Length - fieldSeparator.Length, fieldSeparator.Length);
            RemoveLast(fieldSeparator.Length);
        }

        /// <summary>
        /// Removes the last nChars.
        /// </summary>
        /// <param name="nChars">How many chars.</param>
        public void RemoveLast(int nChars)
        {
            Sb.Remove(Sb.Length - nChars, nChars);
        }
    }

    /// <summary>
    /// String buffer that acts like a stub. Doesn't append nothing.
    /// </summary>
    public class StubBuffer : StringBuffer
    {
        /// <summary>
        /// Casts a String as a StubBuffer.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator StubBuffer(String str)
        {
            return new StubBuffer();
        }

        /// <summary>
        /// Casts a StringBuilder as a StubBuffer.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator StubBuffer(StringBuilder str)
        {
            return (str == null) ? null : str.ToString();
        }

        /// <summary>
        /// Casts a StubBuffer as a String.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator String(StubBuffer str)
        {
            return (str == null) ? null : str.ToString();
        }

        /// <summary>
        /// Casts a StubBuffer as a StringBuilder.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static implicit operator StringBuilder(StubBuffer str)
        {
            return str.Sb;
        }

        /// <summary>
        /// Simulates an Append of sb2 into sb1.
        /// </summary>
        /// <param name="sb1">The SB1.</param>
        /// <param name="sb2">The SB2.</param>
        /// <returns></returns>
        public static StringBuffer operator +(StubBuffer sb1, String sb2)
        {
            return sb1;
        }
    }
}
