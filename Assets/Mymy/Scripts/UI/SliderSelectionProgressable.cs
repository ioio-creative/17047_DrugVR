using UnityEngine;
using UnityEngine.UI;

public class SliderSelectionProgressable : SelectionProgressable
{
    // Reference to the slider whose value 
    // can be controlled by SelectionProgress script
    [SerializeField]
    private Slider m_Slider;


    /* SelectionProgressable interfaces */

    public override float GetValue()
    {
        return m_Slider.value;
    }

    public override void SetValue(float normedProgessValue)
    {
        m_Slider.value = 
            GenerateSmoothStepFromNormedValue(normedProgessValue);
    }

    /* end of SelectionProgressable interfaces */
}
