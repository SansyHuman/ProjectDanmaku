// Copyright (c) 2019 Subo Lee (KAIST HAJE)
// Please direct any bugs/comments/suggestions to suboo0308@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SansyHuman.UDE.Exception
{
    /// <summary>
    /// Exception class thrown when builders that inherits <see cref="SansyHuman.UDE.Util.Builder.UDECommonBulletMovementBuilder{T}"/>
    /// tries to build invalid <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> instance.
    /// </summary>
    /// <seealso cref="SansyHuman.UDE.Util.Builder.UDECommonBulletMovementBuilder{T}"/>
    /// <seealso cref="SansyHuman.UDE.Util.Builder.UDECartesianPolarMovementBuilder"/>
    /// <seealso cref="SansyHuman.UDE.Util.Builder.UDEPolarMovementBuilder"/>
    public class UDEInvalidBulletMovementException : System.Exception
    {
        /// <summary>
        /// Basic constructor with no message.
        /// </summary>
        public UDEInvalidBulletMovementException() : base() { }
        /// <summary>
        /// Constructor with a message.
        /// </summary>
        /// <param name="message">Description of the exception</param>
        public UDEInvalidBulletMovementException(string message) : base(message) { }
    }
}