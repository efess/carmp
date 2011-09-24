#include "OGLTextLayout.h"

FontRepository* OGLTextLayout::Fonts;

OGLTextLayout::OGLTextLayout(void)
{	
	if(Fonts == NULL)
		Fonts = new FontRepository();

	m_initialized = 0;
}

OGLTextLayout::OGLTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment)
{
	if(Fonts == NULL)
		Fonts = new FontRepository();

	m_initialized = 0;
	sf::Font* font = Fonts->GetFont(pFont);

	if(font != NULL)
	{
		m_text = new sf::Text(pString);

		m_text->SetFont(*font);
		m_text->SetCharacterSize(pSize);
		m_text->SetStyle(sf::Text::Regular);
		m_text->SetPosition(100,100);

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
	free(m_text);
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
		
		m_text->SetPosition(pRect.x, pRect.y);
		//m_text.(pRect.width, pRect.height); // TODO: Bounding box??
	}
}

void OGLTextLayout::InternalDraw(sf::RenderWindow* renderer)
{
	if(m_initialized)
		renderer->Draw(*m_text);
}
