﻿using System;

namespace Konsole
{
    public partial class Window
    {
        // **************************
        // **                      **
        // **  Write(string text)  **
        // **                      **
        // **************************
        public void Write(string text)
        {
            lock(_locker)
            {
                _Write(text);
            }
        }
        private void _Write(string text)
        {
            if (!Scrolling && OverflowBottom) return;
            DoCommand(_console, () => __write(_console, text));
        }

        // *******************************************
        // **                                       **
        // **  Write(string format, object args0)   **
        // **                                       **
        // *******************************************
        public void Write(string format, object args0)
        {
            lock (_locker)
            {
                _Write(format, args0);
            }
        }

        private void _Write(string format, object args0)
        {
                var text = string.Format(format, args0);
                _Write(text);
        }

        // ****************************************************
        // **                                                **
        // **  _write(string format, params object[] args)   **
        // **                                                **
        // ****************************************************
        public void Write(string format, params object[] args)
        {
            lock (_locker)
            {
                _Write(format, args);
            }
        }

        private void _Write(string format, params object[] args)
        {
            var text = string.Format(format, args);
            _Write(text);
        }

        // ***********************************************************************
        // **                                                                   **
        // **  Write(ConsoleColor color, string format, params object[] args)   **
        // **                                                                   **
        // ***********************************************************************
        public void Write(ConsoleColor color, string format, params object[] args)
        {
            lock (_locker)
            {
                _Write(color, format, args);
            }
        }

        private void _Write(ConsoleColor color, string format, params object[] args)
        {
            _WithColor(color, () => _Write(format, args));
        }

        // ***************************************************
        // **                                               **
        // **  WriteLine(ConsoleColor color, string text)   **
        // **                                               **
        // ***************************************************
        public void WriteLine(ConsoleColor color, string text)
        {
            lock (_locker)
            {
                _WriteLine(color, text);
            }
        }

        private void _WriteLine(ConsoleColor color, string text)
        {
            var foreground = ForegroundColor;
            try
            {
                ForegroundColor = color;
                _WriteLine(text);
            }
            finally
            {
                ForegroundColor = foreground;
            }
        }

        // **********************************************
        // **                                          **
        // **  WriteLine(Colors colors, string text)   **
        // **                                          **
        // **********************************************
        public void WriteLine(Colors colors, string text)
        {
            lock(_locker)
            {
                _WriteLine(colors, text);
            }
        }
        private void _WriteLine(Colors colors, string text)
        {
            var ccolors = _Colors;
            try
            {
                _Colors = colors;
                _WriteLine(text);
            }
            finally
            {
                _Colors = ccolors;
            }
        }

        // ******************************************
        // **                                      **
        // **  Write(Colors colors, string text)   **
        // **                                      **
        // ******************************************
        public void Write(Colors colors, string text)
        {
            lock(_locker)
            {
                _Write(colors, text);
            }
        }

        private void _Write(Colors colors, string text)
        {
            var ccolors = _Colors;
            try
            {
                _Colors = colors;
                _Write(text);
            }
            finally
            {
                _Colors = ccolors;
            }
        }

        // ***********************************************
        // **                                           **
        // **  Write(ConsoleColor color, string text)   **
        // **                                           **
        // ***********************************************
        public void Write(ConsoleColor color, string text)
        {
            lock(_locker)
            {
                _Write(color, text);
            }
        }
        private void _Write(ConsoleColor color, string text)
        {
            _WithColor(color, () => _Write(text));
        }

        // ***********************************************
        // **                                           **
        // **  WriteLine(string format, object arg0)    **
        // **                                           **
        // ***********************************************
        public void WriteLine(string format, object arg0)
        {
            lock(_locker)
            {
                _writeLine(format, arg0);
            }
        }

        private void _writeLine(string format, object arg0)
        {
            _WriteLine(string.Format(format, arg0));
        }

        // ******************************************************
        // **                                                  **
        // **  WriteLine(string format, params object[] args)  **
        // **                                                  **
        // ******************************************************
        public void WriteLine(string format, params object[] args)
        {
            lock(_locker)
            {
                _writeLine(format, args);
            }
        }

        private void _writeLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        // **************************************************************************
        // **                                                                      **
        // **  WriteLine(ConsoleColor color, string format, params object[] args)  **
        // **                                                                      **
        // **************************************************************************
        /// <summary>
        /// Write the text to the window in the {color} color, withouting resetting the window's current foreground colour. Optionally causes text to wrap, and if text moves beyond the end of the window causes the window to scroll.
        /// </summary>
        public void WriteLine(ConsoleColor color, string format, params object[] args)
        {
            var foreground = ForegroundColor;
            try
            {
                ForegroundColor = color;
                WriteLine(format, args);
            }
            finally
            {
                ForegroundColor = foreground;
            }
        }

        // ********************************
        // **                            **
        // **  WriteLine(string text)    **
        // **                            **
        // ********************************
        public void WriteLine(string text)
        {
            lock (_locker)
            {
                _WriteLine(text);
            }
        }

        private void _WriteLine(string text)
        {
            if (!Scrolling && OverflowBottom)
            {
                return;
            }

            if (OverflowBottom)
            {
                _scrollDown();
                _Write(text);
                Cursor = new XY(0, Cursor.Y + 1);
                return;
            }

            _Write(text);
            Cursor = new XY(0, Cursor.Y + 1);
            if (OverflowBottom && Scrolling)
            {
                _scrollDown();
            }
        }

        // ****************
        // **  __write   **
        // ****************
        private void __write(IConsole console, string text)
        {
            var overflow = "";
            while (overflow != null)
            {
                if (Clipping && Cursor.X >= WindowWidth) break;
                if (!_lines.ContainsKey(Cursor.Y)) return;
                var result = _lines[Cursor.Y].Write(ForegroundColor, BackgroundColor, Cursor.X, text);
                overflow = result.Overflow;
                if (_echo && _console != null)
                {
                    _console.ForegroundColor = ForegroundColor;
                    _console.BackgroundColor = BackgroundColor;
                    _console.PrintAt(CursorLeft + _x, CursorTop + _y, result.Written);
                }
                if (overflow == null)
                {
                    Cursor = Cursor.IncX(text.Length);
                }
                else
                {
                    if (Clipping)
                    {
                        Cursor = new XY(WindowWidth, Cursor.Y);
                        break;
                    }
                    Cursor = new XY(0, Cursor.Y + 1);
                    if (!Scrolling && OverflowBottom)
                        break;
                    if (OverflowBottom)
                        ScrollDown();
                }
                text = overflow;
            }
        }

        // *****************************************************
        // **  _WithColor(ConsoleColor color, Action action)  **
        // *****************************************************
        private void _WithColor(ConsoleColor color, Action action)
        {
            var foreground = ForegroundColor;
            try
            {
                ForegroundColor = color;
                action();
            }
            finally
            {
                ForegroundColor = foreground;
            }
        }

    }
}
