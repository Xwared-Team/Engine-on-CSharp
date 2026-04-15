namespace BPX;
using System.Runtime.Versioning;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

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

    // Light
    int _lightPosLoc, _lightColorLoc, _objectColorLoc;
    Vector3 _lightPos = new Vector3(10f, 10f, 10f);
    float _lightAngle = 0f;
    float _lightRadius = 15f;

    // SkyBox
    Skybox? _skybox;

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

        // SkyBox
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

        _model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotateObject)) * 
                 Matrix4.CreateScale(0.1f);

        _rotateObject += 0.1f;
        float angularSpeed = (2f * MathHelper.Pi) / 10f;
        _lightAngle += angularSpeed * (float)e.Time;

        float lightX = _lightRadius * MathF.Cos(_lightAngle);
        float lightZ = _lightRadius * MathF.Sin(_lightAngle);
        float lightY = 10.0f; 

        _lightPos = new Vector3(lightX, lightY, lightZ);

        GL.Uniform3(_lightPosLoc, _lightPos);
        GL.Uniform3(_lightColorLoc, new Vector3(1f, 1f, 1f));
        GL.Uniform3(_objectColorLoc, new Vector3(0f, 0.5f, 0.71f));

        // Grachic Logic
        GL.ClearColor(0, 0, 0, 1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _skybox!.Draw(_view, _projection);

        GL.UseProgram(_handle);
        GL.BindVertexArray(_vertexArrayObject);

        GL.UniformMatrix4(_modelLoc, false, ref _model);
        GL.UniformMatrix4(_viewLoc, false, ref _view);
        GL.UniformMatrix4(_projLoc, false, ref _projection);

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