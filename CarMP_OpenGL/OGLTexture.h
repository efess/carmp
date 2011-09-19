#include "GLResourceBase.h"
#include "SFML/Graphics/Texture.hpp"
#include "SFML/Graphics/Sprite.hpp"
#include "GLBuffer.h"	
#include "Utils.h"
#include <iostream>

class OGLTexture
	: public GLResourceBase
{
public:
	void SetDimensions(OGL_RECT pRect);
	
	OGLTexture(const char* pByteArray, int pStride);
	OGLTexture(const char* pPath);
	~OGLTexture(void);
private:
	OGL_RECT currentRect;
	sf::Sprite m_sprite;
	sf::Texture m_texture;
protected:
	virtual bool LoadImageFromPath(const char* filename);
	virtual bool LoadImageFromByteArray(const char* byteArray, int stride);
	void InternalDraw(sf::RenderWindow* renderer);

};