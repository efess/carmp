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


void OpenGLManager::CreateOGLWindow(OGL_RECT pRectangle)
{
	// "Main()" Arguments
	int argc = 1;
	char* argv[1] = {"nothing"};	

	sf::RenderWindow renderer_temp(sf::VideoMode(pRectangle.width, pRectangle.height, 8), "SFML Graphics");
	renderer = &renderer_temp; 

	//OGLTextLayout layout("Hello World", "arial", 12, 1);

	while (renderer->IsOpened())
    {
        // Process events
        sf::Event Event;
        while (renderer->PollEvent(Event))
        {
            // Close window : exit
            if (Event.Type == sf::Event::Closed)
                renderer->Close();
        }

        // Clear the screen (fill it with black color)
        renderer->Clear();

		OpenGLManager::MainLoop();

		//layout.Draw(renderer);

        // Display window contents on screen
        renderer->Display();
    }
    /*glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_ALPHA);
	glutInitWindowPosition(pRectangle.x, pRectangle.y);
    glutInitWindowSize(pRectangle.width, pRectangle.height);

    glutCreateWindow("OPENGL WINDOW");
 
	glutDisplayFunc(&Render);
	glutMouseFunc(&OnMouseEvent);
	glutKeyboardFunc(&OnKeyboardEvent);
	glutPassiveMotionFunc(&OnMouseMotionEvent);
	glutMotionFunc(&OnMouseMotionEvent);
	glutIdleFunc(&OnIdle);*/

	//glewInit();
 //   if (!GLEW_VERSION_2_0) {
 //       //fprintf(stderr, "OpenGL 2.0 not available\n");
 //       return 1;
 //   }
	
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
	if (!_instance->MakeShaders()) {x
        fprintf(stderr, "Failed to load resources\n");
        return 1;
    }*/
	   //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	
	renderer->Clear(sf::Color(0,0,0,1));

    OpenGLManager::MainLoop();
	
}

void OpenGLManager::MainLoop()
{
    /*while(renderer.IsOpened())
    {*/
		
		glClearColor(0.f, 0.f, 0.f, 0.f);
		
        // Clear color and depth buffer
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		
		if(_instance->m_renderHandler != NULL)
		_instance->m_renderHandler();

		//renderer.Display();
		
		sf::Sleep(.01);
	//}
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

void OpenGLManager::Clear(OGL_COLOR pBrush)
{
	renderer->Clear(sf::Color(pBrush.r,pBrush.g, pBrush.b, pBrush.a));
}

void OpenGLManager::DrawLine(OGL_POINT pPoint1, OGL_POINT pPoint2, OGL_COLOR pBrush, float pLineWidth)
{
	sf::Color color = sf::Color((sf::Uint8)(pBrush.r * 255), (sf::Uint8)(pBrush.g * 255), (sf::Uint8)(pBrush.b * 255),(sf::Uint8)(pBrush.a * 255));
	sf::Shape shape = sf::Shape::Line(pPoint1.x,pPoint1.y, pPoint2.x, pPoint2.y, pLineWidth, color);
	
	renderer->Draw(shape);
}

void OpenGLManager::DrawRectangle(OGL_COLOR pBrush, OGL_RECT pRect, float pLineWidth)
{
	sf::Color color = sf::Color((sf::Uint8)(pBrush.r * 255), (sf::Uint8)(pBrush.g * 255), (sf::Uint8)(pBrush.b * 255),(sf::Uint8)(pBrush.a * 255));
	sf::Shape shape = sf::Shape::Rectangle(pRect.x,pRect.y, pRect.width, pRect.height, color, pLineWidth, color);
	shape.EnableFill(false);
	renderer->Draw(shape);
}

void OpenGLManager::FillRectangle(OGL_COLOR pBrush, OGL_RECT pRect)
{
	sf::Color color = sf::Color((sf::Uint8)(pBrush.r * 255), (sf::Uint8)(pBrush.g * 255), (sf::Uint8)(pBrush.b * 255),(sf::Uint8)(pBrush.a * 255));
	sf::Shape shape = sf::Shape::Rectangle(pRect.x,pRect.y, pRect.width, pRect.height, color, 0, color);
	shape.EnableFill(true);
	renderer->Draw(shape);
}

void OpenGLManager::DrawEllipse(OGL_ELLIPSE pEllipse, OGL_COLOR pBrush, float pLineWidth)
{
	sf::Color color = sf::Color((sf::Uint8)(pBrush.r * 255), (sf::Uint8)(pBrush.g * 255), (sf::Uint8)(pBrush.b * 255),(sf::Uint8)(pBrush.a * 255));
	sf::Shape shape = sf::Shape::Circle(pEllipse.x, pEllipse.y, pEllipse.radius_x, color, pLineWidth, color);
	
	float scale = pEllipse.radius_x / pEllipse.radius_y;

	shape.SetScale(1, scale);
	shape.EnableFill(false);

	renderer->Draw(shape);
}

void OpenGLManager::FillEllipse(OGL_ELLIPSE pEllipse, OGL_COLOR pBrush)
{
	sf::Color color = sf::Color((sf::Uint8)(pBrush.r * 255), (sf::Uint8)(pBrush.g * 255), (sf::Uint8)(pBrush.b * 255),(sf::Uint8)(pBrush.a * 255));
	sf::Shape shape = sf::Shape::Circle(pEllipse.x, pEllipse.y, pEllipse.radius_x, color);
	
	float scale = pEllipse.radius_x / pEllipse.radius_y;

	shape.SetScale(1, scale);
	shape.EnableFill(true);

	renderer->Draw(shape);
}

void OpenGLManager::DrawImage(OGLTexture* pTexture, OGL_RECT pRectangle, float pAlpha)
{
	pTexture->SetDimensions(pRectangle);
	pTexture->Draw(renderer);
}

void OpenGLManager::DrawText(OGLTextLayout* pTextLayout, OGL_RECT pRectangle, OGL_COLOR pColor)
{
	pTextLayout->SetDimensions(pRectangle);
	pTextLayout->Draw(renderer);
}


OGLTexture* OpenGLManager::CreateImageFromByteArray(const char* pByteArray, int pStride)
{
	OGLTexture* resource = new OGLTexture(pByteArray, pStride);
	
	return resource;
}

OGLTexture* OpenGLManager::CreateImage(const char* pPath)
{
	OGLTexture* resource = new OGLTexture(pPath);
	
	return resource;
}

OGLTextLayout* OpenGLManager::CreateTextLayout(const char* pString, const char* pFont, float pSize, int pAlignment)
{
	OGLTextLayout* textLayout = new OGLTextLayout(pString, pFont, pSize, pAlignment);
	return textLayout;
}

void OpenGLManager::FreeTextLayout(OGLTextLayout* pTextLayout)
{
	free(pTextLayout);
}

void OpenGLManager::FreeImage(OGLTexture* pTexture)
{
	free(pTexture);
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