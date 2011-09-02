#pragma once
#include "Utils.h"
#include <GL\glew.h>
#include <GL\glut.h>

class Shader
{
public:
	GLint GetId(void);
	Shader(GLenum, const char *);
	~Shader(void);
private:
	GLint m_shaderId;
};

