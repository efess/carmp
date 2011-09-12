#pragma once
#include "glresourcebase.h"

class OGLRectangle :
	public GLResourceBase
{
public:
	OGLRectangle(void);
	~OGLRectangle(void);
protected:
	void InternalDraw();
};