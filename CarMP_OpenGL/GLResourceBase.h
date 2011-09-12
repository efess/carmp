#pragma once

// OpenGL Includes

#include "Utils.h"
#include <GL/glew.h>
#ifdef __APPLE__
#  include <GLUT/glut.h>
#else
#  include <GL/glut.h>
#endif

// Bounds are from 0 - 1
typedef struct OGL_RECT {
	float x;
	float y;
	float width;
	float height;
};

typedef struct OGL_COLOR {
	float r;
	float g;
	float b;
	float a;
};

class GLResourceBase
{
public:
	GLResourceBase(void);
	~GLResourceBase(void);
	void Draw(void);
protected:
	virtual void InternalDraw() = 0;
private:
	void BeginDraw(void);
	void EndDraw(void);
};

