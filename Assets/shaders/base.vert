// base.vert
#version 330 core
layout (location = 0) in vec2 aPos;
uniform mat4 uProjection;
uniform mat4 uModel;
uniform vec4 uColor;

out vec4 vColor;

void main()
{
    gl_Position = uProjection * uModel * vec4(aPos, 0.0, 1.0);
    vColor = uColor;
}