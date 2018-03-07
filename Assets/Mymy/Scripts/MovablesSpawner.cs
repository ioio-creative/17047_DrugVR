using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablesSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnObjectData
    {
        public GameObject objectPrefab;
        public int spawnNumber;
    }

    public List<SpawnObjectData> spawnObjList= new List<SpawnObjectData>();

    private void Awake()
    {
        foreach (SpawnObjectData spawnObj in spawnObjList)
        {
            GameObject[] spawnObjArr = new GameObject[spawnObj.spawnNumber];

            for (int i = 0; i < spawnObj.spawnNumber; i++)
            {
                spawnObjArr[i] = Instantiate(spawnObj.objectPrefab, new Vector3(-60, -45 + i*5 , 50), new Quaternion());
            }
        }
    }
}
