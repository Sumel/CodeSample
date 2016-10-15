using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class PlayerCharacter : PlatformerCharacter2D
    {
        enum FrictionState
        {
            ground,
            infinite,
            air
        }
        private float groundFriction;
        private float airFriction = 0;
        private PhysicsMaterial2D phyMat;
        private float infinity = 999999;
        //so that I dont change when theres no need
        private FrictionState fState = FrictionState.ground;
        void Start()
        {
            phyMat = GetComponent<Collider2D>().sharedMaterial;
            groundFriction = phyMat.friction;
        }
        public override void Move(float move, bool crouch, bool jump)
        {
            if(move==0&&grounded)
            {
                if(fState != FrictionState.infinite)
                {
                    fState = FrictionState.infinite;
                    phyMat.friction = infinity;
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Collider2D>().enabled = true;

                }
            }
            else if(GetComponent<Rigidbody2D>().velocity.y == 0 && !grounded)
            {
                if(fState != FrictionState.air)
                {
                    fState = FrictionState.air;
                    phyMat.friction = airFriction;
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Collider2D>().enabled = true;
                }
            }
            else
            {
                if(fState != FrictionState.ground)
                {
                    fState = FrictionState.ground;
                    phyMat.friction = groundFriction;
                    GetComponent<Collider2D>().enabled = false;
                    GetComponent<Collider2D>().enabled = true;
                }
            }


            base.Move(move,crouch,jump);
        }

        void OnApplicationQuit()
        {
            phyMat.friction = groundFriction;
        }
    }
}

