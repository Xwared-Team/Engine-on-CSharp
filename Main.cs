namespace EOCS.Main;

[SupportedOSPlatform("windows")]
public class Main : GameWindow
{
    public static bool debug_mod = false;

    private readonly BaseGame _userGame;

    Matrix4 _model, _view, _projection;
    bool debug_menu = false;

    bool _GamePaused = false;
    bool _GameFullscreen = false;

    float _initialFov = MathHelper.PiOver4;
    private TextRenderer? _textRenderer;
    private Camera? _activeCameraRef; 

    public Main(BaseGame userGame, GameWindowSettings gSettings, NativeWindowSettings nSettings) 
        : base(gSettings, nSettings)
    {
        _userGame = userGame;
    }

    protected override void OnLoad()
    {
        base.OnLoad();     
        Context.SwapInterval = 1;
        CursorState = CursorState.Grabbed;
        if (_GameFullscreen) WindowState = WindowState.Fullscreen;

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend); 
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _textRenderer = new TextRenderer(
            "./Assets/fonts/VCR-OSD-MONO.ttf",
            32,
            "./Assets/shaders/text/shader.vert",
            "./Assets/shaders/text/shader.frag"
        );

        _model = Matrix4.Identity;
        _projection = Matrix4.CreatePerspectiveFieldOfView(_initialFov, Size.X / (float)Size.Y, 0.1f, 1000.0f);

        _userGame.Load(_projection);

        _activeCameraRef = _userGame.ActiveCamera;
        
        if (_activeCameraRef == null)
        {
            _activeCameraRef = new Camera(new Vector3(0, 5, 10), -90.0f, 0.0f);
            _userGame.ActiveCamera = _activeCameraRef;
        }

        _view = _activeCameraRef.GetViewMatrix();
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        if (CursorState != CursorState.Grabbed) return;

        if (_activeCameraRef != null)
        {
            _activeCameraRef.ProcessMouseMovement(e.DeltaX, e.DeltaY);
            _view = _activeCameraRef.GetViewMatrix();
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        var input = KeyboardState;

        if (input.IsKeyReleased(Keys.Escape)){
            _GamePaused = !_GamePaused;
            CursorState = _GamePaused ? CursorState.Normal : CursorState.Grabbed;
        }
        if (input.IsKeyReleased(Keys.F11)){
            _GameFullscreen = !_GameFullscreen;
            WindowState = _GameFullscreen ? WindowState.Fullscreen : WindowState.Normal;
        }
        if (input.IsKeyReleased(Keys.F3)) debug_menu = !debug_menu;

        _userGame.Update(input, (float)e.Time);

        if (_userGame.ActiveCamera != null)
        {
            _activeCameraRef = _userGame.ActiveCamera;
            _view = _activeCameraRef.GetViewMatrix();
        }

        _model = Matrix4.Identity;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {   
        base.OnRenderFrame(e);
        GL.ClearColor(0, 0, 0, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _userGame.Draw(_projection);

        if (_textRenderer != null && debug_menu && _activeCameraRef != null)
        {
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1, 1);
            
            _textRenderer.DrawString("EOCS V0.2.0", 10, 7, 0.7f, ortho, Colors.White, 1f);
            _textRenderer.DrawString($"FPS: {1.0 / e.Time:F1}", 10, 63, 0.7f, ortho, Colors.White, 1f); 
            
            string posText = string.Format(CultureInfo.InvariantCulture, 
                "Pos: {0:F1} {1:F1} {2:F1} | FOV: {3}", 

                _activeCameraRef.Position.X, _activeCameraRef.Position.Y, _activeCameraRef.Position.Z, 
                MathHelper.RadiansToDegrees(_activeCameraRef!.FOV));
            _textRenderer.DrawString(posText, 10, 91, 0.7f, ortho, Colors.White, 1f);

            string dirText = string.Format(CultureInfo.InvariantCulture, 
                "Dir: {0:F1} {1:F1} {2:F1}", 
                _activeCameraRef.Front.X, _activeCameraRef.Front.Y, _activeCameraRef.Front.Z);
            _textRenderer.DrawString(dirText, 10, 119, 0.7f, ortho, Colors.White, 1f); 

        }

        Context.SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        
        float currentFov = _activeCameraRef?.FOV ?? _initialFov;
        _projection = Matrix4.CreatePerspectiveFieldOfView(currentFov, Size.X / (float)Size.Y, 0.1f, 1000.0f);
    }

    protected override void OnUnload()
    {
        _textRenderer?.Dispose();
        base.OnUnload();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (_activeCameraRef != null)
        {
            _activeCameraRef.ProcessMouseScroll(e.OffsetY);
            _projection = Matrix4.CreatePerspectiveFieldOfView(_activeCameraRef.FOV, Size.X / (float)Size.Y, 0.1f, 100.0f);
        }
    }
}