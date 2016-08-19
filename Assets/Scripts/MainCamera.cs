using UnityEngine;
using TSM;

public class MainCamera : MonoBehaviour
{
    public Transform playerObject;
    private float originY;
    private float margineZ;

    void OnValidate()
    {
        playerObject = FindObjectOfType<Player>().gameObject.transform;
    }

    private void Awake()
    {
        originY = this.gameObject.transform.position.y;
        margineZ = playerObject.position.z - this.gameObject.transform.position.z;

        SoundManager.Instance.SetAudioListenerFollower(this.transform);
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(playerObject.position.x, originY, playerObject.position.z - margineZ);
    }
}