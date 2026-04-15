#version 330 core
in vec2 vUV;
in vec4 vColor;
out vec4 FragColor;
uniform sampler2D uFontAtlas;

void main()
{
    vec4 tex = texture(uFontAtlas, vUV);
    float brightness = dot(tex.rgb, vec3(0.299, 0.587, 0.114));
    
    if (brightness < 0.1) discard;
    
    FragColor = vColor;
}