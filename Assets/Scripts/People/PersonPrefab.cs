using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonPrefab : MonoBehaviour
{
    public Party party;

    SpriteRenderer spriteRenderer;

    public float moveSpeed = 2f;
    public float maxDistance = 2f;

    private Vector2 targetPosition;
    private Vector2 randomDirection;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
	{
        targetPosition = GetRandomPosition();
    }

	// Update is called once per frame
	void Update()
	{
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Get a new random target position
            targetPosition = GetRandomPosition();
        }
    }

    public void setPartyAffiliation(Party party, Color partyColor)
    {
        this.party = party;
        spriteRenderer.color = partyColor;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    Vector3 GetRandomPosition()
    {
        // Generate a random position within a maxDistance radius
        randomDirection = Random.insideUnitSphere * maxDistance;
        return randomDirection + (Vector2) transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Consts.RoomTag))
        {
            Vector3 closestPoint = collision.ClosestPoint(transform.position);
            Vector2 oppositeDirection = transform.position - closestPoint; ;

            // Set the target position based on the opposite direction and move distance
            targetPosition = transform.position + new Vector3(oppositeDirection.x, oppositeDirection.y, 0f) * 5;
            Debug.DrawRay(closestPoint, oppositeDirection, Color.magenta, 1000);
        }
    }
}

