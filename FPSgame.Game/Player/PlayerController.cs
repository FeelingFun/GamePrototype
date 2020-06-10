// Copyright (c) Stride contributors (https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Physics;
using Stride.Input;

namespace FPSgame.Player
{
    public class PlayerController : SyncScript
    {
        [Display("Run Speed")]
        public float MaxRunSpeed { get; set; } = 5;
        
        private Vector3 aerialVelocity = Vector3.Zero;

        public static readonly EventKey<float> RunSpeedEventKey = new EventKey<float>();

        // This component is the physics representation of a controllable character
        private CharacterComponent character;

        private readonly EventReceiver<Vector3> moveDirectionEvent = new EventReceiver<Vector3>(PlayerInput.MoveDirectionEventKey);

        /// <summary>
        /// Called when the script is first initialized
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Will search for an CharacterComponent within the same entity as this script
            character = Entity.Get<CharacterComponent>();
            if (character == null) throw new ArgumentException("Please add a CharacterComponent to the entity containing PlayerController!");
        }
        
        /// <summary>
        /// Called on every frame update
        /// </summary>
        public override void Update()
        {
            Move();
        }

        private void Move()
        {
            // Character speed
            moveDirectionEvent.TryReceive(out Vector3 moveDirection);

            if(character.IsGrounded){
                character.SetVelocity(moveDirection * MaxRunSpeed);
                aerialVelocity = moveDirection*MaxRunSpeed;
            }
            else{
                character.SetVelocity(
                    aerialVelocity
                );
                // aerialVelocity *= 0.7f;
            }

            if (Input.IsKeyPressed(Keys.Space))
            {
                var rb = this.Entity.Get<CharacterComponent>();
                if(rb.IsGrounded)
                    rb.Jump(Vector3.UnitY * 10.0f);
            }

            // Broadcast normalized speed
            RunSpeedEventKey.Broadcast(moveDirection.Length());
        }
    }
}
