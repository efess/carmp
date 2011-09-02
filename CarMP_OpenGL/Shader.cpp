#include "Shader.h"

Shader::Shader(GLenum type, const char *filename)
{
    GLint length;
    void *source = Utils::file_contents(filename, &length);
    GLuint shader;
    GLint shader_ok;

    if (!source)
        return; //ERROR
	
	shader = glCreateShader(type);
    glShaderSource(shader, 1, (const GLchar**)&source, &length);
    free(source);
    glCompileShader(shader);

	glGetShaderiv(shader, GL_COMPILE_STATUS, &shader_ok);
    if (!shader_ok) {
        fprintf(stderr, "Failed to compile %s:\n", filename);
        
		Utils::ShowInfoLog(shader, glGetShaderiv, glGetShaderInfoLog);

        glDeleteShader(shader);

        return;// ERROR?
    }
    m_shaderId = shader;
}

GLint Shader::GetId(void)
{
	return m_shaderId;
}

Shader::~Shader(void)
{
}
