
#include "CarMPInterface.h"

extern "C" _declspec(dllexport) int TestFuncion()
{
	
    OpenGLManager* manager = OpenGLManager::GetInstance();
	manager->TestFunction();
	return 1;
}

extern "C" _declspec(dllexport) int CreateWindow()
{
	
	freopen ("error.txt","w",stderr);
  
    OpenGLManager* manager = OpenGLManager::GetInstance();

	int result = 0;
	try
	{
		result = manager->CreateWindow();
	}
	catch(char * err)
	{
		
	}
	fclose (stderr);
	return result;
}

extern "C" _declspec(dllexport) int RegisterMouseCallback(void (__stdcall *pFunction)(MOUSE_EVENT))
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	manager->RegisterMouseCallback(pFunction);
	return 1;
}

extern "C" _declspec(dllexport) int RegisterKeyboardCallback(void (__stdcall *pFunction)(KEYBOARD_EVENT))
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

extern "C" _declspec(dllexport) int DrawImage(int pTextureId, float pAlpha)
{
    OpenGLManager* manager = OpenGLManager::GetInstance();
	GLRECT lol = GLRECT();

	return manager->DrawImage(lol, pTextureId, pAlpha);
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