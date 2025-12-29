using DOTSFPS.Components.FPS;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace DOTSFPS.Systems.FPS
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class FPSInputHarvestingSystem : SystemBase
    {
        private FPSInputActions _inputActions;
        
        protected override void OnCreate()
        {
            _inputActions = new FPSInputActions();
            _inputActions.Enable();
            
            RequireForUpdate<NetworkId>();
        }

        protected override void OnUpdate()
        {
            foreach (var playerInput in SystemAPI.Query<RefRW<FPSCharacterInputControl>>().WithAll<GhostOwnerIsLocal>())
            {
                var moveVector = _inputActions.FPS.Move.ReadValue<Vector2>();
                var lookVector = _inputActions.FPS.Look.ReadValue<Vector2>();

                // Cast to float2
                playerInput.ValueRW.MoveInput = new float2(moveVector.x, moveVector.y);
                playerInput.ValueRW.LookInput = new float2(lookVector.x, lookVector.y);
                
                // Use WasPerformedThisFrame() for things that fire once (Jumps, Reloads)
                if (_inputActions.FPS.Jump.WasPerformedThisFrame())
                    playerInput.ValueRW.Jump.Set();

                if (_inputActions.FPS.Reload.WasPerformedThisFrame())
                    playerInput.ValueRW.Reload.Set();

                if (_inputActions.FPS.InspectChamber.WasPerformedThisFrame())
                    playerInput.ValueRW.InspectChamber.Set();

                if (_inputActions.FPS.TacticalReload.WasPerformedThisFrame())
                    playerInput.ValueRW.TacticalReload.Set();
                
                if (_inputActions.FPS.InspectWeapon.WasPerformedThisFrame())
                    playerInput.ValueRW.InspectWeapon.Set();

                // -- States (Held down) --
                // Use IsInProgress() or IsPressed() for things you hold (Sprint, Aim, Auto-fire)
                if (_inputActions.FPS.Sprint.IsInProgress())
                    playerInput.ValueRW.Sprint.Set();

                if (_inputActions.FPS.Aim.IsInProgress())
                    playerInput.ValueRW.Aim.Set();

                if (_inputActions.FPS.Shoot.IsInProgress()) 
                    playerInput.ValueRW.Shoot.Set();
            }

            foreach (var charView in SystemAPI.Query<RefRW<FPSCharacterView>>())
            {
                charView.ValueRW
            }
        }

        protected override void OnDestroy()
        {
            _inputActions?.Disable();
            _inputActions?.Dispose();
        }
    }
}