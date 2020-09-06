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
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using SansyHuman.UDE.ECS.Object;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Object;
using static SansyHuman.UDE.Object.UDEBulletMovement;

namespace SansyHuman.UDE.ECS.Management
{
    /// <summary>
    /// Component system that calculates bullets' movements using multi-thread.
    /// </summary>
    public class UDEBulletMovementSystem : JobComponentSystem
    {
        static readonly EntityManager manager = World.Active.EntityManager;

        /// <summary>
        /// Struct for job that calculates movements of each bullets.
        /// This job supports burst compile.
        /// </summary>
        [BurstCompile]
        struct MovementJob : IJobForEachWithEntity<Translation, RotationEulerXYZ, UDEBulletMovements, UDEPolarCoordinate, UDEBulletTimeScale>
        {
            /// <summary>Delta time from the last call.</summary>
            public float deltaTime;

            /// <summary>
            /// Calculates the movement of a bullet.
            /// <para>Only used internally.</para>
            /// </summary>
            /// <param name="entity">Bullet's entity</param>
            /// <param name="index">Not used</param>
            /// <param name="translation"><see cref="Unity.Transforms.Translation"/> data</param>
            /// <param name="rotation"><see cref="Unity.Transforms.RotationEulerXYZ"/> data</param>
            /// <param name="movement"><see cref="SansyHuman.UDE.ECS.Object.UDEBulletMovements"/> data</param>
            /// <param name="polarCoord"><see cref="SansyHuman.UDE.ECS.Object.UDEPolarCoordinate"/> data</param>
            /// <param name="timeScale"><see cref="SansyHuman.UDE.ECS.Object.UDEBulletTimeScale"/> data</param>
            public void Execute(Entity entity, int index, ref Translation translation,
                                ref RotationEulerXYZ rotation,
                                ref UDEBulletMovements movement,
                                ref UDEPolarCoordinate polarCoord,
                                [ReadOnly] ref UDEBulletTimeScale timeScale)
            {
                float scaledDeltaTime = deltaTime * timeScale.Value;
                movement.Time += scaledDeltaTime;

                if (movement.Movement.hasEndTime && movement.Time > movement.Movement.endTime)
                    return;
                if (movement.Time < movement.Movement.startTime && movement.Phase == 0)
                    return;

                ref UDEBulletMovementECS currentMovement = ref movement.Movement;
                float2 displacement;
                switch (currentMovement.mode)
                {
                    case MoveMode.CARTESIAN:
                        displacement = CartesianMove(scaledDeltaTime, ref currentMovement, ref rotation);
                        break;
                    case MoveMode.CARTESIAN_POLAR:
                        displacement = CartesianPolarMove(scaledDeltaTime, ref currentMovement, ref rotation);
                        break;
                    case MoveMode.POLAR:
                        displacement = PolarMove(scaledDeltaTime, ref currentMovement, ref rotation, ref polarCoord);
                        break;
                    default:
                        displacement = new Vector2(0, 0);
                        break;
                }

                translation.Value += new float3(displacement.x, displacement.y, 0);
                if (currentMovement.mode != MoveMode.POLAR)
                {
                    float3 origin = new float3(polarCoord.Origin.x, polarCoord.Origin.y, 0);
                    float3 radius = translation.Value - origin;
                    polarCoord.Radius = math.length(radius);
                    polarCoord.Angle = polarCoord.Radius > 0.01f ? UDEMath.Deg(radius.x, radius.y) : rotation.Value.z;
                }
                currentMovement.updateCount++;
            }

            // Calculates the displacement of the bullet in cartesian mode.
            private static float2 CartesianMove(float deltaTime, ref UDEBulletMovementECS movement, ref RotationEulerXYZ rot)
            {
                float2 dr = movement.velocity * deltaTime;
                if (movement.faceToMovingDirection)
                    rot.Value.z = math.radians(UDEMath.Deg(movement.velocity));
                else
                    SetRotation(deltaTime, ref movement, ref rot);

                movement.velocity += movement.accel * deltaTime;
                if (movement.limitSpeed)
                {
                    float speed = math.length(movement.velocity);
                    if (speed > movement.maxMagnitude)
                        movement.velocity = movement.velocity * movement.maxMagnitude / speed;
                    if (speed < movement.minMagnitude)
                        movement.velocity = movement.velocity * movement.minMagnitude / speed;

                    if (movement.velocity.x > movement.maxVelocity.x)
                        movement.velocity.x = movement.maxVelocity.x;
                    if (movement.velocity.y > movement.maxVelocity.y)
                        movement.velocity.y = movement.maxVelocity.y;
                    if (movement.velocity.x < movement.minVelocity.x)
                        movement.velocity.x = movement.minVelocity.x;
                    if (movement.velocity.y < movement.minVelocity.y)
                        movement.velocity.y = movement.minVelocity.y;
                }
                return dr;
            }

            // Calculates the displacement of the bullet in cartesian-polar mode.
            private static float2 CartesianPolarMove(float deltaTime, ref UDEBulletMovementECS movement, ref RotationEulerXYZ rot)
            {
                float dx = movement.speed * math.cos(math.radians(movement.angle)) * deltaTime;
                float dy = movement.speed * Mathf.Sin(math.radians(movement.angle)) * deltaTime;
                if (movement.faceToMovingDirection)
                    rot.Value.z = math.radians(movement.angle);
                else
                    SetRotation(deltaTime, ref movement, ref rot);

                movement.speed += movement.tangentialAccel * deltaTime;
                movement.angle += movement.normalAccel * deltaTime;
                while (movement.angle > 360)
                    movement.angle -= 360;
                while (movement.angle < 0)
                    movement.angle += 360;
                if (movement.limitSpeed)
                {
                    if (movement.speed > movement.maxSpeed)
                        movement.speed = movement.maxSpeed;
                    if (movement.speed < movement.minSpeed)
                        movement.speed = movement.minSpeed;
                }

                return new float2(dx, dy);
            }

            // Calculates the displacement of the bullet in polar coordinate system.
            private static float2 PolarMove(float deltaTime, ref UDEBulletMovementECS movement, ref RotationEulerXYZ rot, ref UDEPolarCoordinate polar)
            {
                float xPre, yPre;
                UDEMath.Polar2Cartesian(polar.Radius, polar.Angle, out xPre, out yPre);
                float2 rPre = new float2(xPre, yPre);

                polar.Radius += movement.radialSpeed * deltaTime;
                movement.radialSpeed += movement.radialAccel * deltaTime;
                polar.Angle += movement.angularSpeed * deltaTime;
                movement.angularSpeed += movement.angularAccel * deltaTime;
                while (polar.Angle > 360)
                    polar.Angle -= 360;
                while (polar.Angle < 0)
                    polar.Angle += 360;

                if (movement.limitSpeed)
                {
                    if (movement.radialSpeed > movement.maxRadialSpeed)
                        movement.radialSpeed = movement.maxRadialSpeed;
                    if (movement.radialSpeed < movement.minRadialSpeed)
                        movement.radialSpeed = movement.minRadialSpeed;
                    if (movement.angularSpeed > movement.maxAngularSpeed)
                        movement.angularSpeed = movement.maxAngularSpeed;
                    if (movement.angularSpeed < movement.minAngularSpeed)
                        movement.angularSpeed = movement.minAngularSpeed;
                }
                float x, y;
                UDEMath.Polar2Cartesian(polar.Radius, polar.Angle, out x, out y);
                movement.angle = polar.Angle;

                float2 displacement = new float2(x, y) - rPre;
                if (movement.faceToMovingDirection)
                    rot.Value.z = math.lengthsq(displacement) > 0.0001f ? math.radians(UDEMath.Deg(displacement)) : rot.Value.z;
                else
                    SetRotation(deltaTime, ref movement, ref rot);

                return displacement;
            }

            // Sets rotation if faceToMovingDirection in UDEBulletMovement is false.
            private static void SetRotation(float deltaTime, ref UDEBulletMovementECS movement, ref RotationEulerXYZ rotation)
            {
                float newRot = math.degrees(rotation.Value.z);
                newRot += movement.rotationAngularSpeed * deltaTime;
                while (newRot > 360)
                    newRot -= 360;
                while (newRot < 0)
                    newRot += 360;

                movement.rotationAngularSpeed += movement.rotationAngularAcceleration * deltaTime;
                if (movement.limitRotationSpeed)
                {
                    if (movement.rotationAngularSpeed > movement.maxRotationSpeed)
                        movement.rotationAngularSpeed = movement.maxRotationSpeed;
                    if (movement.rotationAngularSpeed < movement.minRotationSpeed)
                        movement.rotationAngularSpeed = movement.minRotationSpeed;
                }
                rotation.Value.z = math.radians(newRot);
            }
        }

        /// <summary>
        /// Struct for job that updates phases of bullets.
        /// </summary>
        internal struct PhaseUpdateJob : IJobForEachWithEntity<UDEBulletMovements, UDEPolarCoordinate>
        {
            /// <summary>
            /// Updates the phase of a bullet.
            /// <para>Only used internally.</para>
            /// </summary>
            /// <param name="entity">Bullet's entity</param>
            /// <param name="index">Not used</param>
            /// <param name="movement"><see cref="SansyHuman.UDE.ECS.Object.UDEBulletMovements"/> data</param>
            /// <param name="polarCoord"><see cref="SansyHuman.UDE.ECS.Object.UDEPolarCoordinate"/> data</param>
            public void Execute(Entity entity, int index, ref UDEBulletMovements movement, ref UDEPolarCoordinate polarCoord)
            {
                UDEBulletECS bullet = manager.GetComponentObject<UDEBulletECS>(entity);
                int phase = movement.Phase;
                while (!(phase >= movement.TotalPhase - 1) && movement.Time >= bullet.movements[phase + 1].startTime)
                {
                    phase++;
                }
                if (phase != movement.Phase)
                {
                    UDEBulletMovementECS prevMovement = movement.Movement;
                    movement.Movement = bullet.movements[phase];
                    if (movement.Movement.setSpeedToPrevMovement)
                        SyncMovementSpeed(ref prevMovement, ref movement.Movement, ref polarCoord);
                    movement.Phase = phase;
                }

                if (movement.Movement.hasEndTime && movement.Time >= movement.Movement.endTime)
                {
                    if (movement.Phase == movement.TotalPhase - 1 && movement.Loop)
                    {
                        UDEBulletMovementECS prevMovement = movement.Movement;
                        movement.Movement = bullet.movements[0];
                        if (movement.Movement.setSpeedToPrevMovement)
                            SyncMovementSpeed(ref prevMovement, ref movement.Movement, ref polarCoord);
                        movement.Phase = 0;
                        movement.Time = 0;
                    }
                }
            }

            /// <summary>
            /// Same to <see cref="SansyHuman.UDE.Object.UDEBaseBullet.SyncMovementSpeed(ref UDEBulletMovement, ref UDEBulletMovement)"/>
            /// but for ECS bullets. It is used internally in this job and <see cref="SansyHuman.UDE.ECS.Object.UDEBulletECS.ForceMoveToPhase(int)"/>.
            /// </summary>
            /// <param name="prev">Previous movement</param>
            /// <param name="next">Next movement</param>
            /// <param name="polarCoord">Polar coordinates of the bullet</param>
            /// <seealso cref="SansyHuman.UDE.Object.UDEBaseBullet.SyncMovementSpeed(ref UDEBulletMovement, ref UDEBulletMovement)"/>
            internal static void SyncMovementSpeed(ref UDEBulletMovementECS prev, ref UDEBulletMovementECS next, ref UDEPolarCoordinate polarCoord)
            {
                float angle = polarCoord.Angle;
                Vector2 prevVelocity = new Vector2(0, 0);
                switch (prev.mode)
                {
                    case MoveMode.CARTESIAN:
                        prevVelocity = prev.velocity;
                        break;
                    case MoveMode.CARTESIAN_POLAR:
                        var velocityTuple = UDEMath.Polar2Cartesian(prev.speed, prev.angle);
                        prevVelocity = new Vector2(velocityTuple.x, velocityTuple.y);
                        break;
                    case MoveMode.POLAR:
                        var radialVel = UDEMath.Polar2Cartesian(prev.radialSpeed, angle);
                        var angularVel = UDEMath.Polar2Cartesian(prev.angularSpeed * Mathf.Deg2Rad * polarCoord.Radius, angle + 90f);
                        prevVelocity = new Vector2(radialVel.x + angularVel.x, radialVel.y + angularVel.y);
                        break;
                }

                switch (next.mode)
                {
                    case MoveMode.CARTESIAN:
                        next.velocity = prevVelocity;
                        break;
                    case MoveMode.CARTESIAN_POLAR:
                        (float speed, float angle) polarVel = UDEMath.Cartesian2Polar(prevVelocity);
                        next.speed = polarVel.speed;
                        next.angle = polarVel.angle;
                        break;
                    case MoveMode.POLAR:
                        (float speed, float angle) polarVel2 = UDEMath.Cartesian2Polar(prevVelocity);
                        next.radialSpeed = polarVel2.speed * Mathf.Cos((polarVel2.angle - angle) * Mathf.Deg2Rad);
                        next.angularSpeed = polarVel2.speed * Mathf.Sin((polarVel2.angle - angle) * Mathf.Deg2Rad) * Mathf.Rad2Deg / polarCoord.Radius;
                        break;
                }
            }
        }

        /// <summary>
        /// Struct for job that updates origin of polar coordinate and time scale.
        /// </summary>
        struct PolarCoordAndTimeScaleUpdateJob : IJobForEach<Translation, UDEBulletMovements, UDEPolarCoordinate, UDEOriginCharacter, UDEBulletTimeScale>
        {
            /// <summary>
            /// Updates origin and time scale of the bullet.
            /// <para>Only used internally.</para>
            /// </summary>
            /// <param name="translation"><see cref=Unity.Transforms.Translation"/> data</param>
            /// <param name="movements"><see cref="SansyHuman.UDE.ECS.Object.UDEBulletMovements"/> data</param>
            /// <param name="polarCoord"><see cref="SansyHuman.UDE.ECS.Object.UDEPolarCoordinate"/> data</param>
            /// <param name="originCharacter"><see cref="SansyHuman.UDE.ECS.Object.UDEOriginCharacter"/> data</param>
            /// <param name="timeScale"><see cref="SansyHuman.UDE.ECS.Object.UDEBulletTimeScale"/> data</param>
            public void Execute(ref Translation translation,
                                [ReadOnly] ref UDEBulletMovements movements,
                                ref UDEPolarCoordinate polarCoord,
                                [ReadOnly] ref UDEOriginCharacter originCharacter,
                                [WriteOnly] ref UDEBulletTimeScale timeScale)
            {
                timeScale.Value = UDETime.Instance.GetTimeScale(movements.UsingTimeScale);
                if (polarCoord.SetOriginToOriginCharacter)
                {
                    float3 origin = manager.GetComponentObject<UDEEnemy>(originCharacter.Value).position;
                    translation.Value += (origin - new float3(polarCoord.Origin.x, polarCoord.Origin.y, 0));
                    polarCoord.Origin = new float2(origin.x, origin.y);
                }
            }
        }

        /// <summary>
        /// Executes all jobs in sequence.
        /// <para>Only used internally.</para>
        /// </summary>
        /// <param name="inputDeps">Job to be done first</param>
        /// <returns><see cref="Unity.Jobs.JobHandle"/> instance that contains the information of the last scheduled job</returns>
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MovementJob job = new MovementJob() { deltaTime = Time.deltaTime };
            
            PhaseUpdateJob job2 = new PhaseUpdateJob();
            PolarCoordAndTimeScaleUpdateJob job3 = new PolarCoordAndTimeScaleUpdateJob();
            JobHandle movement = job.Schedule(this, inputDeps);
            JobHandle phase = job2.Schedule(this, movement);
            JobHandle timeScale = job3.Schedule(this, phase);
            return timeScale;
        }
    }
}