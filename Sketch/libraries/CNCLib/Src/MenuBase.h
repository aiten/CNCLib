////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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

////////////////////////////////////////////////////////

class CMenuBase
{
private:

	//	typedef CXXX super;

public:

	CMenuBase()												{ SetMenu(NULL); }

	struct SMenuItemDef;
	struct SMenuDef;

	typedef unsigned char menupos_t;
	typedef void(CMenuBase::*MenuFunction)(const SMenuItemDef*);
	typedef const void* menuparam_t;

	struct SMenuItemDef
	{
	public:
		const char* _text;
		MenuFunction _buttonpress;
		menuparam_t _param1;
		menuparam_t _param2;
	public:
		const __FlashStringHelper* GetText() const			{ return (const __FlashStringHelper*)pgm_read_ptr(&this->_text); }
		MenuFunction GetButtonPress()const;
		menuparam_t GetParam1()	const						{ return (menuparam_t)pgm_read_word(&this->_param1); }
		menuparam_t GetParam2()	const						{ return (menuparam_t)pgm_read_word(&this->_param2); }
	};

	struct SMenuDef
	{
	public:
		const char* _text;
		const SMenuItemDef* _items;
		menuparam_t _param1;
		//		menuparam_t _param2;

		unsigned char GetItemCount() const
		{
			const SMenuItemDef* items = GetItems();
			for (unsigned char x = 0;; x++)
			{
				if (items[x].GetText() == NULL) return x;
			}
		}

		unsigned char FindMenuIdx(const void*param, bool(*check)(const SMenuItemDef*, const void*param)) const
		{
			const SMenuItemDef* item = &GetItems()[0];
			for (unsigned char x = 0; item->GetText() != NULL; x++, item++)
			{
				if (check(item, param)) return x;
			}

			return 0;
		}


	public:
		const __FlashStringHelper* GetText() const		{ return (const __FlashStringHelper*)pgm_read_ptr(&this->_text); }
		const SMenuItemDef* GetItems() const			{ return (const SMenuItemDef*)pgm_read_word(&this->_items); }
		menuparam_t GetParam1()	const					{ return (menuparam_t)pgm_read_word(&this->_param1); }
		//		menuparam_t GetMenuParam2()	const				{ return (menuparam_t)pgm_read_word(&this->_param2); }
	};

protected:

	static bool SendCommand(const __FlashStringHelper* cmd);
	static bool SendCommand(char* cmd);

public:

	menupos_t GetPosition() { return _position; }
	void SetPosition(menupos_t position) { _position = position; }

	menupos_t GetOffset() { return _offset; }
	void SetOffset(menupos_t offset) { _offset = offset; }
	void AddOffset(menupos_t offset) { _offset += offset; }
	void SubOffset(menupos_t offset) { _offset -= offset; }

	void AdjustPositionAndOffset(menupos_t firstline, menupos_t lastline);
	unsigned char ToPrintLine (menupos_t firstline, menupos_t lastline, menupos_t i);		// return 255 if not to print

	const SMenuDef*GetMenuDef()						{ return _current; }

	bool Select();
	virtual void Changed()=0;
	virtual void Beep()=0;

private:

	menupos_t			_position;
	menupos_t			_offset;
	const SMenuDef*		_current;

public:

	void MenuButtonPressSetMenu(const SMenuItemDef*);
	void MenuButtonPressSetCommand(const SMenuItemDef*def)			{ SendCommand((const __FlashStringHelper*)def->GetParam1()); Beep(); }

	void SetMenu(const SMenuDef* pMenu)								{ _current = pMenu; _position = 0; _offset = 0; };

#if defined(__AVR_ARCH__)

	static MenuFunction GetMenuButtonPress_P(const void* adr);

#endif
};

////////////////////////////////////////////////////////
