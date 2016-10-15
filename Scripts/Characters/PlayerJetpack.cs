using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class JetpackParameters
{
    //if player keeps holding space after jumping how long before we start jetpacking
    public float HoldSpaceJetpackDelay = 0.1f;
    public float JetpackStrength = 10.0f;
    public float JetpackMaxFue = 100;
    public float JetpackDrain = 25;
    public bool DisableGravity = true;
    //this is used when we start using jetpack while falling down (to help us get more control)
    public float AdditionalAcceleration = 0.1f;
}

//logic for jetpack is going to be calculated here. Input is still read in main Player script.
public partial class Player : MonoBehaviour
{
    private bool isUsingJetpack = false;
    [SerializeField]
    JetpackParameters DefaultJetpackParameters;
    private void JetpackUpdate()
    {
        if (isUsingJetpack)
        {
            Vector2 ForceToAdd = new Vector2(0, DefaultJetpackParameters.JetpackStrength * Time.deltaTime);
            if (_controller.Velocity.y < 0)
            {
                ForceToAdd += new Vector2(0, DefaultJetpackParameters.AdditionalAcceleration * _controller.Velocity.y * (-1) * Time.deltaTime);
            }
            _controller.AddForce(ForceToAdd);
        }
    }

    private void StartJetpack()
    {
        isUsingJetpack = true;
        if (DefaultJetpackParameters.DisableGravity)
        {
            _controller.GravityEnabled = false;
        }
    }

    private void StopJetpack()
    {
        isUsingJetpack = false;
        if (DefaultJetpackParameters.DisableGravity)
        {
            _controller.GravityEnabled = true;
        }
    }
}
