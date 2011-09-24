#pragma once

#include <sstream>
#include <string>
#include "FontRepository.h"
#include "GLResourceBase.h"

class OGLTextLayout
	: public GLResourceBase
{
public:
	OGLTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment);
	OGLTextLayout(void);
	~OGLTextLayout(void);
	void SetDimensions(OGL_RECT pRectangle);
private:
	
	static FontRepository* Fonts;
	OGL_RECT currentRect;

	sf::Text* m_text;

	char* m_currentString;
	char* m_currentFont;
	float m_currentSize;
	int m_currentAlignment;

	bool m_initialized;
protected:
	bool LoadImage(const char* filename);
	void InternalDraw(sf::RenderWindow* renderer);
	
};

