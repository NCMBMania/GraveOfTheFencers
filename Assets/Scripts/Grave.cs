using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshObstacle))]
public class Grave : MonoBehaviour
{
    Transform playerTransform;
    UI_InGame inGameUIManager;
    public Transform graveMessagePoint;

    public GraveInfo graveInfo;

    void Start()
    {
        playerTransform = Player.Instance.gameObject.transform;
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
