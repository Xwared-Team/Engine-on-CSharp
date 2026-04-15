namespace BPX;
using System.Runtime.Versioning;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System.Drawing;

[SupportedOSPlatform("windows")]
class Main(GameWindowSettings gSettings, NativeWindowSettings nSettings) : GameWindow(gSettings, nSettings)
{
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
    float FOV = MathHelper.PiOver4;

    float _rotateObject;

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
        WindowState = WindowState.Fullscreen;
        GL.Enable(EnableCap.DepthTest);

        ObjModel model = ObjLoader.Load("./Assets/3D_objects/teapol.obj");

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

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        ReadedVertFile = File.ReadAllText("./Assets/shaders/shader.vert");
        vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, ReadedVertFile);

        ReadedFragFile = File.ReadAllText("./Assets/shaders/shader.frag");
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

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
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

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
             _camPos += front * velocity;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            _camPos -= front * velocity;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            _camPos -= right * velocity;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            _camPos += right * velocity;

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            _camPos.Y += velocity;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftShift))
            _camPos.Y -= velocity;

        if (input.IsKeyReleased(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape)){
            _GamePaused = !_GamePaused;
            if (_GamePaused){ CursorState = CursorState.Normal; } else { CursorState = CursorState.Grabbed; }
        }

        Vector3 target = _camPos + front;
        _view = Matrix4.LookAt(_camPos, target, up);

        _rotateObject += 0.1f;

        _model = Matrix4.CreateScale(0.1f) * 
                 Matrix4.CreateTranslation(0, 0, 12) * 
                 Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotateObject));

        // Grachic Logic
        GL.ClearColor(0, 0, 0, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_handle);
        GL.BindVertexArray(_vertexArrayObject);

        GL.UniformMatrix4(_modelLoc, false, ref _model);
        GL.UniformMatrix4(_viewLoc, false, ref _view);
        GL.UniformMatrix4(_projLoc, false, ref _projection);

        GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

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