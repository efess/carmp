#pragma once

#include <iostream>
#include <fstream>

#include <GL/glew.h>
#include <GL/glut.h>


class Utils
{
public:
	Utils(void);
	~Utils(void);
	static void *file_contents(const char *filename, GLint *length);
	static GLchar* LoadFile(const char*, GLint*);
	static void ShowInfoLog(GLuint, 
		PFNGLGETSHADERIVPROC,
		PFNGLGETSHADERINFOLOGPROC);
	static void LoadImage(const char*);
};

