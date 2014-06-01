#pragma once

enum ETickerType
{
	ScrollFromRight,
	ScrollSingleCharFadeInRightFadeOutRight,
	ScrollSingleCharFadeInRightFadeOutLeft,
	ScrollSingleCharFadeInRightFadeOutUp,
	ScrollSingleCharFadeInRightFadeOutDown,
	Scroll5,
	Scroll6
};

extern void TickerLoop(char* text, ETickerType style, int delayms);

