#include "iostream"
#include "OpenGLManager.h"

OpenGLManager* OpenGLManager::_instance = NULL;

OpenGLManager::OpenGLManager(void)
{
	m_mouseHandler = NULL;
	m_keyboardHandler = NULL;
}

OpenGLManager::~OpenGLManager(void)
{
	// iterate and free resources
	/*for(int i = 0; i < _resourceMap.count; i++)
		_resourceMap.A*/
}


int OpenGLManager::CreateOGLWindow(OGL_RECT pRectangle)
{
	// "Main()" Arguments
	int argc = 1;
	char* argv[1] = {"nothing"};	

    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_ALPHA);
	glutInitWindowPosition(pRectangle.x, pRectangle.y);
    glutInitWindowSize(pRectangle.width, pRectangle.height);

    glutCreateWindow("OPENGL WINDOW");
 
	glutDisplayFunc(&Render);
	glutMouseFunc(&OnMouseEvent);
	glutKeyboardFunc(&OnKeyboardEvent);
	glutPassiveMotionFunc(&OnMouseMotionEvent);
	glutMotionFunc(&OnMouseMotionEvent);
	glutIdleFunc(&OnIdle);

	glewInit();
    if (!GLEW_VERSION_2_0) {
        //fprintf(stderr, "OpenGL 2.0 not available\n");
        return 1;
    }
	
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	

	GLint viewPort[4];
	glGetIntegerv(GL_VIEWPORT, viewPort);
	glOrtho(viewPort[0],viewPort[0]+viewPort[2],viewPort[1]+viewPort[3],viewPort[1],-1,1);
		

	//glOrtho(0, pRectangle.width, pRectangle.width, 0, -1, 1);

	//Back to the modelview so we can draw stuff 
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	//glTranslatef(0.375, 0.375, 0);
	
	glEnable(GL_BLEND);
	glDisable(GL_DEPTH_TEST);
	glDisable(GL_LIGHTING);
/*
	if (!_instance->MakeShaders()) {
        fprintf(stderr, "Failed to load resources\n");
        return 1;
    }*/
	   //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	//glBlendFunc(GL_ONE_MINUS_DST_ALPHA,GL_DST_ALPHA);

    glutMainLoop();
	return 0;
}

void OpenGLManager::OnIdle(void)
{
	glutPostRedisplay();
}

void OpenGLManager::Render(void)
{
    glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
    //glClear(GL_COLOR_BUFFER_BIT);
	
	if(_instance->m_renderHandler != NULL)
		_instance->m_renderHandler();


	//TEMP
    glutSwapBuffers();


}

//void OpenGLManager::DrawImage(Rectangle pRectangle, int pTextureId, float pAlpha)
//{
//	
//}

void OpenGLManager::OnMouseEvent(int button, int state,
                                int x, int y)
{
	if(_instance->m_mouseHandler == NULL)
		return;

	OGL_MOUSE_EVENT newEvent;
	newEvent.x = x;
	newEvent.y = y;

	_instance->m_mouseHandler(newEvent);
}

void OpenGLManager::OnMouseMotionEvent(int x, int y)
{	
	if(_instance->m_mouseHandler == NULL)
		return;

	OGL_MOUSE_EVENT newEvent;
	newEvent.x = x;
	newEvent.y = y;

	_instance->m_mouseHandler(newEvent);
}

void OpenGLManager::OnKeyboardEvent(unsigned char key,
                                   int x, int y)
{
	if(_instance->m_keyboardHandler == NULL)
		return;

	OGL_KEYBOARD_EVENT newEvent;
	newEvent.c = key;

	_instance->m_keyboardHandler(newEvent);
}

void OpenGLManager::RegisterMouseCallback(void (__stdcall *phandler)(OGL_MOUSE_EVENT))
{	
	m_mouseHandler = phandler;
}

void OpenGLManager::RegisterKeyboardCallback(void (__stdcall *phandler)(OGL_KEYBOARD_EVENT))
{	
	m_keyboardHandler = phandler;
}

void OpenGLManager::RegisterRenderCallback(void (__stdcall *pHandler)(void))
{
	m_renderHandler = pHandler;
}

int OpenGLManager::DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth)
{
	glEnable(GL_BLEND);
	
	float x2 = pRect.x + pRect.width;
	float y2 = pRect.y + pRect.height;
	float extend = pLineWidth / 2;
	
	glLineWidth(pLineWidth);
	
	glBegin(GL_LINES);   //We want to draw a quad, i.e. shape with four sides
	glColor4f(pBrush.r, pBrush.g, pBrush.b, pBrush.a); //Set the colour to red 

	glVertex2f(pRect.x - extend, pRect.y); 
	glVertex2f(x2 + extend, pRect.y); 

	glVertex2f(x2, pRect.y + extend); 
	glVertex2f(x2, y2 - extend); 

	glVertex2f(x2 + extend, y2); 
	glVertex2f(pRect.x - extend, y2);
	
	glVertex2f(pRect.x, y2 - extend);
	glVertex2f(pRect.x, pRect.y + extend); 

	glEnd();
	//glPopMatrix();
	glDisable(GL_BLEND);
	return 1;
}

int OpenGLManager::DrawImage(OGL_RECT pRectangle, int pResourceId, float pAlpha)
{
	if(_resourceMap.count(pResourceId) > 0)
	{
		map<int, GLResourceBase*>::iterator item = _resourceMap.find(pResourceId);
		
		GLResourceBase * resource = item->second;
		OGLTexture * texture = static_cast<OGLTexture *>(resource);
		
		texture->SetDimensions(pRectangle);
		resource->Draw();
	}
	return 1;
}

int OpenGLManager::CreateImage(const char* pPath)
{
	OGLTexture* resource = new OGLTexture(pPath);
	GLint textureId = resource->GetTextureId();

	_resourceMap[textureId] = resource;
	
	return textureId;
}

int OpenGLManager::DeleteResource(int pResourceId)
{
	return 0;
}

OpenGLManager* OpenGLManager::GetInstance(void)
{
	if(_instance == NULL)
	{
		_instance = new OpenGLManager();
		//static OpenGLManager self;
		//_instance = &self;
	}
	return _instance;
}
