using UnityEngine;
using System.Collections;

public class CraneController2 : MonoBehaviour
{
    public Transform crane;
    public Transform trolley;
    public Transform concrete;
    public Transform cable;
    public Transform hook;
    private SoftParent trolleySoftParent;
    public HookController hookController;
    private Camera mainCamera;

    public float rotationSpeed = 5f;
    public float trolleySpeed = 1f;
    public float liftSpeed = 1f;
    public float cableSpeed = 2f;
    public float minDistance = 0.1f;
    
    // Limits and settings
    public float minTrolleyPosition = -22f;
    public float maxTrolleyPosition = 10f;
    public float maxCableLength = 20f;
    public float minCableLength = 1f;
    
    private bool isSequencePlaying = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        trolleySoftParent = trolley.GetComponent<SoftParent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == concrete && !isSequencePlaying)
            {
                StartCoroutine(CraneSequence());
            }
        }
    }

    private IEnumerator CraneSequence()
    {
        isSequencePlaying = true;
        //Rotate crane
        yield return StartCoroutine(RotateCrane());
        
        //Move trolley
        yield return StartCoroutine(MoveTrolleyToPosition());
        
        //Lower cable
        yield return StartCoroutine(LowerCable());
        
        //Lift concrete
        yield return StartCoroutine(LiftConcrete());
    }

    private IEnumerator RotateCrane()
    {
        while (true)
        {
            // Calculate direction to concrete
            Vector3 direction = concrete.position - crane.position;
            direction.y = 0; // Keep rotation only in horizontal plane
            
            // Calculate the angle and subtract 90 degrees to compensate for the model's orientation
            float targetAngle = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg) + 90f;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            
            // Check if we're close enough to target rotation
            if (Quaternion.Angle(crane.rotation, targetRotation) < 0.1f)
                break;
            
            crane.rotation = Quaternion.RotateTowards(crane.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator MoveTrolleyToPosition()
    {
        trolleySoftParent.SetMoving(true);

        while (true)
        {
            Vector3 targetPosition = new Vector3(concrete.position.x, trolley.position.y, concrete.position.z);
            
            // Check if we're close enough to target position
            if (Vector3.Distance(trolley.position, targetPosition) < minDistance)
                break;
            
            trolley.position = Vector3.MoveTowards(trolley.position, targetPosition, trolleySpeed * Time.deltaTime);
            yield return null;
        }

        trolleySoftParent.SetMoving(false);
        trolleySoftParent.UpdateRelativePosition();
    }

    private IEnumerator LowerCable()
    {
        float targetLength = Vector3.Distance(trolley.position, concrete.position);
        Vector3 initialScale = cable.localScale;
        float currentLength = cable.localScale.y;

        while (!hookController.isConcreteAttached)
        {         
            currentLength = Mathf.MoveTowards(currentLength, targetLength, cableSpeed * Time.deltaTime);
            cable.localScale = new Vector3(initialScale.x, currentLength, initialScale.z);
            
            // Update hook position
            Vector3 newHookPosition = trolley.position - Vector3.up * currentLength;
            hook.position = newHookPosition;
            
            yield return null;
        }
    }
    private IEnumerator LiftConcrete()
    {
        yield return new WaitForSeconds(1f);

        // Lift to minimum cable length
        Vector3 initialScale = cable.localScale;
        
        while (cable.localScale.y > minCableLength)
        {
            float newLength = Mathf.MoveTowards(cable.localScale.y, minCableLength, liftSpeed * Time.deltaTime);
            cable.localScale = new Vector3(initialScale.x, newLength, initialScale.z);
            
            // Update hook position
            hook.position = trolley.position - Vector3.up * newLength;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Move concrete to new position after wait
        MoveConcrete();
    }

    private void MoveConcrete()
    {
        if (concrete != null)
        {    
            // Get and disable SoftParent from the concrete GameObject
            SoftParent softParent = concrete.GetComponent<SoftParent>();
            softParent.enabled = false;
            hookController.isConcreteAttached = false;

            // Calculate random angle around the crane (in radians)
            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            
            // Calculate random distance from crane within trolley limits
            float randomDistance = Random.Range(Mathf.Abs(minTrolleyPosition), Mathf.Abs(maxTrolleyPosition));
            
            // Calculate position relative to crane's forward direction
            Vector3 craneForward = crane.forward;
            Vector3 craneRight = crane.right;
            
            // Calculate offset from crane position
            Vector3 offset = (craneForward * Mathf.Cos(randomAngle) + craneRight * Mathf.Sin(randomAngle)) * randomDistance;
            
            // Random height
            float randomY = Random.Range(10f, 20f);
            
            // Calculate final position
            Vector3 newPosition = crane.position + offset;
            newPosition.y = randomY;
            
            // Instantly move concrete to new position
            concrete.position = newPosition;
            
            // Reset states
            isSequencePlaying = false;
        }
    }
}