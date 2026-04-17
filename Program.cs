using EOCS;
using System.Runtime.Versioning;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
namespace EOCS;

[SupportedOSPlatform("windows")]
class Program{
    static void Main()
    {
        var nativeSettings = new NativeWindowSettings(){
            ClientSize = new Vector2i(1200, 900),
            Title = "EOCS (Engine On CSharp)",
            APIVersion = new Version(3, 3)
        };

        var gameSettings = new GameWindowSettings(){
            // UpdateFrequency = 60.0,   
        };

        using var game = new Main(gameSettings, nativeSettings);
        game.Run();
    }
}
