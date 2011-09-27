#pragma once

//#include "vld.h"
#include "GLResourceBase.h"
#include "Shader.h"
#include "OGLTexture.h"	
#include "OGLTextLayout.h"	

#include <stack>
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
	
	stack<OGL_RECT> clippingStack;
	sf::RenderWindow* renderer;
	sf::Thread* eventThread;

	OGL_RECT m_currentWindowBounds;

	void ApplyGlScissor(OGL_RECT);
	void ProcessEventThread(void);
	bool m_shutDownThread;
public:
	void CreateOGLWindow(const OGL_RECT& pScreen);

	// Drawing methods:
	void DrawImage(OGLTexture* pTexture, const OGL_RECT& pRectangle, float pAlpha);
	void DrawRectangle(const OGL_COLOR& pBrush, const OGL_RECT& pRect, float pLineWidth);
	void FillRectangle(const OGL_COLOR&, const OGL_RECT&);
	void DrawEllipse(const OGL_ELLIPSE&, const OGL_COLOR&, float pLineWidth);
	void DrawLine(const OGL_POINT& pPoint1, const OGL_POINT& pPoint2, const OGL_COLOR& pBrush, float pLineWidth);
	void FillEllipse(const OGL_ELLIPSE&, const OGL_COLOR&);
	void DrawText(OGLTextLayout* pTextLayout, const OGL_RECT& pRectangle, const OGL_COLOR& pColor);
	void Clear(const OGL_COLOR& pBrush);

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

