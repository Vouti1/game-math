using UnityEngine;

public class SoftParent : MonoBehaviour
{
    public Transform parent;
    private Vector3 relativePosition;
    private Quaternion relativeRotation;

    void Awake()
    {
        UpdateRelativePosition();
    }

    void Update()
    {
        transform.position = parent.TransformPoint(relativePosition);
        transform.rotation = parent.rotation * relativeRotation;
    }

    public void UpdateRelativePosition()
    {
        relativePosition = parent.InverseTransformPoint(transform.position);
        relativeRotation = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }
}
