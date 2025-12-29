using System;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSFPS.Components.FPS
{
    [Serializable]
    public struct FPSCharacterComponent : IComponentData
    {
        public float WalkSpeed;
        public float SprintSpeed;
        public float JumpSpeed;
        public float RotationSpeed;
        public float GroundedMovementSharpness;
        public float AirAcceleration;
        public float AirMaxSpeed;
        public float AirDrag;
        public float3 Gravity;
        public bool PreventAirAccelerationAgainstUngroundedHits;
        public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling;

        public float MinViewAngle;
        public float MaxViewAngle;

        public Entity ViewEntity;
        public float ViewPitchDegrees;
        public quaternion ViewLocalRotation;

    }
}