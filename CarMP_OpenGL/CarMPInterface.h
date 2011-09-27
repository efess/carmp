#pragma once


#include "OpenGLManager.h"

namespace CI
{
	extern "C" _declspec(dllexport) void DumpDebugInfo();
	extern "C" _declspec(dllexport) void RegisterKeyboardCallback(void (__stdcall *pFunction)(sf::Event::KeyEvent));
	extern "C" _declspec(dllexport) void RegisterMouseMoveCallback(void (__stdcall *pFunction)(sf::Event::MouseMoveEvent));
	extern "C" _declspec(dllexport) void RegisterMouseUpCallback(void (__stdcall *pFunction)(sf::Event::MouseButtonEvent));
	extern "C" _declspec(dllexport) void RegisterMouseDownCallback(void (__stdcall *pFunction)(sf::Event::MouseButtonEvent));
	extern "C" _declspec(dllexport) void RegisterWindowCloseCallback(void (__stdcall *pFunction)(void));
	
	extern "C" _declspec(dllexport) void CreateOGLWindow(const OGL_RECT& pRectangle);

	extern "C" _declspec(dllexport) void DisplayBuffer();

	extern "C" _declspec(dllexport) void PushClip(const OGL_RECT&);
	extern "C" _declspec(dllexport) void PopClip();

	// Drawing stuff
	extern "C" _declspec(dllexport) void DrawImage(OGLTexture*, const OGL_RECT&, float);
	extern "C" _declspec(dllexport) void DrawRectangle(const OGL_COLOR& pBrush, const OGL_RECT& pRect, float pLineWidth);
	extern "C" _declspec(dllexport) void DrawTextLayout(OGLTextLayout* pTextLayout, const OGL_RECT& pRectangle, const OGL_COLOR& pColor);

	extern "C" _declspec(dllexport) void DrawLine(const OGL_POINT& pPoint1, const OGL_POINT& pPoint2, const OGL_COLOR& pBrush, float pLineWidth);
	extern "C" _declspec(dllexport) void FillEllipse(const OGL_ELLIPSE&, const OGL_COLOR&);
	extern "C" _declspec(dllexport) void FillRectangle(const OGL_COLOR&, const OGL_RECT&);
	extern "C" _declspec(dllexport) void DrawEllipse(const OGL_ELLIPSE&, const OGL_COLOR&, float pLineWidth);

	extern "C" _declspec(dllexport) void Clear(const OGL_COLOR& pBrush);

	// Resource Stuff
	extern "C" _declspec(dllexport) OGLTexture* CreateImage(const char*);
	extern "C" _declspec(dllexport) OGLTexture* CreateImageFromByteArray(const char*, int pStride);
	extern "C" _declspec(dllexport) void FreeImage(OGLTexture*);

	extern "C" _declspec(dllexport) OGLTextLayout* CreateTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment);
	extern "C" _declspec(dllexport) void FreeTextLayout(OGLTextLayout* pTextLayout);
};