using UnityEngine;
using UnityEngine.UI;
using GameMath.UI;

public class CraneController : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public HoldableButton leftButton;
    public HoldableButton rightButton;
    public SoftParent trolleyParent;
    public SoftParent cableParent;
    public Transform trolley;
    public Transform cable;
    public float minTrolleyPosition = -22f;
    public float maxTrolleyPosition = 10f;
    public float minCablePosition = -22f;
    public float maxCablePosition = 10f;
    public Slider trolleySlider;
    public Slider cableSlider;

    private float trolleyDistance; // Distance of trolley from the center

    private void Start()
    {
        trolleySlider.minValue = minTrolleyPosition;
        trolleySlider.maxValue = maxTrolleyPosition;
        trolleySlider.value = trolley.localPosition.x;
        trolleySlider.onValueChanged.AddListener(MoveTrolley);

        cableSlider.minValue = minCablePosition;
        cableSlider.maxValue = maxCablePosition;
        cableSlider.value = cable.localPosition.y;
        cableSlider.onValueChanged.AddListener(MoveCable);

        // Initialize trolley distance
        trolleyDistance = trolley.localPosition.x;
    }

    private void Update()
    {
        if (leftButton.IsHeldDown)
        {
            RotateCounterClockwise();
        }
        else if (rightButton.IsHeldDown)
        {
            RotateClockwise();
        }
    }

    private void RotateClockwise()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void RotateCounterClockwise()
    {
        transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
    }

    public void MoveTrolley(float position)
    {
        trolleyDistance = Mathf.Clamp(position, minTrolleyPosition, maxTrolleyPosition);
        UpdateTrolleyPosition();

        // Update the relative position in the SoftParent script
        trolleyParent.UpdateRelativePosition();
    }

    private void UpdateTrolleyPosition()
    {
        // Calculate the new position based on the crane's rotation
        float angle = -transform.eulerAngles.y * Mathf.Deg2Rad;
        float x = trolleyDistance * Mathf.Cos(angle);
        float z = trolleyDistance * Mathf.Sin(angle);

        // Update the trolley's position
        Vector3 newPosition = new Vector3(x, trolley.localPosition.y, z);
        trolley.localPosition = newPosition;
    }

    public void MoveCable(float position)
    {
        position = Mathf.Clamp(position, minCablePosition, maxCablePosition);
        Vector3 newPosition = cable.localPosition;
        newPosition.y = position;
        cable.localPosition = newPosition;

        // Update the relative position in the SoftParent script
        cableParent.UpdateRelativePosition();
    }
}
