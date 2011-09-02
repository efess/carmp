#pragma once

#include "GLResourceBase.h"
#include "Shader.h"
#include "Texture.h"

#include <map>

// OpenGL Includes

#include <stdlib.h>
#include <GL/glew.h>
#ifdef __APPLE__
#  include <GLUT/glut.h>
#else
#  include <GL/glut.h>
#endif
// End OpenGL Includes

using namespace std;

typedef struct MOUSE_EVENT {
	float x;
	float y;
};

typedef struct KEYBOARD_EVENT {
	unsigned char c;
};

// Bounds are from 0 - 1
typedef struct GLRECT {
	float x;
	float y;
	float right;
	float bottom;
};

class OpenGLManager
{
private:
	void (__stdcall *m_mouseHandler)(MOUSE_EVENT);
	void (__stdcall *m_keyboardHandler)(KEYBOARD_EVENT);
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
	int CreateWindow(void);

	// Drawing methods:
	int DrawImage(GLRECT pRectangle, int pResourceId, float pAlpha);

	// Resource Creation Methods:
	int CreateImage(const char* pPath);
	int DeleteResource(int pResourceId);

	//void DrawImage(Rectangle pRectangle, IImage pImage, float pAlpha);
	
	void RegisterMouseCallback(void (__stdcall *)(MOUSE_EVENT));
	void RegisterKeyboardCallback(void (__stdcall *)(KEYBOARD_EVENT));
	void RegisterRenderCallback(void (__stdcall *)(void));
	
	static OpenGLManager* GetInstance(void);

	void TestFunction(void);
};

