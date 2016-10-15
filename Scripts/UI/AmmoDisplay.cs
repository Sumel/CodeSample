using UnityEngine;
using UnityEngine.UI;
using System;
namespace AssemblyCSharp
{
    public class AmmoDisplay : MonoBehaviour
    {
        private Text ammoText;
        private GameManager gameManager;
        void Awake()
        {
            ammoText = transform.GetChild(0).GetComponent<Text>();
        }

        void Start()
        {
            gameManager = GameManager.instance;
            ammoText.text = gameManager.PlayerStartingHealth.ToString();
        }

        public void SetDisplayedAmmo(float newAmmo)
        {
            string toDisplay = Mathf.RoundToInt(newAmmo).ToString();
            ammoText.text = toDisplay;
        }
    }
}

