#pragma once

////////////////////////////////////////////////////////

#include "Configuration_ProxxonMF70.h"

////////////////////////////////////////////////////////

#include <LCD.h>
#include "RotaryButton.h"

#define ROTARY_ACCURACY	4

////////////////////////////////////////////////////////

class CMyLcd : public CLcd
{
private:

	typedef CLcd super;

public:

	CMyLcd()												{ _curretDraw = NULL; _expectButtonOff = false; _rotaryFocus = MainPage; }

	virtual void Init();
	virtual void Idle(unsigned int idletime);
	virtual void TimerInterrupt();

	typedef signed char rotarypos_t;

	enum EPage
	{
		SplashPage = 0,
		DebugPage = 1,
		AbsPage = 2,
		PresetPage = 3,
		StartSDPage = 4,
		PausePage = 5,
		ErrorPage = 6,
		PageXX = 7,
		PageCount
	};

	enum ERotaryFocus
	{
		MainPage,
		MenuPage
	};

	EnumAsByte(EPage) GetPage();
	void SetRotaryFocusMainPage();
	void SetRotaryFocusMenuPage();

protected:

	virtual void Draw(EDrawType draw);
	virtual unsigned long Splash();
	virtual void FirstDraw();

	CRotaryButton<rotarypos_t, ROTARY_ACCURACY> _button;

private:

	EnumAsByte(ERotaryFocus) _rotaryFocus;
	EnumAsByte(EPage)		_currentpage;
	unsigned char			_currentMenu;
	unsigned char			_currentMenuOffset;

	bool _expectButtonOff;

	typedef bool(*DrawFunction)(CMyLcd* t, bool setup);
	typedef void(*ButtonFunction)(CMyLcd* t);

	struct SPageDef
	{
		DrawFunction draw;
		ButtonFunction buttonpress;
	};

	static SPageDef _pagedef[];

	void DrawLoop(DrawFunction drawfnc)						{ _curretDraw = drawfnc; DrawLoop(); }
	void DrawLoop();

	DrawFunction _curretDraw;

	void ButtonPress();

	void ButtonPressNOP() {};			static void MyButtonPressNOP(CMyLcd* t)					{ return t->ButtonPressNOP(); };
	void ButtonPressPresetPage();		static void MyButtonPressPresetPage(CMyLcd* t)			{ return t->ButtonPressPresetPage(); };
	void ButtonPressStartSDPage();		static void MyButtonPressStartSDPage(CMyLcd* t)			{ return t->ButtonPressStartSDPage(); };
	void ButtonPressPause();			static void MyButtonPressPause(CMyLcd* t)				{ return t->ButtonPressPause(); };
	void ButtonPressMenuPage();			static void MyButtonPressMenuPage(CMyLcd* t)			{ return t->ButtonPressMenuPage(); };

	bool DrawLoopSplash(bool setup);	static bool MyDrawLoopSplash(CMyLcd* t, bool setup)		{ return t->DrawLoopSplash(setup); };
	bool DrawLoopDebug(bool setup);		static bool MyDrawLoopDebug(CMyLcd* t, bool setup)		{ return t->DrawLoopDebug(setup); };
	bool DrawLoopPosAbs(bool setup);	static bool MyDrawLoopPosAbs(CMyLcd* t, bool setup)		{ return t->DrawLoopPosAbs(setup); };
	bool DrawLoopPreset(bool setup);	static bool MyDrawLoopPreset(CMyLcd* t, bool setup)		{ return t->DrawLoopPreset(setup); };
	bool DrawLoopStartSD(bool setup);	static bool MyDrawLoopStartSD(CMyLcd* t, bool setup)    { return t->DrawLoopStartSD(setup); };
	bool DrawLoopPause(bool setup);		static bool MyDrawLoopPause(CMyLcd* t, bool setup)		{ return t->DrawLoopPause(setup); };
	bool DrawLoopError(bool setup);		static bool MyDrawLoopError(CMyLcd* t, bool setup)		{ return t->DrawLoopError(setup); };
	bool DrawLoopMenu(bool setup);		static bool MyDrawLoopMenu(CMyLcd* t, bool setup)		{ return t->DrawLoopMenu(setup); };

	bool DrawLoopSetupDefault();
	void DrawLoopDefaultHead();

	void Beep();

	unsigned char GetMenuIdx();
	unsigned char GetMenuCount();
};

////////////////////////////////////////////////////////

extern CMyLcd Lcd;
