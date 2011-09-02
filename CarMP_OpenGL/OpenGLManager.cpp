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

int OpenGLManager::CreateWindow(void)
{
	// "Main()" Arguments
	int argc = 1;
	char* argv[1] = {"nothing"};	

    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_RGB | GLUT_DOUBLE);
    glutInitWindowSize(400, 300);

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

	if (!_instance->MakeShaders()) {
        fprintf(stderr, "Failed to load resources\n");
        return 1;
    }


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

	MOUSE_EVENT newEvent;
	newEvent.x = x;
	newEvent.y = y;

	_instance->m_mouseHandler(newEvent);
}

void OpenGLManager::OnMouseMotionEvent(int x, int y)
{	
	if(_instance->m_mouseHandler == NULL)
		return;

	MOUSE_EVENT newEvent;
	newEvent.x = x;
	newEvent.y = y;

	_instance->m_mouseHandler(newEvent);
}

void OpenGLManager::OnKeyboardEvent(unsigned char key,
                                   int x, int y)
{
	if(_instance->m_keyboardHandler == NULL)
		return;

	KEYBOARD_EVENT newEvent;
	newEvent.c = key;

	_instance->m_keyboardHandler(newEvent);
}

void OpenGLManager::RegisterMouseCallback(void (__stdcall *phandler)(MOUSE_EVENT))
{	
	m_mouseHandler = phandler;
}

void OpenGLManager::RegisterKeyboardCallback(void (__stdcall *phandler)(KEYBOARD_EVENT))
{	
	m_keyboardHandler = phandler;
}

void OpenGLManager::RegisterRenderCallback(void (__stdcall *pHandler)(void))
{
	m_renderHandler = pHandler;
}

void OpenGLManager::TestFunction()
{
	if(_resourceMap.size() > 0)
	{
		GLRECT lol;
		
		GLResourceBase * resource = _resourceMap.begin()->second;;
		Texture * texture = static_cast<Texture *>(resource);
		DrawImage(lol, texture->GetTextureId(), 1);
	}
}

int OpenGLManager::DrawImage(GLRECT pRectangle, int pResourceId, float pAlpha)
{
	if(_resourceMap.count(pResourceId) > 0)
	{
		map<int, GLResourceBase*>::iterator item = _resourceMap.find(pResourceId);
		
		GLResourceBase * resource = item->second;
		Texture * texture = static_cast<Texture *>(resource);
		texture->Draw();
	}
	return 1;
}

int OpenGLManager::CreateImage(const char* pPath)
{
	Texture* resource = new Texture(pPath);
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
