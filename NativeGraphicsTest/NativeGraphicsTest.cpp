// NativeGraphicsTest.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"

#include "..\CarMP_OpenGL\CarMPInterface.h"

void __stdcall render(void);

int _tmain(int argc, _TCHAR* argv[])
{
	OGL_RECT rect;
	rect.x = 0;
	rect.y = 0;
	rect.width = 640;
	rect.height = 480;
	
	CI::CreateOGLWindow(rect);

	while(true)
	{
		render();

		CI::DisplayBuffer();

		Sleep(10);
	}

	return 0;
}

OGLTextLayout* textObject;
OGLTexture* texture[5];

void __stdcall render(void)
{
	OGL_COLOR color1;
	color1.a = 1;
	color1.b = 0;
	color1.g = 1;
	color1.r = 0;

	OGL_COLOR color2;
	color2.a = .3;
	color2.b = 1;
	color2.g = 1;
	color2.r = 1;
	
	OGL_COLOR color3;
	color3.a = .3;
	color3.b = 0;
	color3.g = 0;
	color3.r = 1;

	OGL_RECT rect1;
	rect1.height = 50;
	rect1.width = 50;
	rect1.x = 76;
	rect1.y = 26;
	
	OGL_RECT rect2;
	rect2.height = 50;
	rect2.width = 50;
	rect2.x = 70;
	rect2.y = 20;

	OGL_RECT rect3;
	rect3.height = 50;
	rect3.width = 50;
	rect3.x = 64;
	rect3.y = 14;
	
	OGL_RECT textureRec;
	textureRec.height = 480;
	textureRec.width = 90;
	textureRec.x = 5;
	textureRec.y = 5;

	if(texture[1] == NULL)
		texture[1] = CI::CreateImage("..\\Images\\Skins\\BMW\\NavigationBar.png");

	
	OGL_ELLIPSE ellipse1;
	ellipse1.radius_x = 100;
	ellipse1.radius_y = 125;
	ellipse1.x = 300;
	ellipse1.y = 300;

	CI::FillEllipse(ellipse1, color1);
	CI::DrawEllipse(ellipse1, color2, 5);
	CI::DrawRectangle(color1, rect1, 6);
	CI::DrawRectangle(color2, rect2, 6);
	CI::DrawRectangle(color3, rect3, 6);

	CI::DrawImage(texture[1], textureRec, 1);

	if(textObject == NULL)
		textObject = CI::CreateTextLayout("Testing", "Arial", 15, 0);
	
	OGL_RECT textRec;
	textRec.height = 480;
	textRec.width = 90;
	textRec.x = 200;
	textRec.y = 100;

	CI::DrawTextLayout(textObject, textRec, color3);
}
