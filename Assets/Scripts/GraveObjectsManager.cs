using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraveObjectsManager : SingletonMonoBehaviour<GraveObjectsManager>
{
    public GameObject gravePrefab;

    public int graveMaxNum = 10;
    public List<GameObject> graveInstanceList = new List<GameObject>();
    private GameObject tempGraveInstance;//Player死亡時に一時的に設置する墓//

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        tempGraveInstance = GenerateDisabledGameObject(gravePrefab, "TempGrave");
        graveInstanceList = InstantiateObjectsPool(gravePrefab, graveMaxNum);
    }

    private void InstallationGrave(GraveInfo graveInfo)
    {
        GameObject graveObject = graveInstanceList.FirstOrDefault(obj => obj.activeSelf == false);

        if (graveObject == null)
        {
            Debug.Log("grave max num");
        }
        else
        {
            graveObject.transform.parent = this.gameObject.transform;
            graveObject.transform.position = graveInfo.position;

            graveObject.GetComponent<Grave>().graveInfo = graveInfo;
            graveObject.SetActive(true);
        }
    }

    public void InstallationGraves(List<GraveInfo> graveInfoList)
    {
        graveInfoList.ForEach(graveInfo => InstallationGrave(graveInfo));
    }

    public void InstallationTempGrave(Vector3 position)
    {
        tempGraveInstance.transform.position = position;
        tempGraveInstance.SetActive(true);
    }

    public void DisableAllGraves()
    {
        graveInstanceList.ForEach(g => g.SetActive(false));
        tempGraveInstance.SetActive(false);
    }

    private List<GameObject> InstantiateObjectsPool(GameObject prefab, int num)
    {
        List<GameObject> gameObjectList = new List<GameObject>();

        for (int i = 0; i < num; i++)
        {
            gameObjectList.Add(GenerateDisabledGameObject(prefab, prefab.name + i));
        }

        return gameObjectList;
    }

    private GameObject GenerateDisabledGameObject(GameObject prefab, string name)
    {
        GameObject obj = Instantiate(prefab) as GameObject;
        obj.name = name;
        obj.transform.parent = this.gameObject.transform;
        obj.SetActive(false);
        return obj;
    }
}