using UnityEngine;

public class ReverseGravityAbility : MonoBehaviour
{
    public KeyCode changeGravityKey = KeyCode.G;

    private Quaternion to;

    // Start is called before the first frame update
    private void Start()
    {
        Physics.gravity = Vector3.down * 9.8f;
        to = transform.rotation;
    }

    private void Update()
    {
        to = transform.rotation;
        if (Input.GetKeyDown(changeGravityKey)) FlipGravity();
    }

    private void FlipGravity()
    {
        // Sets the gravity for the entire scene
        Vector3 gravity = Physics.gravity;
        gravity.y *= -1;
        Physics.gravity = gravity;

        // Rotate the character so you aren't walking on your head
        to = Quaternion.Euler(transform.rotation.eulerAngles.x + 180, transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z);
        transform.rotation = to;
        transform.Rotate(new Vector3(0, 180, 0));
    }
}