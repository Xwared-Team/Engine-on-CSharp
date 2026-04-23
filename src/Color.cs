namespace EOCS.Utils.Colors;

public static class Colors
{
    public static readonly Vector3 Black = new(0.0f, 0.0f, 0.0f);               // #000000
    public static readonly Vector3 White = new(1.0f, 1.0f, 1.0f);               // #FFFFFF
    public static readonly Vector3 Grey = new(0.5f, 0.5f, 0.5f);                // #808080
    public static readonly Vector3 LightGrey = new(0.75f, 0.75f, 0.75f);        // #C0C0C0
    public static readonly Vector3 DarkGrey = new(0.25f, 0.25f, 0.25f);         // #404040
    public static readonly Vector3 Silver = new(0.75f, 0.75f, 0.75f);           // Alias for LightGrey often

    public static readonly Vector3 Red = new(1.0f, 0.0f, 0.0f);                 // #FF0000
    public static readonly Vector3 DarkRed = new(0.55f, 0.0f, 0.0f);            // #8B0000
    public static readonly Vector3 Crimson = new(0.86f, 0.08f, 0.24f);          // #DC143C
    public static readonly Vector3 IndianRed = new(0.8f, 0.36f, 0.36f);         // #CD5C5C


    public static readonly Vector3 Green = new(0.0f, 1.0f, 0.0f);               // #00FF00
    public static readonly Vector3 Lime = new(0.0f, 1.0f, 0.0f);                // #00FF00
    public static readonly Vector3 DarkGreen = new(0.0f, 0.39f, 0.0f);          // #006400
    public static readonly Vector3 ForestGreen = new(0.13f, 0.55f, 0.13f);      // #228B22
    public static readonly Vector3 Olive = new(0.5f, 0.5f, 0.0f);               // #808000

    public static readonly Vector3 Blue = new(0.0f, 0.0f, 1.0f);                // #0000FF
    public static readonly Vector3 DarkBlue = new(0.0f, 0.0f, 0.55f);           // #00008B
    public static readonly Vector3 Navy = new(0.0f, 0.0f, 0.5f);                // #000080
    public static readonly Vector3 SkyBlue = new(0.53f, 0.81f, 0.92f);          // #87CEEB
    public static readonly Vector3 CornflowerBlue = new(0.39f, 0.58f, 0.93f);   // #6495ED

    public static readonly Vector3 Yellow = new(1.0f, 1.0f, 0.0f);              // #FFFF00
    public static readonly Vector3 Gold = new(1.0f, 0.84f, 0.0f);               // #FFD700
    public static readonly Vector3 Orange = new(1.0f, 0.65f, 0.0f);             // #FFA500
    public static readonly Vector3 OrangeRed = new(1.0f, 0.27f, 0.0f);          // #FF4500

    public static readonly Vector3 Purple = new(0.5f, 0.0f, 0.5f);              // #800080
    public static readonly Vector3 Violet = new(0.93f, 0.51f, 0.93f);           // #EE82EE
    public static readonly Vector3 Magenta = new(1.0f, 0.0f, 1.0f);             // #FF00FF
    public static readonly Vector3 Indigo = new(0.29f, 0.0f, 0.51f);            // #4B0082

    public static readonly Vector3 Cyan = new(0.0f, 1.0f, 1.0f);                // #00FFFF
    public static readonly Vector3 Aqua = new(0.0f, 1.0f, 1.0f);                // #00FFFF
    public static readonly Vector3 Teal = new(0.0f, 0.5f, 0.5f);                // #008080
    public static readonly Vector3 Turquoise = new(0.25f, 0.88f, 0.82f);        // #40E0D0

    public static readonly Vector3 Brown = new(0.65f, 0.16f, 0.16f);            // #A52A2A
    public static readonly Vector3 Beige = new(0.96f, 0.96f, 0.86f);            // #F5F5DC
    public static readonly Vector3 Chocolate = new(0.82f, 0.41f, 0.12f);        // #D2691E

    public static readonly Vector3 Zero = new(0.0f, 0.0f, 0.0f);
    
    /// <summary>
    /// Converts RGB code to normalized (0 - 1)
    /// </summary>
    public static Vector3 NormalizeRGB(byte r, byte g, byte b)
    {
        return new Vector3(r / 255f, g / 255f, b / 255f);
    }
}