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
	bool begin(unsigned char ) { return true; };

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
	FILE* _f;

	virtual bool IsFile() { return true; };
	virtual void close()  { if (_f) fclose(_f); _f=NULL; };
	virtual bool isopen() { return _f != NULL; }

	virtual void open(int mode)
	{
		if (mode == FILE_READ)
			fopen_s(&_f, _OSfilename, "r");
		else
		{
			fopen_s(&_f, _OSfilename, "w");
			fclose(_f);
			fopen_s(&_f, _OSfilename, "r+");
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

	virtual bool IsFile() { return false; };
	virtual void close()  { if (_dir) FindClose(_dir); _dir = NULL; };
	virtual bool isopen() { return _dir != NULL; }

	HANDLE _dir;
	bool   _dirEof;
	WIN32_FIND_DATAA ffd;
	char _dirfindmask[512];

	class File openNextFile();

	virtual void open(int /* mode */) 
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

	virtual int available()			{ return feof(GetF()->_f) ? 0 : 1; }
	virtual char read()				{ return (char) fgetc(GetF()->_f); }

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


#if adsfasdf

public:

	HANDLE _dir;

	char _OSfilename[512];
	char _pathname[512];
	char _name[8 + 1 + 3 + 1];
	bool   _dirEof;
	WIN32_FIND_DATAA ffd;
	char _dirfindmask[512];
	uint8_t _mode;


	File(const File& src) : Stream(src)
	{
		_mode = src._mode;
		_f = src._f;
		strcpy_s(_OSfilename, src._OSfilename);
		strcpy_s(_pathname, src._pathname);
		strcpy_s(_name, src._name);
		_dir = src._dir;
		_dirEof = src._dirEof;
		strcpy_s(_dirfindmask, src._dirfindmask);
		ffd = src.ffd;

		if (src.IsFileHandle())
		{
			long pos = ftell(src._f);
			fclose(src._f);
			((File&)src)._f = NULL;
			fopen_s(&_f, _OSfilename, _mode == FILE_READ ? "r" : "r+");
			fseek(_f,pos,SEEK_SET);
		}
	}

	~File()
	{
		if (_f) close();
		if (_dir)
		{
			FindClose(_dir); _dir = NULL;
		}
	}

	File()							{ _f = NULL; _dir = NULL;  _dirEof = false; }

	void close()
	{
		if (_f && _f != (FILE*)1)
			fclose(_f);
		_f = NULL;
	}
	operator bool()					{ return _f != NULL; }

	virtual int available()			{ return feof(_f) ? 0 : 1; }
	virtual char read()				{ return fgetc(_f); }

	unsigned long size()
	{
		struct stat st;
		stat(_OSfilename, &st);
		return st.st_size;
	}

	const char* name()	{ return _name; }
	bool isDirectory()
	{
		struct stat st;
		return stat(_OSfilename, &st) != -1 && (st.st_mode & _S_IFDIR) != 0;
	}

	File openNextFile()
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
		}

		return ret;
	}

	void rewindDirectory(void) {}

	//	virtual int peek();
	//	virtual void flush();
	//	int read(void *buf, uint16_t nbyte);
	boolean seek(unsigned long pos)				{ return fseek(_f, pos, SEEK_SET) == 0; }
	unsigned long position()					{ return ftell(_f); }

};


#endif


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
