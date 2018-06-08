using UnityEngine;

public class ChangeLogoTransform : MonoBehaviour
{
    [SerializeField]
    private float logoContainerScaleFactor = 1f;

    private Vector3 logoContainerOriginalScale;
    private Transform logoContainerTransform;

    private void OnEnable()
    {
        logoContainerTransform = GameManager.Instance.LogoContainerObject.transform;
        logoContainerOriginalScale = logoContainerTransform.localScale;
    }

    private void OnDisable()
    {
        logoContainerTransform.localScale = logoContainerOriginalScale;
    }

    private void Start()
    {
        logoContainerTransform.localScale = logoContainerOriginalScale * logoContainerScaleFactor;
    }	
}
