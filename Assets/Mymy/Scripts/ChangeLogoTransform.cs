using UnityEngine;

public class ChangeLogoTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3 logoContainerPosition;

    private Transform logoContainerTransform;    

	private void Start()
    {
        logoContainerTransform = GameManager.Instance.LogoContainerObject.transform;
        logoContainerTransform.position = logoContainerPosition;
    }		
}
