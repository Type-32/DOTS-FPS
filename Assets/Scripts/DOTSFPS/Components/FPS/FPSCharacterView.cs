using Unity.Entities;
using Unity.Mathematics;

namespace DOTSFPS.Components.FPS
{
    public struct FPSCharacterView : IComponentData
    {
        public Entity CharacterEntity;
        public float2 LookInput;
    }
}