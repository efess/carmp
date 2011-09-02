#include "GLBuffer.h"


GLBuffer::GLBuffer(
    GLenum target,
    const void *buffer_data,
    GLsizei buffer_size) 
{
    GLuint buffer;
    glGenBuffers(1, &buffer);
    glBindBuffer(target, buffer);
    glBufferData(target, buffer_size, buffer_data, GL_STATIC_DRAW);
    
	m_bufferId = buffer;
}

GLBuffer::~GLBuffer(void)
{
	
}

GLint GLBuffer::GetId(void)
{
	return m_bufferId;
}