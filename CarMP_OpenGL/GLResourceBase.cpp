#include "GLResourceBase.h"


GLResourceBase::GLResourceBase(void)
{
}

void GLResourceBase::Draw(sf::RenderWindow* renderer)
{
	BeginDraw();

	InternalDraw(renderer);

	EndDraw();
}

void GLResourceBase::BeginDraw()
{
}

void GLResourceBase::EndDraw()
{
	GLenum errorCode = glGetError();
	const GLubyte* errorString;
	if(errorCode != GL_NO_ERROR)
	{
		errorString = gluErrorString(errorCode);
	}

}

GLResourceBase::~GLResourceBase(void)
{
}
