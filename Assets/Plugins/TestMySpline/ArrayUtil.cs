//
// Author: Ryan Seghers
//
// Copyright (C) 2013-2014 Ryan Seghers
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the irrevocable, perpetual, worldwide, and royalty-free
// rights to use, copy, modify, merge, publish, distribute, sublicense, 
// display, perform, create derivative works from and/or sell copies of 
// the Software, both in source and object code form, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Text;

namespace TestMySpline
{
	/// <summary>
	/// Utility methods for arrays.
	/// </summary>
	public static class ArrayUtil
	{
		/// <summary>
		/// Create a string to display the array values.
		/// </summary>
		/// <param name="array">The array</param>
		/// <param name="format">Optional. A string to use to format each value. Must contain the colon, so something like ':0.000'</param>
		public static string ToString<T>(T[] array, string format = "")
		{
			var s = new StringBuilder();
			string formatString = "{0" + format + "}";

			for (int i = 0; i < array.Length; i++)
			{
				if (i < array.Length - 1)
				{
					s.AppendFormat(formatString + ", ", array[i]);
				}
				else
				{
					s.AppendFormat(formatString, array[i]);
				}
			}

			return s.ToString();
		}
	}
}
