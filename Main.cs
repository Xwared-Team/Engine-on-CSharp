// Main.cs
namespace EOCS.Main; // namespace of this file
using System.Runtime.Versioning; // Allow on Windows

// EOCS usings
using EOCS.SkyBox;
using EOCS.TextRender.TextRender;
using EOCS.ObjLoader;
using EOCS.render;

// Usings
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

[SupportedOSPlatform("windows")]
public class Main(GameWindowSettings gSettings, NativeWindowSettings nSettings) : GameWindow(gSettings, nSettings)
{
    public static bool debug_mod = false;
    
    Matrix4 _model, _view, _projection;
    bool debug_menu = false;

    Vector3 _camPos = new Vector3(0, 20, 80);

    float _yaw = -90.0f;
    float _pitch = 0.0f;

    float _moveSpeed = 5.0f;
    float _mouseSensitivy = 0.1f;
    bool _GamePaused = false;
    bool _GameFullscreen = false;
    float FOV = MathHelper.PiOver4;

    // Light
    Vector3 _lightPos = new Vector3(10f, 15f, 10f);

    private Mesh? _teapotMesh;
    private ShaderProgram? _mainShader;
    private Skybox? _skybox;
    private TextRenderer? _textRenderer;

    protected override void OnLoad()
    {
        base.OnLoad();     
        Context.SwapInterval = 1;
        CursorState = CursorState.Grabbed;
        if (_GameFullscreen) WindowState = WindowState.Fullscreen;

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend); // For Text
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Load Models
        ObjModel model = ObjLoader.Load("./Assets/3D_objects/teapol.obj");
        _teapotMesh = new Mesh(model.Vertices, model.Indices);

        // Create Shader
        _mainShader = new ShaderProgram("./Assets/shaders/main/shader.vert", "./Assets/shaders/main/shader.frag");

        // Init another objects
        // Sky Box
        string[] skyboxFaces = 
        {
            "./Assets/SkyBox/right.png",
            "./Assets/SkyBox/left.png",
            "./Assets/SkyBox/top.png",
            "./Assets/SkyBox/bottom.png",
            "./Assets/SkyBox/front.png",
            "./Assets/SkyBox/back.png"
        };
        _skybox = new Skybox(skyboxFaces);

        // Text Settings
        _textRenderer = new TextRenderer(
            "./Assets/fonts/VCR-OSD-MONO.ttf",
            32,
            "./Assets/shaders/text/shader.vert",
            "./Assets/shaders/text/shader.frag"
        );

        // Initial Matrices
        _model = Matrix4.Identity;
        _view = Matrix4.LookAt(new Vector3(0, 0, 0), Vector3.Zero, Vector3.UnitY);
        _projection = Matrix4.CreatePerspectiveFieldOfView(FOV, Size.X / (float)Size.Y, 0.1f, 1000.0f);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (CursorState != CursorState.Grabbed) return;

        float xOffset = e.DeltaX * _mouseSensitivy;
        float yOffset = e.DeltaY * _mouseSensitivy;

        _yaw += xOffset;
        _pitch += yOffset;

        _pitch = MathHelper.Clamp(_pitch, -89.9f, 89.9f);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        var input = KeyboardState;

        float yawRad = MathHelper.DegreesToRadians(_yaw);
        float pitchRad = MathHelper.DegreesToRadians(_pitch);

        Vector3 front;
        front.X = MathF.Cos(yawRad) * MathF.Cos(pitchRad);
        front.Y = -MathF.Sin(pitchRad);
        front.Z = MathF.Sin(yawRad) * MathF.Cos(pitchRad);
        front = Vector3.Normalize(front);

        Vector3 right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        Vector3 up = Vector3.Normalize(Vector3.Cross(right, front));

        float velocity = _moveSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.W)) _camPos += front * velocity;
        if (input.IsKeyDown(Keys.S)) _camPos -= front * velocity;
        if (input.IsKeyDown(Keys.A)) _camPos -= right * velocity;
        if (input.IsKeyDown(Keys.D)) _camPos += right * velocity;

        if (input.IsKeyDown(Keys.Space)) _camPos.Y += velocity;
        if (input.IsKeyDown(Keys.LeftShift)) _camPos.Y -= velocity;

        if (input.IsKeyReleased(Keys.Escape)){
            _GamePaused = !_GamePaused;
            if (_GamePaused) CursorState = CursorState.Normal; else CursorState = CursorState.Grabbed;
        }
        if (input.IsKeyReleased(Keys.F11)){
            _GameFullscreen = !_GameFullscreen;
            if (_GameFullscreen) WindowState = WindowState.Fullscreen; else WindowState = WindowState.Normal;
        }
        if (input.IsKeyReleased(Keys.F3)){
            debug_menu = !debug_menu;
        }
        
        Vector3 target = _camPos + front;
        _view = Matrix4.LookAt(_camPos, target, up);

        _model = Matrix4.CreateScale(0.5f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {   
        // New Frame
        base.OnRenderFrame(e);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Vector3 front = new Vector3(-_view.M13, -_view.M23, -_view.M33);

        // SkyBox
        GL.DepthMask(false);
        _skybox!.Draw(_view, _projection);
        GL.DepthMask(true);


        GL.Enable(EnableCap.DepthTest);
        // Render Teapot
        if (_mainShader != null && _teapotMesh != null)
        {
            _mainShader.Use();

            _mainShader.SetVector3("lightPos", _lightPos);
            _mainShader.SetVector3("lightColor", Vector3.One);
            _mainShader.SetVector3("objectColor", new Vector3(1f, 0.5f, 0.31f));
            _mainShader.SetMatrix4("model", _model);
            _mainShader.SetMatrix4("view", _view);
            _mainShader.SetMatrix4("projection", _projection);

            _teapotMesh.Draw();
        }

        // Text Render
        if (_textRenderer != null && debug_menu)
        {
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1, 1);
            _textRenderer.DrawString("EOCS V0.1.0", 10, 7, 0.7f, ortho, 1f);
            _textRenderer.DrawString($"FPS: {(1.0 / e.Time):F1}", 10, 63, 0.7f, ortho, 1f); 
            _textRenderer.DrawString($"Position (XYZ): {_camPos.X:F1} {_camPos.Y:F1} {_camPos.Z:F1}", 10, 91, 0.7f, ortho, 1f); 
            _textRenderer.DrawString($"Direction (Normalized): {front.X:F1}, {front.Y:F1}, {front.Z:F1}", 10, 119, 0.7f, ortho, 1f); 
        }

        Context.SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
       _projection = Matrix4.CreatePerspectiveFieldOfView(FOV, Size.X / (float)Size.Y, 0.1f, 1000.0f);
    }

    protected override void OnUnload()
    {
        _teapotMesh?.Dispose();
        _mainShader?.Dispose();
        _skybox?.Dispose();
        _textRenderer?.Dispose();

        base.OnUnload();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        FOV -= e.OffsetY * 0.1f;
        FOV = MathHelper.Clamp(FOV, MathHelper.DegreesToRadians(30), MathHelper.DegreesToRadians(110));

        _projection = Matrix4.CreatePerspectiveFieldOfView(FOV, Size.X / (float)Size.Y, 0.1f, 100.0f);
    }
}