////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/
////////////////////////////////////////////////////////

#pragma once

#include <io.h>
#include <sys/types.h>
#include <sys/stat.h>

////////////////////////////////////////////////////////////

#define FILE_READ 1
#define FILE_WRITE 2

#define SDPATH "c:\\tmp\\SD"

////////////////////////////////////////////////////////////

class SDClass
{
public:
	bool begin(uint8_t ) { return true; };

	class File open(const char *filename, uint8_t mode = FILE_READ);

	bool remove(const char *filename)
	{
		return ::remove(GetFilename(filename)) == 0;
	}

	bool exists(const char *filename)
	{
		return _access(GetFilename(filename), 0) != -1;
	}

private:

	char _fullfilename[512];

	char* GetFilename(const char *filename)
	{
		sprintf_s<512>(_fullfilename, SDPATH"\\%s", filename);
		return _fullfilename;
	}

};

extern SDClass SD;

////////////////////////////////////////////////////////////

class MyDirFile
{
public:
	virtual bool IsFile()=0;
	virtual void close()=0;
	virtual void open(int mode)=0;
	virtual bool isopen()=0;

	char _OSfilename[512];
	char _pathname[512];
	char _name[8 + 1 + 3 + 1];

	bool isDirectory()
	{
		struct stat st;
		return stat(_OSfilename, &st) != -1 && (st.st_mode & _S_IFDIR) != 0;
	}

	MyDirFile() { _refcount = 0; }

	int _refcount;
	void IncRef()	{ _refcount++; };
	void DecRef()	
	{	
		_refcount--; 
		if (_refcount==0) 
		{	
			close(); 
			delete this; 
		} 
	};
};

////////////////////////////////////////////////////////////

class MyFile : public MyDirFile
{
public:

	MyFile()
	{
		_f = NULL;
	}

	FILE* _f;

	virtual bool IsFile() override  { return true; };
	virtual void close()   override { if (_f) fclose(_f); _f = NULL; };
	virtual bool isopen()  override { return _f != NULL; }

	virtual void open(int mode) override
	{
		if (mode == FILE_READ)
			fopen_s(&_f, _OSfilename, "rb");
		else
		{
			fopen_s(&_f, _OSfilename, "wb");
			if (_f != NULL)
			{
				fclose(_f);
				fopen_s(&_f, _OSfilename, "r+");
			}
		}
	}
};

////////////////////////////////////////////////////////////

class MyDir : public MyDirFile
{
public:

	MyDir()
	{
		_dir = NULL;
	}

	virtual bool IsFile() override { return false; };
	virtual void close()  override { if (_dir) FindClose(_dir); _dir = NULL; };
	virtual bool isopen() override { return _dir != NULL; }

	HANDLE _dir;
	bool   _dirEof;
	WIN32_FIND_DATAA ffd;
	char _dirfindmask[512];

	class File openNextFile();

	virtual void open(int /* mode */) override
	{
		close();
		if (!isDirectory()) return;
		strcpy_s(_dirfindmask, _OSfilename);
		strcat_s(_dirfindmask, "\\*");

		_dir = FindFirstFileA(_dirfindmask, &ffd);
	}
};

////////////////////////////////////////////////////////////

class File : public Stream
{
private:

	MyDirFile* _dirfile;

	bool IsDirHandle()	const	{ return _dirfile != NULL && !_dirfile->IsFile(); };
	bool IsFileHandle()	const 	{ return _dirfile != NULL && _dirfile->IsFile(); };

	MyFile* GetF()				{ return IsFileHandle() ? ((MyFile*) _dirfile) : NULL; }
	MyDir* GetD()				{ return IsDirHandle() ? ((MyDir*) _dirfile) : NULL; }


public:

	File operator=(const File& src)
	{
		_dirfile = src._dirfile;
		if (_dirfile) _dirfile->IncRef();
		return *this;
	}

	File(const File& src) : Stream(src)
	{
		_dirfile = src._dirfile;
		if (_dirfile) _dirfile->IncRef();
	}

	~File()
	{
		if (_dirfile) _dirfile->DecRef();
		_dirfile = NULL;
	}

	File()		{ _dirfile = NULL; }

	void close()
	{
		if (_dirfile) 
		{
			_dirfile->close();
			_dirfile->DecRef();
			_dirfile = NULL;
		}
	}

	void open(const char* name, const char* osfilename, const char* pathname, int mode)
	{
		if (isDirectory(osfilename))
		{
			_dirfile = new MyDir();
		}
		else
		{
			_dirfile = new MyFile();
		}
		_dirfile->IncRef();

		strcpy_s(_dirfile->_name,name);
		strcpy_s(_dirfile->_OSfilename,osfilename);
		strcpy_s(_dirfile->_pathname,pathname);

		_dirfile->open(mode);
	}

	operator bool()					{ return _dirfile != NULL && _dirfile->isopen(); }

	virtual int available()	 override { return feof(GetF()->_f) ? 0 : 1; }
	virtual char read() override	{ return (char)fgetc(GetF()->_f); }

	unsigned long size()
	{
		struct stat st;
		stat(_dirfile->_OSfilename, &st);
		return st.st_size;
	}

	const char* name()	{ return _dirfile->_name; }
	
	bool isDirectory(const char *name)
	{
		struct stat st;
		return stat(name, &st) != -1 && (st.st_mode & _S_IFDIR) != 0;
	}

	bool isDirectory()
	{
		return isDirectory(_dirfile->_OSfilename);
	}

	File openNextFile()
	{
		return GetD()->openNextFile();
	}

	void rewindDirectory(void) {}

	//	virtual int peek();
	//	virtual void flush();
	//	int read(void *buf, uint16_t nbyte);
	boolean seek(unsigned long pos)				{ return fseek(GetF()->_f, pos, SEEK_SET) == 0; }
	unsigned long position()					{ return ftell(GetF()->_f); }

};

////////////////////////////////////////////////////////////

inline File MyDir::openNextFile()
{
	File ret;

	if (_dir)
	{
		while (ffd.cFileName[0] == '.')
		{
			if (!FindNextFileA(_dir, &ffd))
			{
				FindClose(_dir);
				_dir = NULL;
				_dirEof = true;
				break;
			}
		}
		if (!_dirEof)
		{
			char tmp[256];
			tmp[0] = 0;
			if (strcmp(_pathname, "/") != 0)
				strcpy_s(tmp, _pathname);
			strcat_s(tmp, "/");
			strcat_s(tmp, ffd.cFileName);

			ret = SD.open(tmp);

			if (!FindNextFileA(_dir, &ffd))
			{
				FindClose(_dir);
				_dir = NULL;
				_dirEof = true;
			}
		}
	}
	else if (!_dirEof)
	{
		if (!isDirectory()) return ret;
		strcpy_s(_dirfindmask, _OSfilename);
		strcat_s(_dirfindmask, "\\*");

		_dir = FindFirstFileA(_dirfindmask, &ffd);
		if (_dir)
			return openNextFile();
	}

	return ret;
}


inline File SDClass::open(const char *filename, uint8_t mode)
{
	char osfilename[256];
	char pathname[256];
	char name[256];

	File file;

	if (filename[0] == 0)
		return file;

//	file._mode = mode;

	if (filename[0] == '/')
	{
		if (filename[1] == 0)
			sprintf_s<256>(osfilename, SDPATH);
		else
			sprintf_s<256>(osfilename, SDPATH"%s", filename);
	}
	else
		sprintf_s<256>(osfilename, SDPATH"\\%s", filename);

	for (char*t = osfilename; *t; t++) if (*t == '/') *t = '\\';

	strcpy_s(pathname, filename);
	char* dirend = strrchr(pathname, '/');
	if (dirend)
		strcpy_s(name, dirend + 1);
	else
		strcpy_s(name, filename);

	file.open(name,osfilename,pathname, mode);

	return file;
}
