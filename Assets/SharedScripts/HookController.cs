using UnityEngine;

public class HookController : MonoBehaviour
{
    public GameObject concrete;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == concrete)
        {
            AttachConcrete(collision.gameObject);
        }
    }

    private void AttachConcrete(GameObject concreteObject)
    {
        // Remove existing SoftParent component if it exists
        SoftParent existingSoftParent = concreteObject.GetComponent<SoftParent>();
        if (existingSoftParent != null)
        {
            Destroy(existingSoftParent);
        }

        // Add SoftParent component to the concrete object
        SoftParent softParent = concreteObject.AddComponent<SoftParent>();
        softParent.parent = this.transform;
        softParent.UpdateRelativePosition();
    }
}