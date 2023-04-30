using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.ConsoleTools;

class TextBox
{
    static object _padLock = new object();  //STATIC / SHARED

    StringBuilder _sb;
    StringBuilder StringBuilder { get { return _sb; } }
    public string Text { get { return _sb.ToString(); } }
    
    int _xOrigin, _yOrigin, _width, _height, _cursorLocalX, _cursorLocalY;
    public int OriginX { get { return _xOrigin; } }
    public int OriginY { get { return _yOrigin; } }
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public int CursorLocalX { get { return _cursorLocalX; } }
    public int CursorLocalY { get { return _cursorLocalY; } }
    public int CursorGlobalX { get { return OriginX + CursorLocalX; } }
    public int CursorGlobalY { get { return OriginY + CursorLocalY; } }

    ConsoleColor _backgroundColor, _foregroundColor;
    ConsoleColor BackgroundColor { get { return _backgroundColor; } }
    ConsoleColor ForegroundColor { get { return _foregroundColor; } }

    public TextBox(int x, int y, int w, int h, ConsoleColor backGroundColor = ConsoleColor.Black)
    {
        _xOrigin = x;
        _yOrigin = y;
        _width = w;
        _height = h;
        _cursorLocalX = 0;
        _cursorLocalY = 0;
        _sb = new StringBuilder(Width * Height);

        SetBackgroundColor(backGroundColor);
    }
    void ResetCursorPosition()
    {
        lock (_padLock)
        {
            _cursorLocalX = 0;
            _cursorLocalY = 0;
        }
    }
    void NextCursorPosition()
    {
        lock (_padLock)
        {
            if (CursorLocalX == Width - 1)
            {
                if (CursorLocalY == Height - 1) return;
                //Next line

                _cursorLocalX = 0;
                _cursorLocalY++;
                return;
            }

            //Next char

            _cursorLocalX++;
        }
    }
    void PreviousCursorPosition()
    {
        lock (_padLock)
        {
            if (CursorLocalX == 0)
            {
                if (CursorLocalY == 0) return;
                //Prev line

                _cursorLocalX = Width - 1;
                _cursorLocalY--;
                return;
            }

            //Prev char

            _cursorLocalX--;
        }
    }

    public void Clear()
    {
        lock (_padLock)
        {
            StringBuilder.Clear();
            ResetCursorPosition();
            for (int i = 0; i < StringBuilder.Capacity; i++)
            {
                TryAppendChar(' ');
            }
            StringBuilder.Clear();
            ResetCursorPosition();
        }
    }
    public bool TryAppendChar(char c)
    {
        lock (_padLock)
        {
            if (StringBuilder.Length >= StringBuilder.Capacity) return false;

            StringBuilder.Append(c);

            Console.SetCursorPosition(CursorGlobalX, CursorGlobalY);
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.Write(c);

            NextCursorPosition();

            return true;
        }
    }
    public bool TryRemoveLastChar()
    {
        lock (_padLock)
        {
            if (StringBuilder.Length <= 0) return false;

            StringBuilder.Remove(StringBuilder.Length - 1, 1);

            Console.SetCursorPosition(CursorGlobalX, CursorGlobalY);
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.Write(' ');

            PreviousCursorPosition();
            return true;
        }
    }
    public void SetText(string newText)
    {
        lock (_padLock)
        {
            Clear();
            for (int i = 0; i < newText.Length; i++)
            {
                if (!TryAppendChar(newText[i])) break;
            }
        }
    }

    ConsoleColor GetForegroundColor()
    {
        lock (_padLock)
        {
            if (BackgroundColor == ConsoleColor.Black) return ConsoleColor.White;
            if (BackgroundColor == ConsoleColor.DarkBlue) return ConsoleColor.White;
            return ConsoleColor.Black;
        }
    }
    public void SetBackgroundColor(ConsoleColor color)
    {
        lock (_padLock)
        {
            _backgroundColor = color;
            _foregroundColor = GetForegroundColor();

            SetText(Text);
        }
    }
}
