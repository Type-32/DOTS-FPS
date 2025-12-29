using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.CharacterController;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
public partial class FirstPersonPlayerInputsSystem : SystemBase
{
    private FPSInputActions _inputActions;
    
    protected override void OnCreate()
    {
        _inputActions = new FPSInputActions();
        _inputActions.Enable();
        
        RequireForUpdate<FixedTickSystem.Singleton>();
        RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FirstPersonPlayer, FirstPersonPlayerInputs>().Build());
    }

    protected override void OnUpdate()
    {
        uint tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;
        
        foreach (var (playerInputs, player) in SystemAPI.Query<RefRW<FirstPersonPlayerInputs>, RefRO<FirstPersonPlayer>>())
        {
            // Read as UnityEngine.Vector2
            var moveVector = _inputActions.FPS.Move.ReadValue<Vector2>();
            var lookVector = _inputActions.FPS.Look.ReadValue<Vector2>();

            // Cast to float2
            playerInputs.ValueRW.MoveInput = new float2(moveVector.x, moveVector.y);
            playerInputs.ValueRW.LookInput = new float2(lookVector.x, lookVector.y);

            if (_inputActions.FPS.Jump.WasPerformedThisFrame())
            {
                playerInputs.ValueRW.JumpPressed.Set(tick);
            }
        }
    }
}

/// <summary>
/// Apply inputs that need to be read at a variable rate
/// </summary>
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct FirstPersonPlayerVariableStepControlSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FirstPersonPlayer, FirstPersonPlayerInputs>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerInputs, player) in SystemAPI.Query<RefRO<FirstPersonPlayerInputs>, RefRO<FirstPersonPlayer>>().WithAll<Simulate>())
        {
            if (SystemAPI.HasComponent<FirstPersonCharacterControl>(player.ValueRO.ControlledCharacter))
            {
                FirstPersonCharacterControl characterControl = SystemAPI.GetComponent<FirstPersonCharacterControl>(player.ValueRO.ControlledCharacter);

                characterControl.LookDegreesDelta = playerInputs.ValueRO.LookInput;

                SystemAPI.SetComponent(player.ValueRO.ControlledCharacter, characterControl);
            }
        }
    }
}

/// <summary>
/// Apply inputs that need to be read at a fixed rate.
/// It is necessary to handle this as part of the fixed step group, in case your framerate is lower than the fixed step rate.
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
[BurstCompile]
public partial struct FirstPersonPlayerFixedStepControlSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FixedTickSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<FirstPersonPlayer, FirstPersonPlayerInputs>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        uint tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;

        foreach (var (playerInputs, player) in SystemAPI.Query<RefRO<FirstPersonPlayerInputs>, RefRO<FirstPersonPlayer>>().WithAll<Simulate>())
        {
            if (SystemAPI.HasComponent<FirstPersonCharacterControl>(player.ValueRO.ControlledCharacter))
            {
                FirstPersonCharacterControl characterControl = SystemAPI.GetComponent<FirstPersonCharacterControl>(player.ValueRO.ControlledCharacter);

                quaternion characterRotation = SystemAPI.GetComponent<LocalTransform>(player.ValueRO.ControlledCharacter).Rotation;

                // Move
                float3 characterForward = MathUtilities.GetForwardFromRotation(characterRotation);
                float3 characterRight = MathUtilities.GetRightFromRotation(characterRotation);
                characterControl.MoveVector = (playerInputs.ValueRO.MoveInput.y * characterForward) + (playerInputs.ValueRO.MoveInput.x * characterRight);
                characterControl.MoveVector = MathUtilities.ClampToMaxLength(characterControl.MoveVector, 1f);

                // Jump
                characterControl.Jump = playerInputs.ValueRO.JumpPressed.IsSet(tick);

                SystemAPI.SetComponent(player.ValueRO.ControlledCharacter, characterControl);
            }
        }
    }
}
