namespace EOCS.Base;

[SupportedOSPlatform("windows")]
public abstract class BaseGame
{
    public Camera? ActiveCamera { get; set; }

    public virtual void Load(Matrix4 initialProjection) 
    {
        if (ActiveCamera == null)
            ActiveCamera = new Camera(new Vector3(0, 5, 10), -90.0f, 0.0f);
    }
    public virtual void Update(KeyboardState input, float deltaTime) 
    {
        ActiveCamera?.ProcessInput(input, deltaTime);
    }
    public virtual void Draw(Matrix4 projection){}
}