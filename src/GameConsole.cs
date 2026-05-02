namespace EOCS.GameConsole;

[SupportedOSPlatform("windows")]
public class GameConsole
{
    public bool IsOpen {get; private set;} = false;
    public List<string> History {get; private set;} = new List<string>();
    
    public string Command {get; private set;} = "";
    private bool _wasToggleKeyPressed = false;
    private const string _pathToFileHistory = "./Assets/ConsoleHistory.log";

    Square _background = new Square(
        new Vector2(0, 0),
        new Vector2(200, 150),
        new Vector3(0.0f, 1.0f, 0.0f),
        0.5f,
        0
    );

    public GameConsole(bool isOpen = false)
    {
        History = ReadHistory();
        IsOpen = isOpen;
    }

    public void ProcessInput(KeyboardState input)
    {
        bool isToggleKeyDown = input.IsKeyDown(Keys.GraveAccent);

        if (isToggleKeyDown && !_wasToggleKeyPressed)
        {
            IsOpen = !IsOpen;

            if (IsOpen) Command = "";
        }
        _wasToggleKeyPressed = isToggleKeyDown;

        if (!IsOpen) return;


        if (input.IsKeyReleased(Keys.Enter))
        {
            CommandExecutor(Command);
            WriteHistory(Command);
            Command = "";
        }
    }

    private List<string> ReadHistory()
    {   
        Console.WriteLine(Config.Get<int>("ConsoleHistoryLenght"));

        if (!File.Exists(_pathToFileHistory))
        {
            Console.WriteLine("[Console] cant read history file");
            return new List<string>();
        }

        try
        {
            return new List<string>(File.ReadAllLines(_pathToFileHistory));
        } catch (Exception e) {
            Console.WriteLine($"[Console] cant read history file {e}");
            return new List<string>();
        }
    }
     
    private void WriteHistory(string command)
    {
        try {
            File.AppendAllText(_pathToFileHistory, command + Environment.NewLine);
        } catch (Exception e) {
            Console.WriteLine($"[Console] Error write command history: {e}");
        }
    }   

    public void DrawConsole(int width, int height)
    {
        if (!IsOpen) return;
        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);

        _background.Position = new Vector2(width / 2f, height / 2f);
        _background.Scale = new Vector2(width, height);

        _background.Color = new Vector3(0.0f, 0.0f, 0.0f);
        _background.Alpha = 0.5f;

        _background.Draw(ortho);

        GL.Enable(EnableCap.DepthTest);
    }

    private void CommandExecutor(string command)
    {
        
    }
}