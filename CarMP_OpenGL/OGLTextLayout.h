#pragma once
 
#include "GLResourceBase.h"
#include "ft2build.h"
#include FT_FREETYPE_H

class OGLTextLayout
	: public GLResourceBase
{
public:
	OGLTextLayout(void);
	~OGLTextLayout(void);
	
private:
	static FT_Library library;
	static void InitializeFT(void);
	static bool _isFTInitialized;
	
protected:
	bool LoadImage(const char* filename);
	void InternalDraw();
};

