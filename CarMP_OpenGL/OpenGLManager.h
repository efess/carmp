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

class OpenGLManager
{
private:
	void (__stdcall *m_mouseMoveHandler)(sf::Event::MouseMoveEvent);
	void (__stdcall *m_mouseUpHandler)(sf::Event::MouseButtonEvent);
	void (__stdcall *m_mouseDownHandler)(sf::Event::MouseButtonEvent);
	void (__stdcall *m_keyboardHandler)(sf::Event::KeyEvent);
	void (__stdcall *m_windowCloseHandler)(void);

	static OpenGLManager* _instance;
    const OpenGLManager& operator=(const OpenGLManager& a);

	OpenGLManager(void);
    OpenGLManager(OpenGLManager const&);              // Don't Implement
//    void operator=(OpenGLManager const&); // Don't implement
	~OpenGLManager(void);
	
	sf::RenderWindow* renderer;
	sf::Thread* eventThread;

	void ProcessEventThread(void);
	bool m_shutDownThread;
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

	void DisplayBuffer(void);

	// Resource Methods:
	OGLTextLayout* OpenGLManager::CreateTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment);
	void OpenGLManager::FreeTextLayout(OGLTextLayout*);
	OGLTexture* CreateImage(const char* pPath);
	OGLTexture* CreateImageFromByteArray(const char* pByteArray, int pStride);
	void FreeImage(OGLTexture* pTexture);
	
	//void DrawImage(Rectangle pRectangle, IImage pImage, float pAlpha);
	
	void RegisterMouseMoveCallback(void (__stdcall *)(sf::Event::MouseMoveEvent));
	void RegisterMouseDownCallback(void (__stdcall *)(sf::Event::MouseButtonEvent));
	void RegisterMouseUpCallback(void (__stdcall *)(sf::Event::MouseButtonEvent));
	void RegisterKeyboardCallback(void (__stdcall *)(sf::Event::KeyEvent));
	void RegisterWindowCloseCallback(void (__stdcall *)(void));

	static OpenGLManager* GetInstance(void);

	void TestFunction(void);

	void PushClip(OGL_RECT);
	void PopClip();
};

