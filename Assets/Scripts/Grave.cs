using UnityEngine;

#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshObstacle))]
public class Grave : MonoBehaviour
{
    public Transform playerTransform;
    UI_InGame inGameUIManager;
    public Transform graveMessagePoint;

    public GraveInfo graveInfo;

    void Awake()
    {
        playerTransform = FindObjectOfType<Player>().gameObject.transform;
        inGameUIManager = UI_InGame.Instance;
    }

    
    void Update()
    {
        if(graveInfo.isUsed == false &&
            Vector3.Distance(playerTransform.position, this.transform.position) < 2f)
        {
            inGameUIManager.SetGraveName(graveMessagePoint, this);
        }
    }
}
