
#include "CarMPInterface.h"

extern "C" _declspec(dllexport) int CreateOGLWindow(OGL_RECT pRectangle)
{
	
	freopen ("error.txt","w",stderr);
  
    OpenGLManager* manager = OpenGLManager::GetInstance();

	int result = 0;
	try
	{
		result = manager->CreateOGLWindow(pRectangle);
	}
	catch(char * err)
	{
		
	}
	fclose (stderr);
	return result;
}

extern "C" _declspec(dllexport) int RegisterMouseCallback(void (__stdcall *pFunction)(OGL_MOUSE_EVENT))
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	manager->RegisterMouseCallback(pFunction);
	return 1;
}

extern "C" _declspec(dllexport) int RegisterKeyboardCallback(void (__stdcall *pFunction)(OGL_KEYBOARD_EVENT))
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	manager->RegisterKeyboardCallback(pFunction);
	return 1;
}

extern "C" _declspec(dllexport) int RegisterRenderCallback(void (__stdcall *pFunction)(void))
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	manager->RegisterRenderCallback(pFunction);
	return 1;
}

extern "C" _declspec(dllexport) int DrawImage(OGL_RECT pRectangle, int pTextureId, float pAlpha)
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	
	return manager->DrawImage(pRectangle, pTextureId, pAlpha);
}
extern "C" _declspec(dllexport) int CreateImage(const char* pPath)
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	return manager->CreateImage(pPath);
}

extern "C" _declspec(dllexport) int DeleteResource(int pResourceId)
{
	
    OpenGLManager* manager = OpenGLManager::GetInstance();
	return manager->DeleteResource(pResourceId);
}

extern "C" _declspec(dllexport) int DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth)
{
	OpenGLManager* manager = OpenGLManager::GetInstance();
	return manager->DrawRectangle(pBrush, pRect, pLineWidth);
}