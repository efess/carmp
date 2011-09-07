#include "GLResourceBase.h"


GLResourceBase::GLResourceBase(void)
{
}

void GLResourceBase::Draw()
{
	BeginDraw();

	InternalDraw();

	EndDraw();
}

void GLResourceBase::BeginDraw()
{
	glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
}

void GLResourceBase::EndDraw()
{
}

GLResourceBase::~GLResourceBase(void)
{
}
