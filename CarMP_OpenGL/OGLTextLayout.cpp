#include "OGLTextLayout.h"

char* OGLTextLayout::FONT_INSTALLATION = "C:\\Windows\\Fonts\\";
char* OGLTextLayout::FONT_EXTENSION = ".ttf";

OGLTextLayout::OGLTextLayout(void)
{
	m_initialized = 0;
}

OGLTextLayout::OGLTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment)
{
	m_initialized = 0;
	if(m_font.LoadFromFile(GetFontPath(pFont)))
	{
		// NO Error
		m_text = sf::Text(pString);

		m_text.SetFont(m_font);
		m_text.SetCharacterSize(pSize);
		m_text.SetStyle(sf::Text::Regular);
		m_text.SetPosition(100,100);

		char tempChar1[1024];
		strcpy(tempChar1, pString);

		m_currentString = tempChar1;
		
		char tempChar2[80];
		strcpy(tempChar2, pFont);

		m_currentFont = tempChar2;
		m_currentSize = pSize;
		m_currentAlignment = pAlignment;
		
		m_initialized = 1;
	}
	
}


OGLTextLayout::~OGLTextLayout(void)
{
	
	//free(m_font);
}

void OGLTextLayout::SetDimensions(OGL_RECT pRect)
{
	if(currentRect.x != pRect.x
		|| currentRect.y != pRect.y
		|| currentRect.width != pRect.width
		|| currentRect.height != pRect.height)
	{
		currentRect = pRect;
		
		m_text.SetPosition(pRect.x, pRect.y);
		//m_text.(pRect.width, pRect.height); // TODO: Bounding box??
	}
}

void OGLTextLayout::InternalDraw(sf::RenderWindow* renderer)
{
	if(m_initialized)
		renderer->Draw(m_text);
}

std::string OGLTextLayout::GetFontPath(const char* pFontName)
{
	std::stringstream stream;
	std::string fontName = std::string(pFontName);

	stream << OGLTextLayout::FONT_INSTALLATION;
	stream << fontName;
	stream << OGLTextLayout::FONT_EXTENSION;

	std::string returnString = stream.str();
	
	return returnString;
}