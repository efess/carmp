#pragma once

#include <map>
#include <string>
#include <sstream>
#include "GLResourceBase.h"

class FontRepository
{
public:
	FontRepository(void);
	~FontRepository(void);

	sf::Font* GetFont(const char* pFontName);
private:
	static char* FONT_INSTALLATION;
	static char* FONT_EXTENSION;
	std::string GetFontPath(std::string pFontName);
	std::map<std::string, sf::Font*> fontCache;

};

