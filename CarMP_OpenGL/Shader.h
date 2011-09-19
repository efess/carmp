#pragma once
#include "Utils.h"

class Shader
{
public:
	GLint GetId(void);
	Shader(GLenum, const char *);
	~Shader(void);
private:
	GLint m_shaderId;
};

