using UnityEngine;

public class SyncObject : MonoBehaviour
{
    [HideInInspector]
    public int objectId;
    public int objectType;
    
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Quaternion rotation;
    [HideInInspector]
    public Vector3 velocity;
    
    [SerializeField]
    private const float lerpRate = 15f; // 補間の速度
    private Rigidbody rb;
    private Vector3 lastPosition;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
    }
    
    void Update()
    {
        // リモートプレイヤーの場合、位置を補間
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpRate);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lerpRate);
    }

    public void CopyFrom(NetworkDataTypes.SyncObjectData data)
    {
        objectId = data.objectId;
        objectType = data.objectType;
        position = data.position;
        rotation = data.rotation;
        velocity = data.velocity;
    }
}
