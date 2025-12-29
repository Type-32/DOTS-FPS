using Unity.Entities;

namespace DOTSFPS.Components.Config
{
    public struct FPSCharacterConfig : IComponentData
    {
        public float WalkSpeed;
        public float SprintSpeed;
        public float JumpSpeed;
        public float RotationSpeed;
        public float Gravity;
    }
}