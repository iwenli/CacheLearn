using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CacheLearn
{
    public static class Extension
    {
        /// <summary>
        /// string.Format的扩展
        /// </summary>
        /// <param name="formatstr"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormatWith(this string formatstr, params object[] str)
        {
            return string.Format(formatstr, str);
        }
    }
}