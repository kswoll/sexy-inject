﻿using System.Text;

namespace SexyInject.Emit
{
    public class DefaultFormatProvider : IFormatProvider
    {
        private DefaultFormatProvider()
        {
        }

        public static DefaultFormatProvider Instance = new DefaultFormatProvider();

        public virtual string Int32ToHex(int int32)
        {
            return int32.ToString("X8");
        }

        public virtual string Int16ToHex(int int16)
        {
            return int16.ToString("X4");
        }

        public virtual string Int8ToHex(int int8)
        {
            return int8.ToString("X2");
        }

        public virtual string Argument(int ordinal)
        {
            return $"V_{ordinal}";
        }

        public virtual string Label(int offset)
        {
            return $"IL_{offset:x4}";
        }

        public virtual string MultipleLabels(int[] offsets)
        {
            StringBuilder sb = new StringBuilder();
            int length = offsets.Length;
            for (int i = 0; i < length; i++)
            {
                if (i == 0) sb.AppendFormat("(");
                else sb.AppendFormat(", ");
                sb.Append(Label(offsets[i]));
            }
            sb.AppendFormat(")");
            return sb.ToString();
        }

        public virtual string EscapedString(string str)
        {
            int length = str.Length;
            StringBuilder sb = new StringBuilder(length * 2);
            for (int i = 0; i < length; i++) {
                char ch = str[i];
                if (ch == '\t') sb.Append("\\t");
                else if (ch == '\n') sb.Append("\\n");
                else if (ch == '\r') sb.Append("\\r");
                else if (ch == '\"') sb.Append("\\\"");
                else if (ch == '\\') sb.Append("\\");
                else if (ch < 0x20 || ch >= 0x7f) sb.AppendFormat("\\u{0:x4}", (int)ch);
                else sb.Append(ch);
            }
            return "\"" + sb + "\"";
        }

        public virtual string SigByteArrayToString(byte[] sig)
        {
            StringBuilder sb = new StringBuilder();
            int length = sig.Length;
            for (int i = 0; i < length; i++) {
                if (i == 0) sb.AppendFormat("SIG [");
                else sb.AppendFormat(" ");
                sb.Append(Int8ToHex(sig[i]));
            }
            sb.AppendFormat("]");
            return sb.ToString();
        }
    }
}