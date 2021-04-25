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
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using SansyHuman.UDE.Object;
using SansyHuman.UDE.Util.Math;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Management;
using static SansyHuman.UDE.Object.UDEBulletMovement;

namespace SansyHuman.UDE.ECS.Object
{
    /// <summary>
    /// Bullet for ECS and Job system.
    /// </summary>
    [AddComponentMenu("UDE/Experimental/ECS/Bullet/ECS Bullet")]
    public class UDEBulletECS : UDEAbstractBullet
    {
        /// <summary>Entity manager of the currently active world.</summary>
        protected EntityManager manager;
        /// <summary>Entity of the bullet itself.</summary>
        protected Entity selfEntity;

        /// <summary>Original of movements of the bullet. Index of the movement is the phase of it.
        /// <para>The real movement of the bullet is stored in <see cref="SansyHuman.UDE.ECS.Object.UDEBulletMovements"/> component.</para></summary>
        public UDEBulletMovementECS[] movements;

        /// <summary>Time to disable sprite and collider and enable halo.
        /// <para>During the summon time, halo marks the position the bullet will appear.</para>
        /// <para>Similar to some bullets in Touhou Project.</para></summary>
        [SerializeField] protected float summonTime = 0.08f;

        public new SpriteRenderer renderer;
        public new Collider2D collider;
        public Behaviour halo;

        /// <value>Gets the current phase of the bullet.</value>
        public int Phase
        {
            get
            {
                UDEBulletMovements movement = manager.GetComponentData<UDEBulletMovements>(selfEntity);
                return movement.Phase;
            }
        }

        /// <inheritdoc/>
        public override float SummonTime
        {
            get => summonTime;
            set
            {
                summonTime = value;
                if (summonTime < 0)
                    summonTime = 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            manager = World.Active.EntityManager;
            renderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();
            halo = GetComponent("Halo") as Behaviour;
        }

        /// <summary>
        /// When the bullet is enabled, create entity of itself and initializes component datas.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            selfEntity = manager.CreateEntity(new ComponentType[] {
                typeof(Translation), typeof(Rotation), typeof(RotationEulerXYZ), typeof(LocalToWorld), typeof(CopyTransformToGameObject),
                typeof(UDEBulletMovements), typeof(UDEPolarCoordinate), typeof(UDEBulletTimeScale), typeof(UDEOriginCharacter)
            });
            GameObjectEntity.AddToEntity(manager, self, selfEntity);
            if (halo != null)
            {
                halo.enabled = true;
                renderer.enabled = false;
                collider.enabled = false;
            }
            manager.SetComponentData<UDEBulletMovements>(selfEntity, new UDEBulletMovements() { IsSummoning = true, SummonTime = summonTime }); 
        }

        /// <summary>
        /// When the bullet is disabled, destroys the entity of itself.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if(selfEntity != null)
                manager.DestroyEntity(selfEntity);
        }

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.UDEAbstractBullet.Initialize(Vector2, Vector2, float, UDEBaseCharacter, UDEBaseShotPattern, UDEBulletMovement[], bool, bool)"/>.
        /// <para>
        /// Initializes the bullet.
        /// Sets the initial position and movements of the bullet.
        /// </para>
        /// <para>If you try to initialize the bullet that already initialized, the initialization will be ignored.</para>
        /// </summary>
        /// <param name="initPos">Initial position of the bullet</param>
        /// <param name="origin">Origin in polar coordinate</param>
        /// <param name="initRotation">Initial rotation of the bullet</param>
        /// <param name="originCharacter"><see cref="SansyHuman.UDE.Object.UDEBaseCharacter"/> instance which shot the bullet</param>
        /// <param name="originShotPattern"><see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern"/> instance which summoned the bullet. It is nullable</param>
        /// <param name="movements">Movements of the bullet. All <see cref="SansyHuman.UDE.Object.UDEBulletMovement"/> should be in the order of phase</param>
        /// <param name="setOriginToCharacter">Whether set the origin in polar coordinate to origin character's position</param>
        /// <param name="loop">Whether turn back to first movement when the last movement end</param>
        public override void Initialize(Vector2 initPos, Vector2 origin, float initRotation, UDEBaseCharacter originCharacter, UDEBaseShotPattern originShotPattern, UDEBulletMovement[] movements, bool setOriginToCharacter = false, bool loop = false)
        {
            if (initialized)
            {
                Debug.LogWarning("You tried to initalize bullet that already has initialized. The initialization is ignored.");
                return;
            }

            base.Initialize(initPos, origin, initRotation, originCharacter, originShotPattern, movements, setOriginToCharacter, loop);
            Translation translation = new Translation() { Value = (Vector3)initPos };
            RotationEulerXYZ rotation = new RotationEulerXYZ() { Value = new float3(0, 0, math.radians(initRotation)) };
            if (movements[0].faceToMovingDirection)
            {
                switch (movements[0].mode)
                {
                    case MoveMode.CARTESIAN:
                        rotation.Value.z = UDEMath.Deg(movements[0].velocity);
                        break;
                    case MoveMode.CARTESIAN_POLAR:
                        rotation.Value.z = movements[0].angle;
                        break;
                    case MoveMode.POLAR:
                        rotation.Value.z = movements[0].angularSpeed > 0 ? movements[0].angle + 90 : movements[0].angle - 90;
                        if (movements[0].angularSpeed == 0)
                            rotation.Value.z = movements[0].angle;
                        break;
                }
            }
            UDEBulletMovementECS[] movementsECS = new UDEBulletMovementECS[movements.Length];
            for (int i = 0; i < movementsECS.Length; i++)
                movementsECS[i] = movements[i].ToECSMovement();
            this.movements = movementsECS;

            UDETime.TimeScale timeScale = UDETime.TimeScale.UNSCALED;
            if (originCharacter is UDEEnemy)
                timeScale = UDETime.TimeScale.ENEMY;
            else if (originCharacter is UDEPlayer)
                timeScale = UDETime.TimeScale.PLAYER;

            UDEBulletMovements movement = new UDEBulletMovements()
            {
                Movement = this.movements[0],
                Phase = 0,
                TotalPhase = this.movements.Length,
                Time = 0,
                UsingTimeScale = timeScale,
                IsSummoning = halo != null,
                SummonTime = summonTime,
                Loop = loop
            };

            float r = (initPos - origin).magnitude;
            float angle = r > 0.01f ? UDEMath.Deg(initPos - origin) : movements[0].angle;

            UDEPolarCoordinate polarCoord = new UDEPolarCoordinate()
            {
                Origin = origin,
                Radius = r,
                Angle = angle,
                SetOriginToOriginCharacter = setOriginToCharacter
            };

            UDEBulletTimeScale scale = new UDEBulletTimeScale() { Value = UDETime.Instance.GetTimeScale(timeScale) };

            GameObjectEntity gmObjEntity = originCharacter.gameObject.GetComponent<GameObjectEntity>();
            if (gmObjEntity == null)
                gmObjEntity = originCharacter.gameObject.AddComponent<GameObjectEntity>();
            UDEOriginCharacter originChara = new UDEOriginCharacter() { Value = gmObjEntity.Entity };

            manager.SetComponentData<Translation>(selfEntity, translation);
            manager.SetComponentData<RotationEulerXYZ>(selfEntity, rotation);
            manager.SetComponentData<UDEBulletMovements>(selfEntity, movement);
            manager.SetComponentData<UDEPolarCoordinate>(selfEntity, polarCoord);
            manager.SetComponentData<UDEBulletTimeScale>(selfEntity, scale);
            manager.SetComponentData<UDEOriginCharacter>(selfEntity, originChara);

            if (movement.IsSummoning)
                manager.AddComponentData(selfEntity, new UDEBulletSummonPhase());

            this.originCharacter = originCharacter;
            this.originShotPattern = originShotPattern;
            if (originShotPattern != null)
                _ = originShotPattern + this;
            transform.SetPositionAndRotation(translation.Value, Quaternion.Euler(0, 0, math.degrees(rotation.Value.z)));
        }

        /// <summary>
        /// Initializes the bullet.
        /// Sets the initial position and movements of the bullet.
        /// <para>If you try to initialize the bullet that already initialized, the initialization will be ignored.</para>
        /// </summary>
        /// <param name="initPos">Initial position of the bullet</param>
        /// <param name="origin">Origin in polar coordinate</param>
        /// <param name="initRotation">Initial rotation of the bullet</param>
        /// <param name="originCharacter"><see cref="SansyHuman.UDE.Object.UDEBaseCharacter"/> instance which shot the bullet</param>
        /// <param name="originShotPattern"><see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern"/> instance which summoned the bullet. It is nullable</param>
        /// <param name="movement">Movement of the bullet.</param>
        /// <param name="setOriginToCharacter">Whether set the origin in polar coordinate to origin character's position</param>
        /// <param name="loop">Whether turn back to first movement when the last movement end</param>
        /// <exception cref="SansyHuman.UDE.Exception.UDEInitializationExcention">Thrown when you try to initialize bullets that already initialized</exception>
        public override void Initialize(Vector2 initPos, Vector2 origin, float initRotation, UDEBaseCharacter originCharacter, UDEBaseShotPattern originShotPattern, in UDEBulletMovement movement, bool setOriginToCharacter = false, bool loop = false)
        {
            Initialize(initPos, origin, initRotation, originCharacter, originShotPattern, new UDEBulletMovement[] { movement }, setOriginToCharacter, loop);
        }

        /// <summary>
        /// Sets forcefully to the next phase of the bullet.
        /// </summary>
        public void ForceMoveToNextPhase()
        {
            ForceMoveToPhase(Phase + 1);
        }

        /// <summary>
        /// Sets forcefully to the phase.
        /// </summary>
        /// <param name="phase">Phase to set</param>
        public void ForceMoveToPhase(int phase)
        {
            if (phase < 0 || phase > movements.Length - 1)
                return;

            UDEBulletMovements movement = manager.GetComponentData<UDEBulletMovements>(selfEntity);
            if (movement.Phase == phase)
                return;

            UDEBulletMovementECS prev = movement.Movement;
            movement.Movement = movements[phase];
            UDEPolarCoordinate polarCoord = manager.GetComponentData<UDEPolarCoordinate>(selfEntity);
            if (movement.Movement.setSpeedToPrevMovement)
                Management.UDEBulletMovementSystem.PhaseUpdateJob.SyncMovementSpeed(ref prev, ref movement.Movement, ref polarCoord);
            movement.Phase = phase;

            manager.SetComponentData<UDEBulletMovements>(selfEntity, movement);
        }
    }
}