using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Bespoke.Common
{
	/// <summary>
	/// 
	/// </summary>
	public static class InputManager
	{
		/// <summary>
		///  Specifies constants that define which mouse button was pressed.
		/// </summary>
		[Flags]
		public enum MouseButtons
		{
			/// <summary>
			/// No button is pressed.
			/// </summary>
			None = 0,

			/// <summary>
			/// The left mouse button was pressed.
			/// </summary>
			Left = 1048576,

			/// <summary>
			/// The right mouse button was pressed.
			/// </summary>
			Right = 2097152,

			/// <summary>
			/// The middle mouse button was pressed.
			/// </summary>
			Middle = 4194304,

			/// <summary>
			/// The first XButton was pressed.
			/// </summary>
			XButton1 = 8388608,

			/// <summary>
			/// The second XButton was pressed.
			/// </summary>
			XButton2 = 16777216
		}
		
		/// <summary>
		/// Virtual key codes.
		/// </summary>
		public enum VKeys : ushort
		{ 
			SHIFT = 0x10,
			CONTROL = 0x11, 
			MENU = 0x12, 
			ESCAPE = 0x1B, 
			BACK = 0x08, 
			TAB  = 0x09, 
			RETURN = 0x0D, 
			PRIOR = 0x21, 
			NEXT = 0x22,
			END  = 0x23, 
			HOME = 0x24, 
			LEFT = 0x25, 
			UP  = 0x26, 
			RIGHT = 0x27,
			DOWN = 0x28, 
			SELECT = 0x29, 
			PRINT = 0x2A, 
			EXECUTE = 0x2B, 
			SNAPSHOT = 0x2C, 
			INSERT = 0x2D,
			DELETE = 0x2E, 
			HELP = 0x2F, 
			NUMPAD0 = 0x60, 
			NUMPAD1 = 0x61, 
			NUMPAD2 = 0x62, 
			NUMPAD3 = 0x63, 
			NUMPAD4 = 0x64, 
			NUMPAD5 = 0x65, 
			NUMPAD6 = 0x66, 
			NUMPAD7 = 0x67, 
			NUMPAD8 = 0x68, 
			NUMPAD9 = 0x69, 
			MULTIPLY = 0x6A, 
			ADD  = 0x6B, 
			SEPARATOR = 0x6C,
			SUBTRACT = 0x6D, 
			DECIMAL = 0x6E, 
			DIVIDE = 0x6F, 
			F1  = 0x70,
			F2  = 0x71, 
			F3  = 0x72, 
			F4  = 0x73, 
			F5  = 0x74, 
			F6  = 0x75,
			F7  = 0x76,
			F8  = 0x77, 
			F9  = 0x78, 
			F10  = 0x79, 
			F11  = 0x7A, 
			F12  = 0x7B, 
			OEM_1 = 0xBA,  // ',:' for US
			OEM_PLUS = 0xBB,  // '+' any country 
			OEM_COMMA = 0xBC,  // ',' any country 
			OEM_MINUS = 0xBD,  // '-' any country 
			OEM_PERIOD = 0xBE,  // '.' any country 
			OEM_2 = 0xBF,  // '/?' for US 
			OEM_3 = 0xC0,  // '`~' for US 
			MEDIA_NEXT_TRACK = 0xB0, 
			MEDIA_PREV_TRACK = 0xB1, 
			MEDIA_STOP = 0xB2, 
			MEDIA_PLAY_PAUSE = 0xB3,
			LWIN = 0x5B, 
			RWIN = 0x5C,

            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A
		}

        public enum KeyEventType : uint
        {
            Down = 0,
            Up = KEYEVENTF_KEYUP
        }

		#region User32 Imports

		private const int INPUT_MOUSE = 0;
		private const int INPUT_KEYBOARD = 1;
		private const uint XBUTTON1 = 0x0001;
		private const uint XBUTTON2 = 0x0002;
		private const uint MOUSEEVENTF_MOVE = 0x0001;
		private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		private const uint MOUSEEVENTF_LEFTUP = 0x0004;
		private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
		private const uint MOUSEEVENTF_XDOWN = 0x0080;
		private const uint MOUSEEVENTF_XUP = 0x0100;
		private const uint MOUSEEVENTF_WHEEL = 0x0800;
		private const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
		private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
		private const uint WHEEL_DELTA = 120;
		private const uint KEYEVENTF_KEYUP = 0x0002;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

		[StructLayout(LayoutKind.Sequential)]
		private struct MOUSEINPUT
		{
			public int dx;//4
			public int dy;//4
			public uint mouseData;//4
			public uint dwFlags;//4
			public uint time;//4
			public IntPtr dwExtraInfo;//4
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct KEYBDINPUT
		{
			public ushort wVk;//2
			public ushort wScan;//2
			public uint dwFlags;//4
			public uint time;//4
			public IntPtr dwExtraInfo;//4
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct HARDWAREINPUT
		{
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}

		[StructLayout(LayoutKind.Explicit, Size = 28)]
		private struct INPUT
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(4)]
			public MOUSEINPUT mi;
			[FieldOffset(4)]
			public KEYBDINPUT ki;
			[FieldOffset(4)]
			public HARDWAREINPUT hi;
		}

		[DllImport("user32.dll")]
		private static extern IntPtr GetMessageExtraInfo();

		[DllImport("user32.dll")]
		private static extern bool SetCursorPos(int x, int y);
 
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void MouseMove(int x, int y)
		{
			SetCursorPos(x, y);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		public static void MouseDown(MouseButtons button)
		{
			uint mouseData;
			uint downFlag = MouseButtonToFlag(button, true, out mouseData);

			INPUT[] buffer = new INPUT[1];
			buffer[0].type = INPUT_MOUSE;
			buffer[0].mi.dx = 0;
			buffer[0].mi.dy = 0;
			buffer[0].mi.mouseData = mouseData;
			buffer[0].mi.dwFlags = downFlag;
			buffer[0].mi.time = 0;
			buffer[0].mi.dwExtraInfo = (IntPtr)0;

			SendInput(1, buffer, Marshal.SizeOf(buffer[0]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		public static void MouseUp(MouseButtons button)
		{
			uint mouseData;
			uint downFlag = MouseButtonToFlag(button, false, out mouseData);

			INPUT[] buffer = new INPUT[1];
			buffer[0].type = INPUT_MOUSE;
			buffer[0].mi.dx = 0;
			buffer[0].mi.dy = 0;
			buffer[0].mi.mouseData = mouseData;
			buffer[0].mi.dwFlags = downFlag;
			buffer[0].mi.time = 0;
			buffer[0].mi.dwExtraInfo = (IntPtr)0;

			SendInput(1, buffer, Marshal.SizeOf(buffer[0]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		public static void MouseClick(MouseButtons button)
		{
			uint mouseData;
			uint downFlag = MouseButtonToFlag(button, true, out mouseData);

			INPUT[] buffer = new INPUT[2];
			buffer[0].type = INPUT_MOUSE;
			buffer[0].mi.dx = 0;
			buffer[0].mi.dy = 0;
			buffer[0].mi.mouseData = mouseData;
			buffer[0].mi.dwFlags = downFlag;
			buffer[0].mi.time = 0;
			buffer[0].mi.dwExtraInfo = (IntPtr)0;

			uint upFlag = MouseButtonToFlag(button, false, out mouseData);

			buffer[1].type = INPUT_MOUSE;
			buffer[1].mi.dx = 0;
			buffer[1].mi.dy = 0;
			buffer[1].mi.mouseData = mouseData;
			buffer[1].mi.dwFlags = upFlag;
			buffer[1].mi.time = 0;
			buffer[1].mi.dwExtraInfo = (IntPtr)0;

			SendInput(2, buffer, Marshal.SizeOf(buffer[0]));
		}

		/// <summary>
		/// 
		/// </summary>
		public static void MouseDoubleClick()
		{
			MouseClick(MouseButtons.Left);
			MouseClick(MouseButtons.Left);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public static void MouseWheel(uint value)
		{
			uint mouseData = value * WHEEL_DELTA;
			
			INPUT[] buffer = new INPUT[1];
			buffer[0].type = INPUT_MOUSE;
			buffer[0].mi.dx = 0;
			buffer[0].mi.dy = 0;
			buffer[0].mi.mouseData = mouseData;
			buffer[0].mi.dwFlags = MOUSEEVENTF_WHEEL;
			buffer[0].mi.time = 0;
			buffer[0].mi.dwExtraInfo = (IntPtr)0;

			SendInput(1, buffer, Marshal.SizeOf(buffer[0]));
		}

        public static void SendKeys(VKeys[] keys)
        {
            SendKeys(ModifierKeys.None, keys, KeyEventType.Down);
            SendKeys(ModifierKeys.None, keys, KeyEventType.Up);
        }

        public static void SendKeys(ModifierKeys modifierKeys, VKeys[] keys)
        {
            SendKeys(modifierKeys, keys, KeyEventType.Down);
            SendKeys(modifierKeys, keys, KeyEventType.Up);
        }

        public static void SendKeys(ModifierKeys modifierKeys, VKeys[] keys, KeyEventType eventType)
        {
            Assert.ParamIsNotNull(keys);

            List<INPUT> inputBuffer = new List<INPUT>();

            if (eventType == KeyEventType.Down)
            {
                // Key down events
                if (Library.IsFlagSet(modifierKeys, Bespoke.Common.ModifierKeys.Shift))
                {
                    inputBuffer.Add(CreateKeyEvent(VKeys.SHIFT, KeyEventType.Down));
                }
                if (Library.IsFlagSet(modifierKeys, Bespoke.Common.ModifierKeys.Control))
                {
                    inputBuffer.Add(CreateKeyEvent(VKeys.CONTROL, KeyEventType.Down));
                }
                if (Library.IsFlagSet(modifierKeys, Bespoke.Common.ModifierKeys.Alt))
                {
                    inputBuffer.Add(CreateKeyEvent(VKeys.MENU, KeyEventType.Down));
                }
                foreach (VKeys key in keys)
                {
                    inputBuffer.Add(CreateKeyEvent(key, KeyEventType.Down));
                }
            }
            else
            {
                // Add key up events
                foreach (VKeys key in keys)
                {
                    inputBuffer.Add(CreateKeyEvent(key, KeyEventType.Up));
                }
                if (Library.IsFlagSet(modifierKeys, Bespoke.Common.ModifierKeys.Alt))
                {
                    inputBuffer.Add(CreateKeyEvent(VKeys.MENU, KeyEventType.Up));
                }
                if (Library.IsFlagSet(modifierKeys, Bespoke.Common.ModifierKeys.Control))
                {
                    inputBuffer.Add(CreateKeyEvent(VKeys.CONTROL, KeyEventType.Up));
                }
                if (Library.IsFlagSet(modifierKeys, Bespoke.Common.ModifierKeys.Shift))
                {
                    inputBuffer.Add(CreateKeyEvent(VKeys.SHIFT, KeyEventType.Up));
                }
            }

            if (inputBuffer.Count > 0)
            {
                INPUT[] buffer = inputBuffer.ToArray();
                SendInput((uint)buffer.Length, buffer, Marshal.SizeOf(buffer[0]));
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public static void Tab()
		{
            VKeys[] keys = { VKeys.TAB };
            SendKeys(ModifierKeys.None, keys);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void ShiftTab()
		{
            VKeys[] keys = { VKeys.TAB };
            SendKeys(ModifierKeys.Shift, keys);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void AltTab()
		{
            VKeys[] keys = { VKeys.TAB };
            SendKeys(ModifierKeys.Alt, keys);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void AltShiftTab()
		{
            VKeys[] keys = { VKeys.TAB };
            SendKeys(ModifierKeys.Alt | ModifierKeys.Shift, keys);
		}

		#region Private Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="button"></param>
		/// <param name="down"></param>
		/// <param name="mouseData"></param>
		/// <returns></returns>
		private static uint MouseButtonToFlag(MouseButtons button, bool down, out uint mouseData)
		{
			uint mouseEventFlag;

			switch (button)
			{
				case MouseButtons.Left:
					mouseEventFlag = (down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP);
					mouseData = 0;
					break;

				case MouseButtons.Middle:
					mouseEventFlag = (down ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP);
					mouseData = 0;
					break;

				case MouseButtons.Right:
					mouseEventFlag = (down ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP);
					mouseData = 0;
					break;

				case MouseButtons.XButton1:
					mouseEventFlag = (down ? MOUSEEVENTF_XDOWN : MOUSEEVENTF_XUP);
					mouseData = XBUTTON1;
					break;

				case MouseButtons.XButton2:
					mouseEventFlag = (down ? MOUSEEVENTF_XDOWN : MOUSEEVENTF_XUP);
					mouseData = XBUTTON2;
					break;

				default:
					throw new ArgumentException();
			}

			return mouseEventFlag;
		}

        private static INPUT CreateKeyEvent(VKeys key, KeyEventType eventType)
        {
            INPUT keyEvent = new INPUT();
            keyEvent.type = INPUT_KEYBOARD;
            keyEvent.ki.wVk = (ushort)key;
            keyEvent.ki.wScan = 0;
            keyEvent.ki.dwFlags = (uint)eventType;
            keyEvent.ki.time = 0;
            keyEvent.ki.dwExtraInfo = (IntPtr)0;

            return keyEvent;
        }

		#endregion
	}
}
