using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace DOTSFPS.Components.FPS
{
    public struct FPSCharacterInputControl : IComponentData
    {
        public float2 MoveInput;
        public float2 LookInput;
        public InputEvent Jump, Shoot, Aim, Reload, Sprint, InspectChamber, InspectWeapon, TacticalReload;
    }
}