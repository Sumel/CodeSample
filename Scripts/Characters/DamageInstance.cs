
using System;
using UnityEngine;
namespace AssemblyCSharp
{
    public class DamageInstance
    {
        public float Damage;
        public GameObject Dealer;
        public bool TriggersKnockback;
        public bool TriggersInvulnerability;
        public bool IsProjectile;
        public GameObject ProjectileOwner;
        public float KnockBackStrength;
        public Action<Player> SpecialKnockbackBehaviour;
        private DamageInstance(float Damage, GameObject Dealer, bool TriggersKnockback, float KnockBackStrength, bool TriggersInvulnerability, Action<Player> SpecialKnockbackBehaviour, bool IsProjectile, GameObject ProjectileOwner)
        {
            this.Damage = Damage;
            this.Dealer = Dealer;
            this.TriggersKnockback = TriggersKnockback;
            this.KnockBackStrength = KnockBackStrength;
            this.TriggersInvulnerability = TriggersInvulnerability;
            this.IsProjectile = IsProjectile;
            this.ProjectileOwner = ProjectileOwner;
            this.SpecialKnockbackBehaviour = SpecialKnockbackBehaviour;
        }

        public DamageInstance(float Damage, GameObject Dealer, GameObject ProjectileOwner, bool TriggersKnockback = true, float KnockbackStrength = 10, bool TriggersInvulnerability = true)
        : this(Damage, Dealer, TriggersKnockback, KnockbackStrength, TriggersInvulnerability, null, true, ProjectileOwner)
        { }

        //normally these 2 would be one constructor with default parameters but unity currently doesnt support it
        public DamageInstance(float Damage, GameObject Dealer, bool TriggersKnockback, float KnockbackStrength, bool TriggersInvulnerability = true)
        : this(Damage, Dealer, TriggersKnockback, KnockbackStrength, TriggersInvulnerability, null, false, null)
        { }

        public DamageInstance(float Damage, GameObject Dealer)
        : this(Damage, Dealer, true, 10)
        { }

        public DamageInstance(float Damage, GameObject Dealer, Action<Player> SpecialKnockbackBehaviour, bool TriggersInvulnerability = true)
        : this(Damage, Dealer, true, 0, TriggersInvulnerability, SpecialKnockbackBehaviour, false, null)
        { }

        public DamageInstance(float Damage, GameObject Dealer, Action<Player> SpecialKnockbackBehaviour, GameObject ProjectileOwner, bool TriggersInvulnerability = true)
        : this(Damage, Dealer, true, 0, TriggersInvulnerability, SpecialKnockbackBehaviour, true, ProjectileOwner)
        { }
    }
}

