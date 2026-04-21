#version 330 core
in vec2 TexCoord;
out vec4 FragColor;

uniform sampler2D uTexture;
uniform vec3 textColor;

void main()
{
    vec4 sampled = texture(uTexture, TexCoord);
    FragColor = vec4(textColor, 1.0) * sampled; 
}