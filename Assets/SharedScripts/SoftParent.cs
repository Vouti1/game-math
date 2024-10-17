using UnityEngine;

public class SoftParent : MonoBehaviour
{
    public Transform parent;
    private Vector3 relativePosition;
    private Quaternion relativeRotation;
    private bool isMoving = false;

    void Awake()
    {
        UpdateRelativePosition();
    }

    void LateUpdate()
    {
        if (!isMoving) // Only update position if not being moved by crane controller
        {
            transform.position = parent.TransformPoint(relativePosition);
            transform.rotation = parent.rotation * relativeRotation;
        }
    }

    public void UpdateRelativePosition()
    {
        relativePosition = parent.InverseTransformPoint(transform.position);
        relativeRotation = Quaternion.Inverse(parent.rotation) * transform.rotation;
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
        if (!moving)
        {
            UpdateRelativePosition(); // Update relative position when movement ends
        }
    }
}