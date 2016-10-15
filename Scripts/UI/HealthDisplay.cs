using UnityEngine;
using UnityEngine.UI;
using System;
namespace AssemblyCSharp
{
    public class HealthDisplay : MonoBehaviour
    {
        private Text healthNumberText;
        private GameManager gameManager;
        void Awake()
        {
            healthNumberText = transform.GetChild(0).GetComponent<Text>();
        }

        void Start()
        {
            gameManager = GameManager.instance;
            healthNumberText.text = gameManager.PlayerStartingHealth.ToString();
        }

        public void SetDisplayedHealth(float newHealth)
        {
            string toDisplay = Mathf.RoundToInt(newHealth).ToString();
            healthNumberText.text = toDisplay;
        }
    }
}

