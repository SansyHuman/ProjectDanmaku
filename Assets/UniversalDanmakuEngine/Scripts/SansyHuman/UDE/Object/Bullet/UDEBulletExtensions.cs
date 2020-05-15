using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SansyHuman.UDE.Util.Builder;
using SansyHuman.UDE.Pattern;
using SansyHuman.UDE.Util.Math;

namespace SansyHuman.UDE.Object
{ 
    public static class UDEBulletExtensions
    {
        public static void MoveBulletToDirection(
            this UDEAbstractBullet bullet,
            UDEBaseCharacter shooter,
            UDEBaseShotPattern originShotPattern,
            Vector2 initialPos,
            float initialRotation,
            Vector2 velocity,
            bool faceToMovingDirection = true)
        {
            if (!bullet.gameObject.activeSelf)
                return;

            UDECartesianMovementBuilder builder = UDECartesianMovementBuilder.Create().Velocity(velocity);
            if (!faceToMovingDirection)
                builder = builder.DoNotFaceToMovingDirection();

            bullet.Initialize(initialPos, initialPos, initialRotation, shooter, originShotPattern, builder.Build());
        }

        public static void RotateBulletAroundCharacter(
            this UDEAbstractBullet bullet,
            UDEBaseCharacter character,
            UDEBaseShotPattern originShotPattern,
            UDEMath.PolarCoord initialPolarCoord,
            float initialRotation,
            float angularSpeed,
            float radialSpeed = 0.0f,
            bool faceToMovingDirection = true)
        {
            if (!bullet.gameObject.activeSelf)
                return;

            UDEPolarMovementBuilder builder = UDEPolarMovementBuilder.Create().AngularSpeed(angularSpeed).RadialSpeed(radialSpeed);
            if (!faceToMovingDirection)
                builder = builder.DoNotFaceToMovingDirection();

            Vector2 origin = character.transform.position;

            bullet.Initialize(origin + (Vector2)initialPolarCoord, origin, initialRotation, character, originShotPattern, builder.Build(), true);
        }
    }
}