namespace EOCS;
using System.Runtime.Versioning;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

[SupportedOSPlatform("windows")]
class Main(GameWindowSettings gSettings, NativeWindowSettings nSettings) : GameWindow(gSettings, nSettings)
{
    public static bool debug_mod = false;
    int _vertexBufferObject;
    int _vertexArrayObject;
    int _elementBufferObject;

    string? ReadedVertFile;
    int vertexShader;

    string? ReadedFragFile;
    int fragmentShader;

    int _handle;

    int _modelLoc, _viewLoc, _projLoc;
    Matrix4 _model, _view, _projection;

    int _vertexCount;
    int _indexCount;

    Vector3 _camPos = new Vector3(0, 0, 60);

    float _yaw = -90.0f;
    float _pitch = 0.0f;

    float _moveSpeed = 5.0f;
    float _mouseSensitivy = 0.1f;
    bool _GamePaused = false;
    bool _GameFullscreen = false;
    float FOV = MathHelper.PiOver4;

    float _rotateObject;

    // Light
    int _lightPosLoc, _lightColorLoc, _objectColorLoc;
    Vector3 _lightPos = new Vector3(10f, 10f, 10f);

    Skybox? _skybox;

    private TextRenderer? _textRenderer;

    void CheckShaderCompilation(int shaderId)
    {   
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);

        if (success == 0)
        {
            string LogInfo = GL.GetShaderInfoLog(shaderId);
            Console.WriteLine($"Shrder Error: {LogInfo}");
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();     
        Context.SwapInterval = 1;
        CursorState = CursorState.Grabbed;
        if (_GameFullscreen) 
            WindowState = WindowState.Fullscreen;
        GL.Enable(EnableCap.DepthTest);

        ObjModel model = ObjLoader.Load("./Assets/3D_objects/teapol.obj");

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

        _textRenderer = new TextRenderer(
            "./Assets/fonts/VCR-OSD-MONO.ttf",
            32,
            "./Assets/shaders/text/shader.vert",
            "./Assets/shaders/text/shader.frag"
        );

        _vertexCount = model.Vertices.Length / 3;
        _indexCount = model.Indices.Length;

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, 
                      model.Vertices.Length * sizeof(float),
                      model.Vertices,
                      BufferUsageHint.StaticDraw
        );

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, 
                      model.Indices.Length * sizeof(uint),
                      model.Indices,
                      BufferUsageHint.StaticDraw);

        int stride = 6 * sizeof(float);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        ReadedVertFile = File.ReadAllText("./Assets/shaders/main/shader.vert");
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, ReadedVertFile);

        ReadedFragFile = File.ReadAllText("./Assets/shaders/main/shader.frag");
        fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, ReadedFragFile);

        GL.CompileShader(vertexShader);
        GL.CompileShader(fragmentShader);

        CheckShaderCompilation(vertexShader);
        CheckShaderCompilation(fragmentShader);

        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        GL.LinkProgram(_handle);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        _lightPosLoc = GL.GetUniformLocation(_handle, "lightPos");
        _lightColorLoc = GL.GetUniformLocation(_handle, "lightColor");
        _objectColorLoc = GL.GetUniformLocation(_handle, "objectColor");

        _modelLoc = GL.GetUniformLocation(_handle, "model");
        _viewLoc = GL.GetUniformLocation(_handle, "view");
        _projLoc = GL.GetUniformLocation(_handle, "projection");

        _model = Matrix4.Identity;
        _view = Matrix4.LookAt(new Vector3(0, 20, 80), Vector3.Zero, Vector3.UnitY);
        _projection = Matrix4.CreatePerspectiveFieldOfView(FOV, Size.X / (float)Size.Y, 0.1f, 100.0f);
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
        

        Vector3 target = _camPos + front;
        _view = Matrix4.LookAt(_camPos, target, up);

        _rotateObject += 0.1f;
        _model = Matrix4.CreateTranslation(0, 0, 12) * 
                 Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotateObject)) * 
                 Matrix4.CreateScale(0.1f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _skybox!.Draw(_view, _projection);

        GL.UseProgram(_handle);
        
        GL.Uniform3(_lightPosLoc, _lightPos);
        GL.Uniform3(_lightColorLoc, new Vector3(1f, 1f, 1f));
        GL.Uniform3(_objectColorLoc, new Vector3(1f, 0.5f, 0.31f));

        GL.UniformMatrix4(_modelLoc, false, ref _model);
        GL.UniformMatrix4(_viewLoc, false, ref _view);
        GL.UniformMatrix4(_projLoc, false, ref _projection);

        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

        if (_textRenderer != null)
        {
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1, 1);
            _textRenderer.DrawString("EOCS Engine V0.0.4", 10, 10, 1f, ortho, 1f); 
            _textRenderer.DrawString("Create by Dovintc", 10, 50, 1f, ortho, 1f); 
            _textRenderer.DrawString($"FPS: {(1.0 / e.Time):F1}", 10, 90, 1f, ortho, 1f); 
        }

        Context.SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

       _projection = Matrix4.CreatePerspectiveFieldOfView(FOV, Size.X / (float)Size.Y, 0.1f, 100.0f);
    }

    protected override void OnUnload()
    {
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteBuffer(_elementBufferObject);

        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteProgram(_handle);

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