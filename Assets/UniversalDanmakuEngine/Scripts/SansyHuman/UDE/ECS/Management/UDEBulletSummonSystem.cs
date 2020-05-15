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

using Unity.Entities;
using SansyHuman.UDE.ECS.Object;

namespace SansyHuman.UDE.ECS.Management
{
    /// <summary>
    /// Component system that handles the summon state of ECS bullets.
    /// </summary>
    public class UDEBulletSummonSystem : ComponentSystem
    {
        /// <summary>
        /// Override of <see cref="Unity.Entities.ComponentSystem.OnUpdate()"/>.
        /// Checks bullet entities who are in summon phase and updates there states.
        /// <para>Only used internally.</para>
        /// </summary>
        protected override void OnUpdate()
        {
            EntityManager manager = World.Active.EntityManager;
            Entities.WithAll<UDEBulletMovements, UDEBulletSummonPhase>().ForEach((Entity entity, ref UDEBulletMovements movement) =>
            {
                if (movement.IsSummoning)
                {
                    UDEBulletECS bullet = manager.GetComponentObject<UDEBulletECS>(entity);
                    if (bullet.halo == null)
                    {
                        movement.IsSummoning = false;
                        manager.RemoveComponent<UDEBulletSummonPhase>(entity);
                        return;
                    }
                    else
                    {
                        if (movement.Time > movement.SummonTime)
                        {
                            movement.Time = 0;
                            bullet.halo.enabled = false;
                            bullet.renderer.enabled = true;
                            bullet.collider.enabled = true;
                            movement.IsSummoning = false;
                            manager.RemoveComponent<UDEBulletSummonPhase>(entity);
                            return;
                        }
                    }
                }
            });
        }
    }
}