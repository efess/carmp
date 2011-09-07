#pragma once

#include "GLResourceBase.h"
#include "Shader.h"
#include "OGLTexture.h"

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
	
	// Functions for GL to call
	static void Render(void);
	static void OnMouseEvent(int, int, int, int);
	static void OnKeyboardEvent(unsigned char, int, int);
	static void OnMouseMotionEvent(int, int);
	static void OnIdle(void);
	
	bool MakeShaders(void);
public:
	int CreateOGLWindow(OGL_RECT pScreen);

	// Drawing methods:
	int DrawImage(OGL_RECT pRectangle, int pResourceId, float pAlpha);
	int DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth);

	// Resource Creation Methods:
	int CreateImage(const char* pPath);
	int DeleteResource(int pResourceId);


	//void DrawImage(Rectangle pRectangle, IImage pImage, float pAlpha);
	
	void RegisterMouseCallback(void (__stdcall *)(OGL_MOUSE_EVENT));
	void RegisterKeyboardCallback(void (__stdcall *)(OGL_KEYBOARD_EVENT));
	void RegisterRenderCallback(void (__stdcall *)(void));
	
	static OpenGLManager* GetInstance(void);

	void TestFunction(void);
};

