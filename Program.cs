namespace EOCS;

using System.Runtime.Versioning;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using EOCS.Script;

[SupportedOSPlatform("windows")]
class Program
{
    static void Main()
    {
        Config.Init();
        var userGame = new MyGame();
        using var engine = new Main.Main(userGame);
        
        engine.Run();
    }
}