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

using UnityEngine;

namespace SansyHuman
{
    /// <summary>
    /// Namespace of all Universal Danmaku Engine scripts.
    /// </summary>
    namespace UDE
    {
        /// <summary>
        /// Namespace of scripts for ECS and Job system(Experimental and not stable).
        /// </summary>
        namespace ECS
        {
            /// <summary>
            /// Namespace of component systems of ECS.
            /// </summary>
            namespace Management { }
            /// <summary>
            /// Namespace of entities and components of ECS.
            /// </summary>
            namespace Object { }
            
        }
        /// <summary>
        /// Namespace of exception classes.
        /// </summary>
        namespace Exception { }
        /// <summary>
        /// Namespace of singleton classes that manage the game.
        /// </summary>
        namespace Management { }
        /// <summary>
        /// Namespace of objects such as character, bullet, and laser.
        /// </summary>
        namespace Object { }
        /// <summary>
        /// Namespace of scripts for patterns.
        /// </summary>
        namespace Pattern { }
        /// <summary>
        /// Namespace of useful utilities.
        /// </summary>
        namespace Util
        {
            /// <summary>
            /// Namespace of builders of <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/>.
            /// </summary>
            namespace Builder { }
            /// <summary>
            /// Namespace of math libraries.
            /// </summary>
            namespace Math { }
        }
    }
}