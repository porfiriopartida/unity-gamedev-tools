using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = target.transform.position - offset;
    }
}
