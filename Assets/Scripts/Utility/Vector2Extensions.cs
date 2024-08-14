using UnityEngine;

public static class Vector2Extensions
{
    /// <summary>
    /// Round off to remove precision error when comparing two vectors
    /// </summary>
    public static Vector2 RoundOff(this Vector2 vector)
    {
        float roundedX = Mathf.Round(vector.x * 1000f) / 1000f;
        float roundedY = Mathf.Round(vector.y * 1000f) / 1000f;
        return new Vector2(roundedX, roundedY);
    }

    /// <summary>
    /// Converts a Vector2 to a Vector2Int by truncating the float values to integers.
    /// </summary>
    /// <param name="vector">The Vector2 to convert.</param>
    /// <returns>A Vector2Int representation of the Vector2.</returns>
    public static Vector2Int ToVector2Int(this Vector2 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }
}
