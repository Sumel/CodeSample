using UnityEngine;
using System;
namespace AssemblyCSharp
{
    public class WeaponManager : MonoBehaviour
    {
        enum WeaponSlot { Primary, Secondary, Tertiary };

        [SerializeField]
        private GameObject primaryWeapon;
        [SerializeField]
        private GameObject secondaryWeapon;
        [SerializeField]
        private GameObject tertiaryWeapon;
        [SerializeField]
        private WeaponSlot startingWeaponSlot;

        private GameObject chosenWeapon;

        void Start()
        {
            GameManager.DelayedFunction(0, InitWeapons);
        }

        private void InitWeapons()
        {
            primaryWeapon.SetActive(false);
            secondaryWeapon.SetActive(false);
            tertiaryWeapon.SetActive(false);
            switch (startingWeaponSlot)
            {
                case WeaponSlot.Primary:
                    chosenWeapon = primaryWeapon;
                    break;
                case WeaponSlot.Secondary:
                    chosenWeapon = secondaryWeapon;
                    break;
                case WeaponSlot.Tertiary:
                    chosenWeapon = tertiaryWeapon;
                    break;
            }
            chosenWeapon.SetActive(true);
            chosenWeapon.SendMessage("OnSelect");
        }

        void Update()
        {
            if (Input.GetButtonDown("PrimaryWeapon") && primaryWeapon != null)
            {
                tryChoosingWeapon(primaryWeapon);
            }
            else if (Input.GetButtonDown("SecondaryWeapon") && secondaryWeapon != null)
            {
                tryChoosingWeapon(secondaryWeapon);
            }
            else if (Input.GetButtonDown("TertiaryWeapon") && tertiaryWeapon != null)
            {
                tryChoosingWeapon(tertiaryWeapon);
            }
        }

        private void tryChoosingWeapon(GameObject weap)
        {
            if (chosenWeapon == weap)
            {
                return;
            }
            chosenWeapon.SendMessage("OnDeselect");
            chosenWeapon.SetActive(false);
            chosenWeapon = weap;
            weap.SetActive(true);
            weap.SendMessage("OnSelect");
        }

    }
}

