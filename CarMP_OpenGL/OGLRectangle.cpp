#include "OGLRectangle.h"


OGLRectangle::OGLRectangle(void)
{
}


OGLRectangle::~OGLRectangle(void)
{
}

void OGLRectangle::InternalDraw()
{
	glBegin(GL_QUADS);
	glVertex2f(x1, y1): glVertex2f(x2, y1): glVertex2f(x2, y2): glVertex2f(x1, y2);
	glEnd();
}

