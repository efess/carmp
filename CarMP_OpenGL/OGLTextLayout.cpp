#include "OGLTextLayout.h"


bool OGLTextLayout::_isFTInitialized = false;
FT_Library OGLTextLayout::library = NULL;

OGLTextLayout::OGLTextLayout(void)
{
}


OGLTextLayout::~OGLTextLayout(void)
{
}

void OGLTextLayout::InternalDraw()
{

}


void OGLTextLayout::InitializeFT(void)
{
	if(_isFTInitialized)
		return;
	
	  FT_Error error = FT_Init_FreeType( &library );
	  if ( error )
	  {
			//... an error occurred during library initialization ...
	  }

	_isFTInitialized = true;
}