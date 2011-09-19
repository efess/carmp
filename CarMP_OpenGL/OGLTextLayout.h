#pragma once

#include <map>
#include <sstream>
#include <string>
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
	static char* FONT_INSTALLATION;
	static char* FONT_EXTENSION;
	static std::string GetFontPath(const char* pFontName);

	OGL_RECT currentRect;

	
	sf::Font m_font;
	sf::Text m_text;

	char* m_currentString;
	char* m_currentFont;
	float m_currentSize;
	int m_currentAlignment;

	bool m_initialized;
protected:
	bool LoadImage(const char* filename);
	void InternalDraw(sf::RenderWindow* renderer);
};

