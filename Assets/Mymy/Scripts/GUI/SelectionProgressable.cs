using UnityEngine;

// note: have to inherit MonoBehaviout for 
// GameObject.GetComponent<SelectionProgressable>() to work
public abstract class SelectionProgressable : MonoBehaviour
{
    private const float MinValue = 0f;
    private const float MaxValue = 1f;


    // 0 <= normedProgessValue <= 1
    public abstract void SetValue(float normedProgessValue);

    public void SetValueToMin()
    {
        SetValue(MinValue);
    }

    public void SetValueToMax()
    {
        SetValue(MaxValue);
    }


    protected float GenerateSmoothStepFromNormedValue(float normedValue)
    {
        return Mathf.SmoothStep(MinValue, MaxValue, normedValue);
    }
}
