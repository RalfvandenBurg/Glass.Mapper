/*
   Copyright 2012 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/ 
//-CRE-

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Mapper
{
    public static class ExtensionMethods
    {
        #region String

        /// <summary>
        ///     Replaces the format item in a specified string with the string representation
        ///     of a corresponding object in a specified array.
        /// </summary>
        /// <param name="target">A composite format string (see Remarks).</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string
        ///     representation of the corresponding objects in args.</returns>
        public static string Formatted(this string target, params object[] args)
        {
            return string.Format(target, args);
        }

        /// <summary>
        /// Indicates whether the specified string is null or an System.String.Empty
        ///     string.
        /// </summary>
        /// <param name="target">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string target)
        {
            return string.IsNullOrEmpty(target);
        }

        /// <summary>
        /// Indicates whether the specified string is not null and isn't System.String.Empty
        ///     string.
        /// </summary>
        /// <param name="target">The string to test.</param>
        /// <returns>flase if the value parameter is null or an empty string (""); otherwise, true.</returns>
        public static bool IsNotNullOrEmpty(this string target)
        {
            return !IsNullOrEmpty(target);
        }

        #endregion

        #region IEnumerable<T>
        
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action )
        {
            foreach (var item in list)
            {
                action(item);
            }
            return list;
        }
    public static IEnumerable<T> MakeEnumerable<T>(this T obj)
        {
            return new T[] {obj};
        }

        #endregion


      

        #region Misc

        public static T CastTo<T>(this object target)
        {
            return (T)target;
        }

        #endregion
    }
}



