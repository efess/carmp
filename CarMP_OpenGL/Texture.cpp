#include "Texture.h"

const GLfloat Texture::_default_vertex_array[] = { 
    -1.0f, -1.0f,
     1.0f, -1.0f,
    -1.0f,  1.0f,
     1.0f,  1.0f
};

const GLushort Texture::_default_element_array[] = { 0, 1, 2, 3 };

bool Texture::_isIlInitialized = false;

Texture::Texture(const char* pPath)
	: m_vertex_buffer(GL_ARRAY_BUFFER, _default_vertex_array, sizeof(*_default_vertex_array)),
	m_element_buffer(GL_ELEMENT_ARRAY_BUFFER, _default_element_array, sizeof(*_default_element_array))
{
	this->LoadImage(pPath);
}

Texture::~Texture()
{
	// KILL IT!
}

void Texture::Draw(void)
{
	//glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, m_textureId);
    
	//glUniform1i(m_textureId, 0);

	glBindBuffer(GL_ARRAY_BUFFER, m_vertex_buffer.GetId());
	glVertexPointer(2, GL_FLOAT,sizeof(GLfloat)*2, (void*)0);
	
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_element_buffer.GetId());
	glTexCoordPointer(2, GL_FLOAT,sizeof(GLfloat)*2, (void*)0);
	// No attribute pointer - no shaders...
    //glVertexAttribPointer(
    //    g_resources.attributes.position,  /* attribute */
    //    2,                                /* size */
    //    GL_FLOAT,                         /* type */
    //    GL_FALSE,                         /* normalized? */
    //    sizeof(GLfloat)*2,                /* stride */
    //    (void*)0                          /* array buffer offset */
    //);
    //glEnableVertexAttribArray(g_resources.attributes.position);

	glDrawElements(GL_TRIANGLE_STRIP,
		4,
		GL_UNSIGNED_SHORT,
		(void*)0);
	
	GLenum err = glGetError();
	if(err != GL_NO_ERROR)
	{
		
	}
}

bool Texture::LoadImage(const char* filename)
{
	Texture::InitializeIL();

	/* We want all images to be loaded in a consistent manner */
	ilEnable(IL_ORIGIN_SET);
	/* In the next section, we load one image */
	ilGenImages(1, & handle);
	ilBindImage(handle);
	ILboolean loaded = ilLoadImage(filename);
	if (loaded == IL_FALSE)
	return -1; /* error encountered during loading */
	/* Let’s spy on it a little bit */
	width = ilGetInteger(IL_IMAGE_WIDTH); // getting image width
	height = ilGetInteger(IL_IMAGE_HEIGHT); // and height
	
	/* how much memory will we need? */
	int memory_needed = width * height * 3 * sizeof(unsigned char);
	 
	/* We multiply by 3 here because we want 3 components per pixel */
	ILubyte * data = (ILubyte *)malloc(memory_needed);
	
	/* finally get the image data */
	ilCopyPixels(0, 0, 0, width, height, 1, IL_RGB, IL_UNSIGNED_BYTE, data);
	
	///* And maybe we want to save that all... */
	//ilSetPixels(0, 0, 0, w, h, 1, IL_RGB, IL_UNSIGNED_BYTE, data);
	///* and dump them to the disc... */
	//ilSaveImage("our_result.png");
	/* Finally, clean the mess! */	
    
	glGenTextures(1, &m_textureId);
    glBindTexture(GL_TEXTURE_2D, m_textureId);

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S,     GL_CLAMP_TO_EDGE);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T,     GL_CLAMP_TO_EDGE);

	glTexImage2D(
        GL_TEXTURE_2D, 0,           /* target, level of detail */
        GL_RGB8,                    /* internal format */
        width, height, 0,           /* width, height, border */
        GL_BGR, GL_UNSIGNED_BYTE,   /* external format, type */
        data                        /* pixels */
    );

	GLenum err = glGetError();
	if(err != GL_NO_ERROR)
	{
		
	}
		//Utils::ShowInfoLog(

	ilDeleteImages(1, & handle);
	free(data); data = NULL;
	return 0;
}

GLuint Texture::GetTextureId(void)
{
	return m_textureId;
}

void Texture::InitializeIL(void)
{
	if(_isIlInitialized)
		return;
	
	ilInit();

	_isIlInitialized = true;
}