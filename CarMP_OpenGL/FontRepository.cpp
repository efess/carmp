#include "FontRepository.h"

char* FontRepository::FONT_INSTALLATION = "C:\\Windows\\Fonts\\";
char* FontRepository::FONT_EXTENSION = ".ttf";


FontRepository::FontRepository(void)
	: fontCache()
{

}


FontRepository::~FontRepository(void)
{  
	std::map<std::string, sf::Font*> ::const_iterator end = fontCache.end(); 
    for (std::map<std::string, sf::Font*> ::const_iterator it = fontCache.begin(); it != end; ++it)
    {
		free(it->second);
    }
}

std::string FontRepository::GetFontPath(std::string pFontName)
{
	std::stringstream stream;
	std::string fontName = std::string(pFontName);

	stream << FontRepository::FONT_INSTALLATION;
	stream << fontName;
	stream << FontRepository::FONT_EXTENSION;

	std::string returnString = stream.str();
	
	return returnString;
}

sf::Font * FontRepository::GetFont(const char* pFontName)
{
	std::string fontName = std::string(pFontName);
	
	std::map<std::string, sf::Font*>::iterator fontIterator;

	fontIterator = fontCache.find(fontName);

	if(fontIterator == fontCache.end()) 
	{
		std::string fontPath = GetFontPath(pFontName);
		sf::Font* font = new sf::Font();

		if(font->LoadFromFile(fontPath))
		{
			fontCache[fontName] = font;
			return font;
		};
		return NULL;
	}
	else
	{
		sf::Font* font = fontIterator->second;
		
		return font;
	}
}