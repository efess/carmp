#pragma once

#include "OpenGLManager.h"
namespace CI
{
	extern "C" _declspec(dllexport) int RegisterKeyboardCallback(void (__stdcall *pFunction)(OGL_KEYBOARD_EVENT));
	extern "C" _declspec(dllexport) int RegisterMouseCallback(void (__stdcall *pFunction)(OGL_MOUSE_EVENT));
	extern "C" _declspec(dllexport) int RegisterRenderCallback(void (__stdcall *pFunction)(void));

	extern "C" _declspec(dllexport) void CreateOGLWindow(OGL_RECT pRectangle);

	// Drawing stuff
	extern "C" _declspec(dllexport) void DrawImage(OGLTexture*, OGL_RECT, float);
	extern "C" _declspec(dllexport) void DrawImage(OGLTexture*, OGL_RECT, float);
	extern "C" _declspec(dllexport) void DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth);
	extern "C" _declspec(dllexport) void DrawText(OGLTextLayout* pTextLayout, OGL_RECT pRectangle, OGL_COLOR pColor);

	extern "C" _declspec(dllexport) void DrawLine(OGL_POINT pPoint1, OGL_POINT pPoint2, OGL_COLOR pBrush, float pLineWidth);
	extern "C" _declspec(dllexport) void FillEllipse(OGL_ELLIPSE, OGL_COLOR);
	extern "C" _declspec(dllexport) void FillRectangle(OGL_COLOR, OGL_RECT);
	extern "C" _declspec(dllexport) void DrawEllipse(OGL_ELLIPSE, OGL_COLOR, float pLineWidth);

	extern "C" _declspec(dllexport) void Clear(OGL_COLOR pBrush);

	// Resource Stuff
	extern "C" _declspec(dllexport) OGLTexture* CreateImage(const char*);
	extern "C" _declspec(dllexport) OGLTexture* CreateImageFromByteArray(const char*, int pStride);
	extern "C" _declspec(dllexport) void FreeImage(OGLTexture*);

	extern "C" _declspec(dllexport) OGLTextLayout* CreateTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment);
	extern "C" _declspec(dllexport) void FreeTextLayout(OGLTextLayout* pTextLayout);


};