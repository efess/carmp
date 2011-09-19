#pragma once
#include <SFML/OpenGL.hpp>
#include <SFML/Graphics.hpp>
// OpenGL Includes

#include "Utils.h"

// Bounds are from 0 - 1
typedef struct OGL_RECT {
	float x;
	float y;
	float width;
	float height;
};

typedef struct OGL_COLOR {
	float r;
	float g;
	float b;
	float a;
};

typedef struct OGL_ELLIPSE {
	float x;
	float y;
	float radius_x;
	float radius_y;
};

typedef struct OGL_POINT {
	float x;
	float y;
};

class GLResourceBase
{
public:
	GLResourceBase(void);
	~GLResourceBase(void);
	void Draw(sf::RenderWindow* renderer);
protected:
	virtual void InternalDraw(sf::RenderWindow* renderer) = 0;
private:
	void BeginDraw(void);
	void EndDraw(void);
};

