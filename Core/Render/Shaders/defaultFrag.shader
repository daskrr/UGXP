#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D slot;
uniform vec4 color;

void main()
{
    outputColor = texture(slot, texCoord) * color;
}