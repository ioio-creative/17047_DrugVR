using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    [SerializeField]
    private float m_EulerYOffset;

    private void Start()
    {
        // offset scene rotation + a fixed offset
        float sceneRotation = GameManager.Instance.CurrentSceneScroll.SkyShaderDefaultRotation;
        Quaternion rotationAlongY = Quaternion.Euler(0, sceneRotation + m_EulerYOffset, 0);

        transform.rotation = rotationAlongY * transform.rotation;
    }
}
