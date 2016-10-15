using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class EnemyParameters
{

    public float MaxSpeedChasing = 5;
    public float MaxSpeedFighting = 7;
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;
    public bool IsHostile = true;
    public bool ChaseUntilDead = false;
    public bool DamageOnContact = true;
    public float Damage = 5;
    public float ContactDamageCooldown = 0.1f;
    public float AggroTimeAfterShot = 5;
}
