#pragma once

#include "GLResourceBase.h"
#include "Shader.h"
#include "OGLTexture.h"	
#include "OGLTextLayout.h"	

#include <map>

// End OpenGL Includes

using namespace std;

typedef struct OGL_MOUSE_EVENT {
	float x;
	float y;
};

typedef struct OGL_KEYBOARD_EVENT {
	unsigned char c;
};


class OpenGLManager
{
private:
	void (__stdcall *m_mouseHandler)(OGL_MOUSE_EVENT);
	void (__stdcall *m_keyboardHandler)(OGL_KEYBOARD_EVENT);
	void (__stdcall *m_renderHandler)(void);
	map<int, GLResourceBase*> _resourceMap;

	static OpenGLManager* _instance;
    const OpenGLManager& operator=(const OpenGLManager& a);

	OpenGLManager(void);
    OpenGLManager(OpenGLManager const&);              // Don't Implement
//    void operator=(OpenGLManager const&); // Don't implement
	~OpenGLManager(void);
	
	void MainLoop();
	sf::RenderWindow* renderer;
	// Functions for GL to call
	static void Render(void);
	static void OnMouseEvent(int, int, int, int);
	static void OnKeyboardEvent(unsigned char, int, int);
	static void OnMouseMotionEvent(int, int);
	
	
public:
	void CreateOGLWindow(OGL_RECT pScreen);

	// Drawing methods:
	void DrawImage(OGLTexture* pTexture, OGL_RECT pRectangle, float pAlpha);
	void DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth);
	void FillRectangle(OGL_COLOR, OGL_RECT);
	void DrawEllipse(OGL_ELLIPSE, OGL_COLOR, float pLineWidth);
	void DrawLine(OGL_POINT pPoint1, OGL_POINT pPoint2, OGL_COLOR pBrush, float pLineWidth);
	void FillEllipse(OGL_ELLIPSE, OGL_COLOR);
	void DrawText(OGLTextLayout* pTextLayout, OGL_RECT pRectangle, OGL_COLOR pColor);
	void Clear(OGL_COLOR pBrush);

	// Resource Methods:
	OGLTextLayout* OpenGLManager::CreateTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment);
	void OpenGLManager::FreeTextLayout(OGLTextLayout*);
	OGLTexture* CreateImage(const char* pPath);
	OGLTexture* CreateImageFromByteArray(const char* pByteArray, int pStride);
	void FreeImage(OGLTexture* pTexture);
	
	//void DrawImage(Rectangle pRectangle, IImage pImage, float pAlpha);
	
	void RegisterMouseCallback(void (__stdcall *)(OGL_MOUSE_EVENT));
	void RegisterKeyboardCallback(void (__stdcall *)(OGL_KEYBOARD_EVENT));
	void RegisterRenderCallback(void (__stdcall *)(void));
	
	static OpenGLManager* GetInstance(void);

	void TestFunction(void);
};

