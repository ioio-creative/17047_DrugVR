using DrugVR_Scribe;
using UnityEngine;

public class Sc01SClient : MonoBehaviour
{
    private const DrugVR_SceneENUM nextSceneToLoadWhenSide01IsTrue = DrugVR_SceneENUM.Sc01A;
    private const DrugVR_SceneENUM nextSceneToLoadWhenSide01IsFalse = DrugVR_SceneENUM.Sc01B;
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
            managerInst.GoToScene(nextSceneToLoadWhenSide01IsTrue);
        }
        else
        {
            managerInst.GoToScene(nextSceneToLoadWhenSide01IsFalse);
        }
    }
}