#include "GLResourceBase.h"
#include "GLBuffer.h"	
#include "Utils.h"
#include <iostream>

#include <GL\glew.h>
#include <GL\glut.h>

#include <IL/il.h>
#include <IL/ilu.h>
#include <IL/ilut.h>


class OGLTexture
	: public GLResourceBase
{
public:
	void Draw(void);
	GLuint GetTextureId(void);
	void SetDimensions(OGL_RECT pRect);
	
	OGLTexture(const char* pPath);
	~OGLTexture(void);
private:
	GLBuffer m_vertex_buffer;
	GLBuffer m_element_buffer;
	ILuint handle, width, height;
	GLuint m_textureId;
	OGL_RECT currentRect;

	static void InitializeIL(void);
	static bool _isIlInitialized;
	static const GLfloat _default_vertex_array[];
	static const GLushort _default_element_array[];

protected:
	virtual bool LoadImage(const char* filename);
	void InternalDraw();

};