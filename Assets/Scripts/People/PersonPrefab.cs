using UnityEngine;

public class PersonPrefab : MonoBehaviour
{
    public Party party;
    public float moveSpeed = 2f;
    public float maxDistance = 2f;
    public float rotationSpeed = 5f; // Speed at which rotation interpolates

    private Vector2 targetPosition;
    private Vector2 randomDirection;
    private SpriteRenderer spriteRenderer;
    private Quaternion targetRotation;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        targetPosition = GetRandomPosition();
    }

    void Update()
    {
        // Move towards the target position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Smoothly rotate towards the direction of movement
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f) // Check if moving
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Get a new random target position
            targetPosition = GetRandomPosition();
        }
    }

    public void SetPartyAffiliation(Party party, Sprite partySprite)
    {
        this.party = party;
        spriteRenderer.sprite = partySprite;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    Vector3 GetRandomPosition()
    {
        // Generate a random position within a maxDistance radius
        randomDirection = Random.insideUnitCircle * maxDistance; // Use insideUnitCircle for 2D
        return randomDirection + (Vector2)transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Consts.RoomTag))
        {
            Vector3 closestPoint = collision.ClosestPoint(transform.position);
            Vector2 oppositeDirection = transform.position - closestPoint;

            // Set the target position based on the opposite direction and move distance
            targetPosition = transform.position + new Vector3(oppositeDirection.x, oppositeDirection.y, 0f) * 5;
            Debug.DrawRay(closestPoint, oppositeDirection, Color.magenta, 1000);
        }
    }
}
