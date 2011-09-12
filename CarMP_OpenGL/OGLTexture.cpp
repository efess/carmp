#include "OGLTexture.h"

const GLfloat OGLTexture::_default_vertex_array[] = { 
    -1.0f, -1.0f,
     1.0f, -1.0f,
    -1.0f,  1.0f,
     1.0f,  1.0f
};

const GLushort OGLTexture::_default_element_array[] = { 0, 1, 2, 3 };

bool OGLTexture::_isIlInitialized = false;

OGLTexture::OGLTexture(const char* pPath)
	: m_vertex_buffer(GL_ARRAY_BUFFER, _default_vertex_array, sizeof(*_default_vertex_array)),
	m_element_buffer(GL_ELEMENT_ARRAY_BUFFER, _default_element_array, sizeof(*_default_element_array))
{
	this->LoadImage(pPath);
}

OGLTexture::~OGLTexture()
{
	// KILL IT!
}

void OGLTexture::SetDimensions(OGL_RECT pRect)
{
	if(currentRect.x != pRect.x
		|| currentRect.y != pRect.y
		|| currentRect.width != pRect.width
		|| currentRect.height != pRect.height)
	{
		currentRect = pRect;
	}
}

void OGLTexture::InternalDraw(void)
{
	float x2 = currentRect.x + currentRect.width;
	float y2 = currentRect.y + currentRect.height;

	//glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, m_textureId);
	glEnable(GL_TEXTURE_2D);
    
	//glUniform1i(m_textureId, 0);
	glBegin(GL_QUADS);

  /*  glTexCoord2f(0.0f, 0.0f); glVertex2f(0.0f, 0.0f);
    glTexCoord2f(0.0f, 1.0f); glVertex2f(0.0f, 1.0f);
    glTexCoord2f(1.0f, 1.0f); glVertex2f(1.0f, 1.0f);
    glTexCoord2f(1.0f, 0.0f); glVertex2f(1.0f, 0.0f);*/
	
	/*
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);
	glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, (int)GL_MODULATE);
*/
    glTexCoord2f(0.0f, 0.0f); glVertex2f(currentRect.x, currentRect.y);
    glTexCoord2f(0.0f, 1.0f); glVertex2f(currentRect.x, y2);
    glTexCoord2f(1.0f, 1.0f); glVertex2f(x2, y2);
    glTexCoord2f(1.0f, 0.0f); glVertex2f(x2, currentRect.y);

	glEnd();

	/*glGenTextures(1, texture);
	glBindTexture(GL_TEXTURE_2D, texture[0]);
*/
	
	
	 
}

bool OGLTexture::LoadImage(const char* filename)
{
	OGLTexture::InitializeIL();

	char tempCopy[600];
	
	strcpy(tempCopy, filename);

	/*glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S,     GL_CLAMP_TO_EDGE);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T,     GL_CLAMP_TO_EDGE);
	*/
//	m_textureId = ilutOglLoadImage(filename);
	m_textureId = ilutGLLoadImage(tempCopy);
	
	
	/* We want all images to be loaded in a consistent manner */
	//ilEnable(IL_ORIGIN_SET);
	///* In the next section, we load one image */
	//ilGenImages(1, & handle);
	//ilBindImage(handle);
	//ILboolean loaded = ilLoadImage(filename);
	//if (loaded == IL_FALSE)
	//return -1; /* error encountered during loading */
	///* Let’s spy on it a little bit */
	//width = ilGetInteger(IL_IMAGE_WIDTH); // getting image width
	//height = ilGetInteger(IL_IMAGE_HEIGHT); // and height
	//
	///* how much memory will we need? */
	//int memory_needed = width * height * 3 * sizeof(unsigned char);
	// 
	///* We multiply by 3 here because we want 3 components per pixel */
	//ILubyte * data = (ILubyte *)malloc(memory_needed);
	//
	///* finally get the image data */
	//ilCopyPixels(0, 0, 0, width, height, 1, IL_RGB, IL_UNSIGNED_BYTE, data);
	//
	/////* And maybe we want to save that all... */
	////ilSetPixels(0, 0, 0, w, h, 1, IL_RGB, IL_UNSIGNED_BYTE, data);
	/////* and dump them to the disc... */
	////ilSaveImage("our_result.png");
	///* Finally, clean the mess! */	
 //   
	//glGenTextures(1, &m_textureId);
 //   glBindTexture(GL_TEXTURE_2D, m_textureId);

	//glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
 //   glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
 //   glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S,     GL_CLAMP_TO_EDGE);
 //   glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T,     GL_CLAMP_TO_EDGE);
	//
	//GLuint ilutOglBindTexImage();

	////glTexImage2D(
 ////       GL_TEXTURE_2D, 0,           /* target, level of detail */
 ////       GL_RGB8,                    /* internal format */
 ////       width, height, 0,           /* width, height, border */
 ////       GL_BGR, GL_UNSIGNED_BYTE,   /* external format, type */
 ////       data                        /* pixels */
 ////   );

	//GLenum err = glGetError();
	//if(err != GL_NO_ERROR)
	//{
	//	
	//}
	//	//Utils::ShowInfoLog(

	//ilDeleteImages(1, & handle);
	//free(data); data = NULL;
	return 0;
}

GLuint OGLTexture::GetTextureId(void)
{
	return m_textureId;
}

void OGLTexture::InitializeIL(void)
{
	if(_isIlInitialized)
		return;
	
	ilInit();
	iluInit();

	ilutRenderer(ILUT_OPENGL);

	_isIlInitialized = true;
}