using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QEShotGraph : MonoBehaviour
{
    [SerializeField]
    Color makeAfterColor, makeBeforeColor, missAfterColor, missBeforeColor;
    [SerializeField]
    Image beforeImage, afterImage;

    [SerializeField]
    [Range(0, 1)]
    float beforeDuration, afterDuration;

    [SerializeField]
    bool make = false;

    [SerializeField]
    TMPro.TMP_Text numberText;
    [ContextMenu("Update Graph")]
    void UpdateGraph()
    {
        beforeImage.fillAmount = beforeDuration;
        afterImage.fillAmount = afterDuration;
        if (make)
        {
            beforeImage.color = makeBeforeColor;
            afterImage.color = makeAfterColor;
        }
        else
        {
            beforeImage.color = missBeforeColor;
            afterImage.color = missAfterColor;
        }
    }

    public void SetValues(Vector2 durations,bool new_make,int count)
    {
        beforeDuration = durations[0];
        afterDuration = durations[1];
        numberText.SetText(count.ToString());
        make = new_make;
        UpdateGraph();
    }
}
