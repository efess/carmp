#include "iostream"
#include "OpenGLManager.h"

OpenGLManager* OpenGLManager::_instance = NULL;

OpenGLManager::OpenGLManager(void)
{
	m_shutDownThread = false;

	m_mouseDownHandler = NULL;
	m_mouseUpHandler = NULL;
	m_windowCloseHandler = NULL;
	m_mouseMoveHandler = NULL;
	m_keyboardHandler = NULL;
}

OpenGLManager::~OpenGLManager(void)
{
	m_shutDownThread = true;
}

void OpenGLManager::CreateOGLWindow(OGL_RECT pRectangle)
{
	// Initialize SFML window and thread
	//sf::RenderWindow renderer_temp(
	
	//renderer = &renderer_temp; */
	renderer = new sf::RenderWindow(sf::VideoMode(pRectangle.width, pRectangle.height, 8), "CarMP OpenGL Window");
	
	
	//eventThread = new sf::Thread(&OpenGLManager::ProcessEventThread, _instance);
	
	//eventThread->Launch();
}

void OpenGLManager::DisplayBuffer()
{
	renderer->Display();
	OpenGLManager::ProcessEventThread();
}

void OpenGLManager::ProcessEventThread()
{
	//while(!m_shutDownThread && renderer->IsOpened())
	{		
		sf::Event newEvent;
		while(renderer->PollEvent(newEvent))
		{
			switch(newEvent.Type)
			{
				case sf::Event::KeyPressed:
					
					if(m_keyboardHandler != NULL)
						m_keyboardHandler(newEvent.Key);
					break;
				case sf::Event::Closed:
					// Need to send a message up to c# land too...
					if(m_windowCloseHandler != NULL)
						m_windowCloseHandler();
					renderer->Close();
					break;
				case sf::Event::MouseMoved:
					if(m_mouseMoveHandler != NULL)
						m_mouseMoveHandler(newEvent.MouseMove);
					break;
				case sf::Event::MouseButtonPressed:
					if(m_mouseDownHandler != NULL)
						m_mouseDownHandler(newEvent.MouseButton);
					break;
				case sf::Event::MouseButtonReleased:
					if(m_mouseUpHandler != NULL)
						m_mouseUpHandler(newEvent.MouseButton);
					break;
			}
		}
		//Sleep(10);
	}
}

void OpenGLManager::RegisterMouseMoveCallback(void (__stdcall *phandler)(sf::Event::MouseMoveEvent))
{	
	m_mouseMoveHandler = phandler;
}

void OpenGLManager::RegisterMouseUpCallback(void (__stdcall *phandler)(sf::Event::MouseButtonEvent))
{	
	m_mouseUpHandler = phandler;
}

void OpenGLManager::RegisterMouseDownCallback(void (__stdcall *phandler)(sf::Event::MouseButtonEvent))
{	
	m_mouseDownHandler = phandler;
}

void OpenGLManager::RegisterKeyboardCallback(void (__stdcall *phandler)(sf::Event::KeyEvent))
{	
	m_keyboardHandler = phandler;
}

void OpenGLManager::RegisterWindowCloseCallback(void (__stdcall *phandler)())
{	
	m_windowCloseHandler = phandler;
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

void OpenGLManager::PushClip(OGL_RECT pBoundingRectangle)
{
}
void OpenGLManager::PopClip()
{
}