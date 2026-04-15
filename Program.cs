// Game Create By Dov1nt (Xwared Team)
// Game Create By Dov1nt (Xwared Team)
// Game Create By Dov1nt (Xwared Team)
using BPX;
using OpenTK.Windowing.Desktop;
namespace BCX;

class Program{
    [Obsolete]
    static void Main()
    {
        var nativeSettings = NativeWindowSettings.Default;
        nativeSettings.Size = new OpenTK.Mathematics.Vector2i(1200, 900);
        nativeSettings.Title = "RGB triangle";

        var gameSettings = GameWindowSettings.Default;

        using (var game = new Main(gameSettings, nativeSettings))
        {
            game.Run();
        }
    }
}
