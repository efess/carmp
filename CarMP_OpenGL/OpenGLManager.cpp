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
}

int OpenGLManager::CreateOGLWindow(OGL_RECT pRectangle)
{
	// "Main()" Arguments
	int argc = 1;
	char* argv[1] = {"nothing"};	

    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_RGBA | GLUT_DOUBLE);
	glutInitWindowPosition(pRectangle.x, pRectangle.y);
    glutInitWindowSize(pRectangle.width, pRectangle.height);

    glutCreateWindow("Hello World");
 
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
	
	glClearColor(0.0, 0.0, 0.0, 0.0);  //Set the cleared screen colour to black
	glViewport(0, 0, pRectangle.width, pRectangle.width);   //This sets up the viewport so that the coordinates (0, 0) are at the top left of the window

	
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	glOrtho(0, pRectangle.width, pRectangle.width, 0, -10, 10);

	//Back to the modelview so we can draw stuff 
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	//glTranslatef(0.375, 0.375, 0);
	
/*
	if (!_instance->MakeShaders()) {
        fprintf(stderr, "Failed to load resources\n");
        return 1;
    }*/


    glutMainLoop();
	return 0;
}



///////////////////// C /////////////////////////////

bool OpenGLManager::MakeShaders(void)
{
	Shader* vertexShader = new Shader(
        GL_VERTEX_SHADER,
        "CarMP_OpenGL.v.glsl"
    );

    Shader* fragmentShader = new Shader(
        GL_FRAGMENT_SHADER,
        "CarMP_OpenGL.f.glsl"
    );

	// link shader
	GLint program_ok;
	
	GLuint program = glCreateProgram();
    glAttachShader(program, vertexShader->GetId());
    glAttachShader(program, fragmentShader->GetId());
    glLinkProgram(program);

	glGetProgramiv(program, GL_LINK_STATUS, &program_ok);
    if (!program_ok) {
        fprintf(stderr, "Failed to link shader program:\n");
        Utils::ShowInfoLog(program, glGetProgramiv, glGetProgramInfoLog);
        glDeleteProgram(program);
        return 0;
    }
    return program;

	return 1;
}

void OpenGLManager::OnIdle(void)
{
	glutPostRedisplay();
}

void OpenGLManager::Render(void)
{
    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT);
	
	if(_instance->m_renderHandler != NULL)
		_instance->m_renderHandler();

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

	float x2 = pRect.x + pRect.width;
	float y2 = pRect.y + pRect.height;
	float extend = pLineWidth / 2;
	
	glLineWidth(pLineWidth);
	
	glBegin(GL_LINES);   //We want to draw a quad, i.e. shape with four sides
	glColor4f(pBrush.r, pBrush.g, pBrush.b, pBrush.a); //Set the colour to red 

	glVertex2f(pRect.x - extend, pRect.y); 
	glVertex2f(x2 + extend, pRect.y); 

	glVertex2f(x2, pRect.y); 
	glVertex2f(x2, y2); 

	glVertex2f(x2 + extend, y2); 
	glVertex2f(pRect.x - extend, y2);
	
	glVertex2f(pRect.x, y2);
	glVertex2f(pRect.x, pRect.y); 

	glEnd();
	//glPopMatrix();

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
