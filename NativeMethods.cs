using System.Runtime.InteropServices;

namespace ClipboardAI;

internal static class NativeMethods
{
    public const int WM_DRAWCLIPBOARD = 0x0308;
    public const int WM_CHANGECBCHAIN = 0x030D;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public const int WM_HOTKEY = 0x0312;
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_NOREPEAT = 0x4000;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("user32.dll")]
    public static extern uint GetClipboardSequenceNumber();

    [DllImport("user32.dll")]
    public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    public const uint INPUT_KEYBOARD = 1;
    public const uint KEYEVENTF_KEYUP = 0x0002;

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT_UNION
    {
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public uint type;
        public INPUT_UNION u;
    }

    public static void SendCtrlC()
    {
        var inputs = new INPUT[4];

        inputs[0].type = INPUT_KEYBOARD;
        inputs[0].u.ki = new KEYBDINPUT { wVk = 0x11 }; // Ctrl

        inputs[1].type = INPUT_KEYBOARD;
        inputs[1].u.ki = new KEYBDINPUT { wVk = 0x43 }; // C

        inputs[2].type = INPUT_KEYBOARD;
        inputs[2].u.ki = new KEYBDINPUT { wVk = 0x43, dwFlags = KEYEVENTF_KEYUP };

        inputs[3].type = INPUT_KEYBOARD;
        inputs[3].u.ki = new KEYBDINPUT { wVk = 0x11, dwFlags = KEYEVENTF_KEYUP };

        SendInput(4, inputs, Marshal.SizeOf<INPUT>());
    }

    public static void SendCtrlCWithAltRelease()
    {
        const ushort VK_LMENU = 0xA4;
        const ushort VK_RMENU = 0xA5;
        const ushort VK_CONTROL = 0x11;
        const ushort VK_C = 0x43;

        var inputs = new INPUT[8];

        inputs[0] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_LMENU, dwFlags = KEYEVENTF_KEYUP } } };
        inputs[1] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_RMENU, dwFlags = KEYEVENTF_KEYUP } } };
        inputs[2] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_CONTROL } } };
        inputs[3] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_C } } };
        inputs[4] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_C, dwFlags = KEYEVENTF_KEYUP } } };
        inputs[5] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_CONTROL, dwFlags = KEYEVENTF_KEYUP } } };
        inputs[6] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_LMENU } } };
        inputs[7] = new INPUT { type = INPUT_KEYBOARD, u = new INPUT_UNION { ki = new KEYBDINPUT { wVk = VK_RMENU } } };

        SendInput(8, inputs, Marshal.SizeOf<INPUT>());
    }
}
