namespace EOCS.PrimitiveFigures._2D.Primitive2D_AbstractClass;

public abstract class Primitive2D : IDisposable
{
    private int VBO;
    private int VAO;
    private int ShaderProgram = -1;

    public Vector2 Position {get; set;} = Vector2.Zero;
    public Vector3 Color {get; set;} = Vector3.One;
    public float Alpha {get; set;} = 1.0f;
    public Vector2 Scale {get; set;} = Vector2.One;
    public float Rotation {get; set;} = 0.0f; 

    protected int VertexCount {get; private set;}

    private const string pathVertex = "./Assets/shaders/base.vert";
    private const string pathFragment = "./Assets/shaders/base.frag";

    public Primitive2D()
    {
        InitializeGeometry();
        InitializeShader();
    }

    public Primitive2D(Vector2 position, Vector2 scale, Vector3 color, float alpha, float rotation) : this()
    {
        Position = position;
        Scale = scale;
        Color = color;
        Alpha = alpha;
        Rotation = rotation;
    }

    private void InitializeShader()
    {
        if (ShaderProgram != -1) return;

        if (!File.Exists(pathVertex))
        {
            Console.WriteLine($"[Primitive2D] Cant load file '{pathVertex}' - cant exist");
            return;
        }

        if (!File.Exists(pathFragment))
        {
            Console.WriteLine($"[Primitive2D] Cant load file '{pathFragment}' - cant exist");
            return; 
        }

        string vertexSource = File.ReadAllText(pathVertex);
        string fragmentSource = File.ReadAllText(pathFragment);

        int vertShader = GL.CreateShader(ShaderType.VertexShader);
        int fragShader = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vertShader, vertexSource);
        GL.ShaderSource(fragShader, fragmentSource);

        GL.CompileShader(vertShader);
        GL.CompileShader(fragShader);

        CheckShaderCompilation(vertShader);
        CheckShaderCompilation(fragShader);

        ShaderProgram = GL.CreateProgram();
        GL.AttachShader(ShaderProgram, vertShader);
        GL.AttachShader(ShaderProgram, fragShader);

        GL.LinkProgram(ShaderProgram);
        CheckShaderLink();

        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);
    }

    private void InitializeGeometry()
    {   
        float[] vertices = GetVertices();
        VertexCount = vertices.Length / 2;

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void CheckShaderCompilation(int shaderId)
    {   
        GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
            throw new Exception($"[Primitive2D] Shader Error: {GL.GetShaderInfoLog(shaderId)}");
    }

    public void CheckShaderLink()
    {
        GL.GetProgram(ShaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
            throw new Exception($"[Primitive2D] Link Error: {GL.GetProgramInfoLog(ShaderProgram)}");
    }

    protected abstract float[] GetVertices();

        public void Draw(Matrix4 projection)
    {
        GL.UseProgram(ShaderProgram);

        Matrix4 model = Matrix4.Identity;
        model *= Matrix4.CreateScale(Scale.X, Scale.Y, 1.0f);
        model *= Matrix4.CreateRotationZ(Rotation);
        model *= Matrix4.CreateTranslation(Position.X, Position.Y, 0.0f);

        int locProj = GL.GetUniformLocation(ShaderProgram, "uProjection");
        int locModel = GL.GetUniformLocation(ShaderProgram, "uModel");
        int locColor = GL.GetUniformLocation(ShaderProgram, "uColor");

        GL.UniformMatrix4(locProj, false, ref projection);
        GL.UniformMatrix4(locModel, false, ref model);
        
        Vector4 finalColor = new Vector4(Color.X, Color.Y, Color.Z, Alpha);
        
        GL.Uniform4(locColor, finalColor);

        GL.BindVertexArray(VAO);
        GL.DrawArrays(PrimitiveType.TriangleFan, 0, VertexCount);
        GL.BindVertexArray(0);
    }

    public virtual void Dispose()
    {
        GL.DeleteVertexArray(VAO);
        GL.DeleteBuffer(VBO);

    }
}