using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    public float fwdSpeed;
    public float sideSpeed;
    public float jumpForce;
    public Transform innerModel;
    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var isJumpPressed = Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1");

        var newVelocity = _rigidbody.velocity;
        
        if (vertical != 0)
        {
            newVelocity.z = vertical * Time.deltaTime * fwdSpeed;
            _rigidbody.velocity = newVelocity;
        }
        if (horizontal != 0)
        {
            newVelocity.x = horizontal * Time.deltaTime * sideSpeed;
            _rigidbody.velocity = newVelocity;
        }

        if (isJumpPressed)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (_rigidbody.velocity.magnitude > 0.01f)
        {
            var velocity = _rigidbody.velocity.normalized;
            innerModel.transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z));
        }
    }
}
