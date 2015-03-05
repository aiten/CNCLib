/*
Trace.h
*/

#pragma once

#include <crtdbg.h>
#include <stdarg.h>
#include <stdio.h>
#include <string.h>

#ifdef _DEBUG
#define TRACEMAXSTRING 1024

inline void Trace(const char* format, ...)
{
	char szBuffer[TRACEMAXSTRING];
	va_list args;
	va_start(args, format);
	int nBuf;
	nBuf = _vsnprintf_s(szBuffer,
		TRACEMAXSTRING,
		format,
		args);
	va_end(args);

	_RPT0(_CRT_WARN, szBuffer);
}
#define TraceEx char szBuffer[TRACEMAXSTRING]; _snprintf_s(szBuffer,TRACEMAXSTRING,"%s(%d): ", \
	&strrchr(__FILE__,'\\')[1],__LINE__); \
	_RPT0(_CRT_WARN,szBuffer); \
	Trace 

#else
inline void Trace(const char* , ...) {};
inline void TraceEx(const char* , ...) {};
#endif


