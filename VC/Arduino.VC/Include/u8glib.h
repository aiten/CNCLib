#pragma once

class U8GLIB_ST7920_128X64_1X // : public Stream
{
public:
	U8GLIB_ST7920_128X64_1X(int, int, int) {};
	void firstPage() {};
	bool nextPage() { return false; }
	void drawStr(int, int, const char*) {}
	void setFont(int) {};
	void setPrintPos(int , int )	{ };

	void print(const char)			{ };

	void print(const char*)			{ };
	void println(const char*)		{ };
};

static int u8g_font_unifont;
static int u8g_font_unifontr;
static int u8g_font_6x12;
