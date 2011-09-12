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
	glColor3f(1.0f, 1.0f, 1.0f);
	
	glEnable(GL_BLEND);
}

void GLResourceBase::EndDraw()
{
	GLenum errorCode = glGetError();
	const GLubyte* errorString;
	if(errorCode != GL_NO_ERROR)
	{
		errorString = gluErrorString(errorCode);
	}
	glDisable(GL_BLEND);
}

GLResourceBase::~GLResourceBase(void)
{
}
