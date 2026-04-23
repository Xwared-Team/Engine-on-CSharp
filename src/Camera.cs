namespace EOCS.Core;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Camera
{
    public Vector3 Front => _front;
    public Vector3 Position { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    
    private float _fov = MathHelper.PiOver4;
    public float FOV
    { 
        get => _fov; 
        set 
        {
            _fov = MathHelper.Clamp(value, 
                MathHelper.DegreesToRadians(1.0f), 
                MathHelper.DegreesToRadians(90.0f));
        }
    }

    private Vector3 _front;
    private Vector3 _up;
    private Vector3 _right;
    private readonly Vector3 _worldUp = Vector3.UnitY;

    public float MoveSpeed { get; set; } = 5.0f;
    public float MouseSensitivity { get; set; } = 0.1f;

    public Camera(Vector3 position, float yaw = -90.0f, float pitch = 0.0f)
    {
        Position = position;
        Yaw = yaw;
        Pitch = pitch;
        UpdateVectors();
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }

    private void UpdateVectors()
    {
        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        front.Y = -MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        
        _front = Vector3.Normalize(front);
        _right = Vector3.Normalize(Vector3.Cross(_front, _worldUp));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
    }

    public void ProcessInput(KeyboardState input, float deltaTime)
    {
        float velocity = MoveSpeed * deltaTime;

        if (input.IsKeyDown(Keys.W)) Position += _front * velocity;
        if (input.IsKeyDown(Keys.S)) Position -= _front * velocity;
        if (input.IsKeyDown(Keys.A)) Position -= _right * velocity;
        if (input.IsKeyDown(Keys.D)) Position += _right * velocity;

        if (input.IsKeyDown(Keys.Space)) Position += Vector3.UnitY * velocity;
        if (input.IsKeyDown(Keys.LeftShift)) Position -= Vector3.UnitY * velocity;
    }

    public void ProcessMouseMovement(float xOffset, float yOffset)
    {
        xOffset *= MouseSensitivity;
        yOffset *= MouseSensitivity;

        Yaw += xOffset;
        Pitch += yOffset;

        if (Pitch > 89.0f) Pitch = 89.0f;
        if (Pitch < -89.0f) Pitch = -89.0f;

        UpdateVectors();
    }
    
    public void ProcessMouseScroll(float yOffset)
    {
        FOV -= (yOffset / 120.0f) * 0.1f;
    }
}