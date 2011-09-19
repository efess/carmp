#pragma once
#include <SFML/OpenGL.hpp>


typedef struct Vertex 
{
	float X;
	float Y;
};


class GLBuffer
{
public:
	GLint GetId(void);
	GLBuffer(GLenum, const void *, GLsizei);
	~GLBuffer(void);
private:
	GLuint m_bufferId;
};
