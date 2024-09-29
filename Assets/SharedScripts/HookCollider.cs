using UnityEngine;

public class HookController : MonoBehaviour
{
    public Transform hook;         // The hook transform (Soft Parented to the cable)
    public Transform concrete;     // The concrete object to attach
    private bool isAttached = false;  // Flag to check if concrete is attached

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the hook collides with the concrete object
        if (collision.gameObject == concrete && !isAttached)
        {
            Debug.Log("Concrete object attached!");
            AttachConcrete(collision.gameObject);
        }
    }

    private void AttachConcrete(GameObject concreteObject)
    {
        // Set the concrete object as a child of the hook
        concreteObject.transform.SetParent(hook);

        // Set the attached flag to true
        isAttached = true;
    }

    private void DetachConcrete()
    {
        if (isAttached)
        {
            // Detach the concrete from the hook
            concrete.SetParent(null);

            // Optionally, restore the concrete's Rigidbody to non-kinematic
            Rigidbody concreteRigidbody = concrete.GetComponent<Rigidbody>();
            if (concreteRigidbody != null)
            {
                concreteRigidbody.isKinematic = false;
            }

            // Reset the attached flag
            isAttached = false;
        }
    }
}
