
#include "CarMPInterface.h"
namespace CI
{
	extern "C" _declspec(dllexport) void CreateOGLWindow(OGL_RECT pRectangle)
	{
	
		freopen ("error.txt","w",stderr);
  
		OpenGLManager* manager = OpenGLManager::GetInstance();

		try
		{
			manager->CreateOGLWindow(pRectangle);
		}
		catch(char * err)
		{
		
		}
		fclose (stderr);
	}

	extern "C" _declspec(dllexport) void DisplayBuffer()
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->DisplayBuffer();
	}

	extern "C" _declspec(dllexport) void RegisterWindowCloseCallback(void (__stdcall *pFunction)())
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->RegisterWindowCloseCallback(pFunction);
	}

	extern "C" _declspec(dllexport) void RegisterMouseMoveCallback(void (__stdcall *pFunction)(sf::Event::MouseMoveEvent))
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->RegisterMouseMoveCallback(pFunction);
	}

	extern "C" _declspec(dllexport) void RegisterKeyboardCallback(void (__stdcall *pFunction)(sf::Event::KeyEvent))
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->RegisterKeyboardCallback(pFunction);
	}
	
	extern "C" _declspec(dllexport) void RegisterMouseUpCallback(void (__stdcall *pFunction)(sf::Event::MouseButtonEvent))
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->RegisterMouseUpCallback(pFunction);
	}

	extern "C" _declspec(dllexport) void RegisterMouseDownCallback(void (__stdcall *pFunction)(sf::Event::MouseButtonEvent))
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->RegisterMouseDownCallback(pFunction);
	}

	extern "C" _declspec(dllexport) void DrawImage(OGLTexture* pTexture, OGL_RECT pRectangle, float pAlpha)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
	
		manager->DrawImage(pTexture, pRectangle, pAlpha);
	}

	extern "C" _declspec(dllexport) void DrawTextLayout(OGLTextLayout* pTextLayout, OGL_RECT pRectangle, OGL_COLOR pColor)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
	
		manager->DrawText(pTextLayout, pRectangle, pColor);
	}

	extern "C" _declspec(dllexport) OGLTexture* CreateImage(const char* pPath)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		return manager->CreateImage(pPath);
	}
	
	extern "C" _declspec(dllexport) OGLTexture* CreateImageFromByteArray(const char* pByteArray, int pStride)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		return manager->CreateImageFromByteArray(pByteArray, pStride);
	}
	
	extern "C" _declspec(dllexport) void FreeImage(OGLTexture* pTexture)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->FreeImage(pTexture);
	}
	extern "C" _declspec(dllexport) OGLTextLayout* CreateTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		return manager->CreateTextLayout(pString, pFont, pSize, pAlignment);
	}
	extern "C" _declspec(dllexport) void FreeTextLayout(OGLTextLayout* pTextLayout)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->FreeTextLayout(pTextLayout);
	}

	extern "C" _declspec(dllexport) void DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->DrawRectangle(pBrush, pRect, pLineWidth);
	}

	extern "C" _declspec(dllexport) void FillRectangle(OGL_COLOR pBrush, OGL_RECT pRect)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->FillRectangle(pBrush, pRect);
	}
	
	extern "C" _declspec(dllexport) void DrawEllipse(OGL_ELLIPSE pEllipse, OGL_COLOR pBrush, float pLineWidth)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->DrawEllipse(pEllipse, pBrush, pLineWidth);
	}
	
	extern "C" _declspec(dllexport) void FillEllipse(OGL_ELLIPSE pEllipse, OGL_COLOR pBrush)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->FillEllipse(pEllipse, pBrush);
	}
	
	extern "C" _declspec(dllexport) void DrawLine(OGL_POINT pPoint1, OGL_POINT pPoint2, OGL_COLOR pBrush, float pWidth)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->DrawLine(pPoint1, pPoint2, pBrush, pWidth);
	}

	extern "C" _declspec(dllexport) void Clear(OGL_COLOR pBrush)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->Clear(pBrush);
	}
	extern "C" _declspec(dllexport) void PushClip(OGL_RECT pBoundingRectangle)
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->PushClip(pBoundingRectangle);
	}
	extern "C" _declspec(dllexport) void PopClip()
	{
		OpenGLManager* manager = OpenGLManager::GetInstance();
		manager->PopClip();
	}
};