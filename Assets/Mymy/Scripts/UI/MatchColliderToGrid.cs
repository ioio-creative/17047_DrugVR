using UnityEngine;
using UnityEngine.UI;

public class MatchColliderToGrid : MonoBehaviour
{
    public GridLayoutGroup Grid
    {
        set
        {
            m_GridLayoutGroup = value;
        }
    }

    [SerializeField]
    private GridLayoutGroup m_GridLayoutGroup;

    [SerializeField]
    private BoxCollider m_BoxCollider;

    private void Start()
    {
        Vector3 boxColliderSize = m_BoxCollider.size;
        boxColliderSize.x = m_GridLayoutGroup.cellSize.x;
        boxColliderSize.y = m_GridLayoutGroup.cellSize.y;
        m_BoxCollider.size = boxColliderSize;
    }
}
