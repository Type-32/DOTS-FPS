using DOTSFPS.Components.FPS;
using DOTSFPS.Components.Tags;
using Unity.CharacterController;
using Unity.Entities;
using UnityEngine;

namespace DOTSFPS.Authoring
{
    public class FPSCharacterAuthoring : MonoBehaviour
    {
        [Header("Movement Stats")]
        public float WalkSpeed = 5f;
        public float SprintSpeed = 10f;
        public float JumpSpeed = 8f;
        public float RotationSpeed = 15f;

        [Header("Character Physics")]
        // This struct comes from the CharacterController package. 
        // It exposes Capsule size, Step handling, Max Slope, etc. in the Inspector.
        public AuthoringKinematicCharacterProperties CharacterProperties = AuthoringKinematicCharacterProperties.GetDefault();

        public class Baker : Baker<FPSCharacterAuthoring>
        {
            public override void Bake(FPSCharacterAuthoring authoring)
            {
                // 1. Bake the Kinematic Character (Capsule, Physics Body, Mass)
                // This magic utility function from the package handles the heavy lifting
                // of setting up physics for a controller.
                KinematicCharacterUtilities.BakeCharacter(this, authoring, authoring.CharacterProperties);

                Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

                // 2. Add our Custom Components
                AddComponent(entity, new FPSCharacterTag());
                
                AddComponent(entity, new FPSCharacterComponent
                {
                    WalkSpeed = authoring.WalkSpeed,
                    SprintSpeed = authoring.SprintSpeed,
                    JumpSpeed = authoring.JumpSpeed,
                    RotationSpeed = authoring.RotationSpeed
                });

                // 3. Add the Input Component
                // We add it here so the entity HAS the slot for data. 
                // It starts empty, and our Harvesting System will fill it later.
                AddComponent<FPSCharacterInputControl>(entity);
            }
        }
    }
}