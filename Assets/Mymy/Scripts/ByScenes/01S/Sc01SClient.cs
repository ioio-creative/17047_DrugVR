using DrugVR_Scribe;
using UnityEngine;

public class Sc01SClient : MonoBehaviour
{
    [SerializeField]
    private DrugVR_SceneENUM sceneToGoWhenSide01IsTrue = DrugVR_SceneENUM.Sc01A;
    [SerializeField]
    private DrugVR_SceneENUM sceneToGoWhenSide01IsFalse = DrugVR_SceneENUM.Sc01B;
    private GameManager managerInst;


    /* MonoBehaviour */

    private void Awake()
    {
        managerInst = GameManager.Instance;
    }

    /* end of MonoBehaviour */


    public void GoToSceneOnChoice()
    {
        if (Scribe.Side01)
        {
            managerInst.GoToScene(sceneToGoWhenSide01IsTrue);
        }
        else
        {
            managerInst.GoToScene(sceneToGoWhenSide01IsFalse);
        }
    }
}