using System;
using UnityEngine;
using UnityEngine.UI;

public class RadialSelectionProgressable : SelectionProgressable
{
    // Reference to the image whose fill amount 
    // can be controlled by SelectionProgress script
    [SerializeField]
    private Image m_Selection;


    /* SelectionProgressable interfaces */

    public override float GetValue()
    {
        return m_Selection.fillAmount;
    }

    public override void SetValue(float normedProgessValue)
    {
        m_Selection.fillAmount =
            GenerateSmoothStepFromNormedValue(normedProgessValue);
    }

    /* end of SelectionProgressable interfaces */
}
