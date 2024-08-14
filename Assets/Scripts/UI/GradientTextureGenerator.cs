using UnityEngine;

public static class GradientTextureGenerator
{
    public static Texture2D GenerateGradientTexture(Vector2 size, Color startColor, Color endColor, float threshold, bool reverseDir)
    {
        Vector2Int pixelVector = size.ToVector2Int();

        int width = pixelVector.x;
        int height = pixelVector.y;

        // Ensure the threshold is within the range [0, 1]
        threshold = Mathf.Clamp01(threshold);

        Texture2D texture = new Texture2D(width, height);

        // Calculate the x position where the gradient starts
        int thresholdX = Mathf.RoundToInt(threshold * width);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color;
                if (x < thresholdX)
                {
                    // Before the threshold, use the start color
                    color = startColor;
                }
                else
                {
                    // After the threshold, calculate the lerp value
                    float t = (float)(x - thresholdX) / (width - thresholdX - 1);
                    color = Color.Lerp(startColor, endColor, t);
                }

                //texture.SetPixel(width - x - 1, y, color);
                if (reverseDir)
                    texture.SetPixel(width - x - 1, y, color);
                else
                    texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

}
