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
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Exception;
using SansyHuman.UDE.Util.Math;
using static SansyHuman.UDE.Object.UDEBulletMovement;
using System;

namespace SansyHuman.UDE.Object
{
    /// <summary>
    /// The Base class of all bullets.
    /// </summary>
    [AddComponentMenu("UDE/Bullet/Base Bullet")]
    [DisallowMultipleComponent]
    public class UDEBaseBullet : UDEAbstractBullet
    {
        #region Fields

        /// <summary>Time scale the bullet uses.</summary>
        [SerializeField] protected UDETime.TimeScale usingTimeScale;

        /// <summary>Origin of bullet's local coordinate.</summary>
        [SerializeField] protected Vector2 origin;
        /// <summary>Whether change origin to the position of origin character.
        /// <para>If true, bullet follows the character when it is in polar mode.</para>
        /// </summary>
        [SerializeField] protected bool setOriginToCharacter;
        /// <summary>Current position of the bullet.</summary>
        [SerializeField] protected Vector2 position;
        /// <summary>Current rotation angle of the bullet.
        /// <para>The unit is degree and right horizon is 0.</para>
        /// </summary>
        [SerializeField] protected float rotation;

        /// <summary>Distance from origin in polar coordinate system.</summary>
        [SerializeField] protected float r;
        /// <summary>Angle in polar coordinate system.</summary>
        [SerializeField] protected float angle;

        /// <summary>Current phase of movement.</summary>
        protected int phase = 0;
        /// <summary>Time passed from the initialization.</summary>
        protected float time = 0;

        /// <summary>Time to disable sprite and collider and enable halo.
        /// <para>During the summon time, halo marks the position the bullet will appear.</para>
        /// <para>Similar to some bullets in Touhou Project.</para></summary>
        [SerializeField] private float summonTime = 0.08f;
        /// <summary>Whether the bullet is summoning. Only used internally.</summary>
        [SerializeField] protected bool isSummoning = false;

        /// <summary>Original copy of movements.</summary>
        protected UDEBulletMovement[] movementsOriginal;
        /// <summary>Movements of the bullet. Index of the movement is the phase of it.</summary>
        protected UDEBulletMovement[] movements;
        /// <summary>Whether loop the movement. If true, turns back to first movement when last movement ends.</summary>
        protected bool loop;

        /// <summary>Delegate of <see cref="SansyHuman.UDE.Object.UDEBaseBullet.MoveBullet(float)"/>.</summary>
        protected UDEObjectManager.ObjectMoveHandler moveHandler;

        /// <value>Gets the current phase of the bullet.</value>
        public int Phase { get => phase; }
        /// <value>Gets the square of distance of the bullet from origin.</value>
        public float SqrMagnitudeFromOrigin { get => Vector2.SqrMagnitude(position - origin); }
        /// <value>Gets the current movement.</value>
        public ref UDEBulletMovement CurrentMovement { get => ref movements[phase]; }
        /// <value>Gets the movement in phase.</value>
        public ref UDEBulletMovement this[int phase] { get => ref movements[phase]; }
        /// <value>Sets the summon time(default value is 0.08 seconds.</value>
        public float SummonTime
        {
            get => summonTime;
            set
            { 
                summonTime = value;
                if (summonTime < 0)
                    summonTime = 0;
            } 
        }
        #endregion

        #region Initialize and Dispose
        /// <summary><see cref="SpriteRenderer"/> of the bullet. Only assigned internally.</summary>
        protected new SpriteRenderer renderer;
        /// <summary>Collider of the bullet. Only assigned internally.</summary>
        protected new Collider2D collider;
        /// <summary>Halo of the bullet. Only assigned internally.</summary>
        protected Behaviour halo;

        protected override void Awake()
        {
            base.Awake();

            moveHandler = new UDEObjectManager.ObjectMoveHandler(MoveBullet);

            renderer = gameObject.GetComponent<SpriteRenderer>();
            collider = gameObject.GetComponent<Collider2D>();
            halo = gameObject.GetComponent("Halo") as Behaviour;
        }

        /// <summary>
        /// When the bullet is enabled, initializes some values and registers itself to <see cref="SansyHuman.UDE.Management.UDEObjectManager"/>.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            time = 0;
            phase = 0;

            if (halo != null)
            {
                renderer.enabled = false;
                collider.enabled = false;
                isSummoning = true;
            }
        }

        /// <summary>
        /// When the bullet is disabled, deregisters itself from <see cref="SansyHuman.UDE.Management.UDEObjectManager"/>.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            UDEObjectManager.Instance.MoveObjects -= moveHandler;

            if (halo != null)
                halo.enabled = true;
        }

        /// <summary><see cref="Transform"/> of the origin character. Only assigned internally.</summary>
        protected Transform originChrTr;

        /// <summary>
        /// Override of <see cref="SansyHuman.UDE.Object.UDEAbstractBullet.Initialize(Vector2, Vector2, float, UDEBaseCharacter, UDEBaseShotPattern, UDEBulletMovement[], bool, bool)"/>.
        /// <para>
        /// Initializes the bullet.
        /// Sets the initial position and movements of the bullet.
        /// </para>
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

            this.position = initPos;
            this.origin = origin;
            this.rotation = initRotation;

            this.originCharacter = originCharacter;
            this.originShotPattern = originShotPattern;
            this.setOriginToCharacter = setOriginToCharacter;

            this.loop = loop;
            this.movements = movements;
            this.movementsOriginal = (UDEBulletMovement[])movements.Clone();

            r = (initPos - origin).magnitude;
            angle = r > 0.01f ? UDEMath.Deg(initPos - origin) : movements[0].angle;
            if (movements[0].faceToMovingDirection)
            {
                switch (movements[0].mode)
                {
                    case MoveMode.CARTESIAN:
                        rotation = UDEMath.Deg(movements[0].velocity);
                        break;
                    case MoveMode.CARTESIAN_POLAR:
                        rotation = movements[0].angle;
                        break;
                    case MoveMode.POLAR:
                        rotation = movements[0].angularSpeed > 0 ? movements[0].angle + 90 : movements[0].angle - 90;
                        if (movements[0].angularSpeed == 0)
                            rotation = movements[0].angle;
                        break;
                }
            }

            if (this.originCharacter is UDEEnemy)
                this.usingTimeScale = UDETime.TimeScale.ENEMY;
            else if (this.originCharacter is UDEPlayer)
                this.usingTimeScale = UDETime.TimeScale.PLAYER;
            else
                this.usingTimeScale = UDETime.TimeScale.UNSCALED;

            originChrTr = this.originCharacter.transform;

            UDEObjectManager.Instance.MoveObjects += moveHandler;
            if (originShotPattern != null)
                _ = originShotPattern + this;
            bulletTr.SetPositionAndRotation(position, Quaternion.Euler(0, 0, rotation));
        }

        /// <summary>
        /// Initializes the bullet.
        /// Sets the initial position and movements of the bullet.
        /// </summary>
        /// <param name="initPos">Initial position of the bullet</param>
        /// <param name="origin">Origin in polar coordinate</param>
        /// <param name="initRotation">Initial rotation of the bullet</param>
        /// <param name="originCharacter"><see cref="SansyHuman.UDE.Object.UDEBaseCharacter"/> instance which shot the bullet</param>
        /// <param name="originShotPattern"><see cref="SansyHuman.UDE.Pattern.UDEBaseShotPattern"/> instance which summoned the bullet. It is nullable</param>
        /// <param name="movement">Movement of the bullet.</param>
        /// <param name="setOriginToCharacter">Whether set the origin in polar coordinate to origin character's position</param>
        /// <param name="loop">Whether turn back to first movement when the last movement end</param>
        public override void Initialize(Vector2 initPos, Vector2 origin, float initRotation, UDEBaseCharacter originCharacter, UDEBaseShotPattern originShotPattern, UDEBulletMovement movement, bool setOriginToCharacter = false, bool loop = false)
        {
            this.Initialize(initPos, origin, initRotation, originCharacter, originShotPattern, new UDEBulletMovement[] { movement }, setOriginToCharacter, loop);
        }
        #endregion

        #region Bullet Movement
        /// <summary>
        /// Moves the bullet. Only called internally in <see cref="SansyHuman.UDE.Management.UDEObjectManager"/>
        /// </summary>
        /// <param name="deltaTime">
        /// <para>The passed time from the previous frame.</para>
        /// <para>It is recommended to pass <see cref="UnityEngine.Time.deltaTime"/></para>
        /// </param>
        protected virtual void MoveBullet(float deltaTime)
        {
            float scaledDeltaTime;
            switch (usingTimeScale)
            {
                case UDETime.TimeScale.ENEMY:
                    scaledDeltaTime = deltaTime * UDETime.Instance.EnemyTimeScale;
                    break;
                case UDETime.TimeScale.PLAYER:
                    scaledDeltaTime = deltaTime * UDETime.Instance.PlayerTimeScale;
                    break;
                case UDETime.TimeScale.UNSCALED:
                default:
                    scaledDeltaTime = deltaTime;
                    break;
            }
            time += scaledDeltaTime;

            if (halo != null)
            {
                if (isSummoning)
                {
                    if (time > summonTime)
                    {
                        renderer.enabled = true;
                        collider.enabled = true;
                        halo.enabled = false;
                        isSummoning = false;
                        time = 0;
                    }
                }
            }

            int prevPhase = phase;
            while (!(phase >= movements.Length - 1) && time >= movements[phase + 1].startTime)
            {
                phase++;
            }
            if (phase != prevPhase)
            {
                movements[phase] = movementsOriginal[phase];
                if (movements[phase].setSpeedToPrevMovement)
                    SyncMovementSpeed(ref movements[prevPhase], ref movements[phase]);
            }
            if (movements[phase].hasEndTime && time >= movements[phase].endTime)
            {
                if (phase == movements.Length - 1 && loop)
                {
                    movements[0] = movementsOriginal[0];
                    if (movements[0].setSpeedToPrevMovement)
                        SyncMovementSpeed(ref movements[phase], ref movements[0]);
                    phase = 0;
                    time = movements[0].startTime;
                }
                else
                    return;
            }
            if (phase == 0 && time < movements[phase].startTime)
                return;

            ref UDEBulletMovement currentMovement = ref movements[phase];
            Vector2 displacement;
            switch (currentMovement.mode)
            {
                case MoveMode.CARTESIAN:
                    displacement = CartesianMove(scaledDeltaTime, ref currentMovement);
                    break;
                case MoveMode.CARTESIAN_POLAR:
                    displacement = CartesianPolarMove(scaledDeltaTime, ref currentMovement);
                    break;
                case MoveMode.POLAR:
                    displacement = PolarMove(scaledDeltaTime, ref currentMovement);
                    break;
                default:
                    displacement = new Vector2(0, 0);
                    break;
            }

            position += displacement;
            if (currentMovement.mode != MoveMode.POLAR)
            {
                if (setOriginToCharacter)
                    origin = originChrTr.position;
                r = (position - origin).magnitude;
                angle = r > 0.01f ? UDEMath.Deg(position - origin) : rotation;
            }
            bulletTr.SetPositionAndRotation(position, Quaternion.Euler(0, 0, rotation));
            currentMovement.updateCount++;
        }

        /// <summary>
        /// Sets the next movement's initial speed to current speed.
        /// </summary>
        /// <param name="prev">previous movement</param>
        /// <param name="next">next movement</param>
        protected void SyncMovementSpeed(ref UDEBulletMovement prev, ref UDEBulletMovement next)
        {
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
                    var radialVel = UDEMath.Polar2Cartesian(prev.radialSpeed, this.angle);
                    var angularVel = UDEMath.Polar2Cartesian(prev.angularSpeed, this.angle + 90f);
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
                    next.radialSpeed = polarVel2.speed * Mathf.Cos(polarVel2.angle - this.angle);
                    next.angularSpeed = polarVel2.speed * Mathf.Sin(polarVel2.angle - this.angle);
                    break;
            }
        }

        // Calculates the displacement of the bullet in cartesian mode.
        private Vector2 CartesianMove(float deltaTime, ref UDEBulletMovement movement)
        {
            Vector2 dr = movement.velocity * deltaTime;
            if (movement.faceToMovingDirection)
                rotation = UDEMath.Deg(movement.velocity);
            else
                SetRotation(deltaTime, ref movement);

            float passedTime = time - movement.startTime;
            movement.velocity += (Vector2)movement.accel(passedTime) * deltaTime;
            if (movement.limitSpeed)
            {
                if (movement.velocity.magnitude > movement.maxMagnitude)
                    movement.velocity = movement.velocity * movement.maxMagnitude / movement.velocity.magnitude;
                if (movement.velocity.magnitude < movement.minMagnitude)
                    movement.velocity = movement.velocity * movement.minMagnitude / movement.velocity.magnitude;
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
        private Vector2 CartesianPolarMove(float deltaTime, ref UDEBulletMovement movement)
        {
            float dx = movement.speed * Mathf.Cos(movement.angle * Mathf.Deg2Rad) * deltaTime;
            float dy = movement.speed * Mathf.Sin(movement.angle * Mathf.Deg2Rad) * deltaTime;
            if (movement.faceToMovingDirection)
                rotation = movement.angle;
            else
                SetRotation(deltaTime, ref movement);

            float passedTime = time - movement.startTime;
            movement.speed += movement.tangentialAccel(passedTime) * deltaTime;
            movement.angle += movement.normalAccel(passedTime) * deltaTime;
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
            
            return new Vector2(dx, dy);
        }
        
        // Calculates the displacement of the bullet in polar coordinate system.
        private Vector2 PolarMove(float deltaTime, ref UDEBulletMovement movement)
        {
            float xPre, yPre;
            UDEMath.Polar2Cartesian(r, angle, out xPre, out yPre);
            Vector2 rPre = new Vector2(xPre, yPre);

            float passedTime = time - movement.startTime;
            r += movement.radialSpeed * deltaTime;
            movement.radialSpeed += movement.radialAccel(passedTime) * deltaTime;
            angle += movement.angularSpeed * deltaTime;
            movement.angularSpeed += movement.angularAccel(passedTime) * deltaTime;
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;

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
            UDEMath.Polar2Cartesian(r, angle, out x, out y);
            movement.angle = angle;

            Vector2 originDisplacement = Vector2.zero;
            if (setOriginToCharacter && originCharacter != null)
            {
                originDisplacement = originCharacter.transform.position - new Vector3(origin.x, origin.y);
                origin = originChrTr.position;
            }

            Vector2 displacement = new Vector2(x, y) - rPre;
            if (movement.faceToMovingDirection)
                rotation = displacement.sqrMagnitude > 0.0001f ? UDEMath.Deg(displacement) : rotation;
            else
                SetRotation(deltaTime, ref movement);

            return displacement + originDisplacement;
        }
        
        // Sets rotation if faceToMovingDirection in UDEBulletMovement is false.
        private void SetRotation(float deltaTime, ref UDEBulletMovement movement)
        {
            rotation += movement.rotationAngularSpeed * deltaTime;
            while (rotation > 360)
                rotation -= 360;
            while (rotation < 0)
                rotation += 360;

            float passedTime = time - movement.startTime;
            movement.rotationAngularSpeed += movement.rotationAngularAcceleration(passedTime) * deltaTime;
            if (movement.limitRotationSpeed)
            {
                if (movement.rotationAngularSpeed > movement.maxRotationSpeed)
                    movement.rotationAngularSpeed = movement.maxRotationSpeed;
                if (movement.rotationAngularSpeed < movement.minRotationSpeed)
                    movement.rotationAngularSpeed = movement.minRotationSpeed;
            }
        }
        #endregion

        /// <summary>
        /// Sets forcefully to the next phase of the bullet.
        /// </summary>
        public void ForceMoveToNextPhase()
        {
            ForceMoveToPhase(phase + 1);
        }

        /// <summary>
        /// Sets forcefully to the phase.
        /// </summary>
        /// <param name="phase">Phase to set</param>
        public void ForceMoveToPhase(int phase)
        {
            if (this.phase == phase || phase > movements.Length - 1 || phase < 0)
                return;

            if (this.phase <= phase)
                for (int i = this.phase + 1; i <= phase; i++)
                    movements[i] = movementsOriginal[i];
            else
                for (int i = this.phase - 1; i >= phase; i--)
                    movements[i] = movementsOriginal[i];

            if (movements[phase].setSpeedToPrevMovement)
                SyncMovementSpeed(ref movements[this.phase], ref movements[phase]);

            movements[this.phase] = movementsOriginal[this.phase];

            time = movements[phase].startTime;
            this.phase = phase;
        }

        /// <summary>
        /// Override of the operator <c>++</c>. Sets forcefully to the next phase.
        /// </summary>
        /// <param name="bullet">Bullet to set the phase to next</param>
        /// <returns>Itself</returns>
        public static UDEBaseBullet operator ++(UDEBaseBullet bullet)
        {
            bullet.ForceMoveToNextPhase();
            return bullet;
        }
    }
}