using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;

public class AccuracyDisplay : SizeChanger
{
    [SerializeField]
    private Sprite goodImage = null;
    [SerializeField]
    private Sprite mediumImage = null;
    [SerializeField]
    private Sprite badImage = null;

    private RhythmAccuracy lastAccuracy = RhythmAccuracy.Full;
    private Image imageComponent;
    void Start()
    {
        imageComponent = transform.GetChild(0).GetComponent<Image>();
        base.Start();
    }

    public void CheckAccuracyDisplay(RhythmAccuracy Accuracy)
    {
        if (lastAccuracy != Accuracy)
        {
            lastAccuracy = Accuracy;
            switch (Accuracy)
            {
                case RhythmAccuracy.Full:
                    ChangeSprite(goodImage);
                    break;
                case RhythmAccuracy.Partial:
                    ChangeSprite(mediumImage);
                    break;
                case RhythmAccuracy.Miss:
                    ChangeSprite(badImage);
                    break;
            }
        }
        TryChangingSize();
    }

    private void ChangeSprite(Sprite newSprite)
    {
        imageComponent.overrideSprite = newSprite;
    }
}
