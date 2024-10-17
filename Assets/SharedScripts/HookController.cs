using UnityEngine;

public class HookController : MonoBehaviour
{
    public GameObject concrete;
    public bool isConcreteAttached;
    public CraneController2 craneController; 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == concrete)
        {
            AttachConcrete(collision.gameObject);
            Debug.Log("Hook hit the concrete");
        }
    }
    private void AttachConcrete(GameObject concrete)
    {
        isConcreteAttached = true;
        // Remove existing SoftParent component if it exists
        SoftParent existingSoftParent = concrete.GetComponent<SoftParent>();
        Destroy(existingSoftParent);
        // Add SoftParent component to the concrete object
        SoftParent softParent = concrete.AddComponent<SoftParent>();
        softParent.enabled = true;
        softParent.parent = this.transform;
        softParent.UpdateRelativePosition();
    }
}