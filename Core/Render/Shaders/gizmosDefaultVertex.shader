#version 330 core

in vec2 aPosition;

uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    gl_Position = vec4(aPosition, 0.0, 1.0) * view * projection;
}
