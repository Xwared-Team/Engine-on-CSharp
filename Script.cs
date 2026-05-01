namespace EOCS.Script;

using EOCS.SkyBox;

[SupportedOSPlatform("windows")]
public class MyGame : BaseGame
{
    private List<GameObject> _objects = new List<GameObject>();
    private Skybox? _skybox;

    public override void Load(Matrix4 initialProjection)
    {
        ActiveCamera = new Camera(new Vector3(0, 0, 0), -90.0f, 0.0f);

        var modelData = ObjLoader.Load("./Assets/3D_objects/teapol.obj");
        var mesh = new Mesh(modelData.Vertices, modelData.Indices);
        var shader = new ShaderProgram(
            "./Assets/shaders/main/shader.vert", 
            "./Assets/shaders/main/shader.frag"
        );

        var teapot1 = new GameObject(mesh, shader)
        {
            Position = new Vector3(-20, 0, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.8f, 0.2f, 0.2f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(teapot1);

        var teapot2 = new GameObject(mesh, shader)
        {
            Position = new Vector3(20, 0, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.2f, 0.2f, 0.8f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(teapot2);

        var teapot3 = new GameObject(mesh, shader)
        {
            Position = new Vector3(-20, 20, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.2f, 0.8f, 0.2f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(teapot3);
        
        var teapot4 = new GameObject(mesh, shader)
        {
            Position = new Vector3(20, 20, -50), 
            Scale = new Vector3(0.5f),
            Color = new Vector3(0.8f, 0.8f, 0.2f),
            LightPos = new Vector3(10f, 15f, 10f)
        };
        _objects.Add(teapot4);

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
    }

    public override void Update(KeyboardState input, float deltaTime)
    {
        base.Update(input, deltaTime);
    }

    public override void Draw(Matrix4 projection)
    {
        if (ActiveCamera == null) return;
        Matrix4 view = ActiveCamera.GetViewMatrix();

        _skybox!.Draw(view, projection);

        foreach (var obj in _objects)
        {
            obj.Draw(view, projection);
        }
    }
}