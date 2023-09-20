using UGXP.Game;

namespace UGXP;

/// <summary>
/// The Input class contains functions for reading keys and mouse
/// </summary>
public class Input
{
	/// <summary>
	/// Returns 'true' if given key is down, else returns 'false'
	/// </summary>
	/// <param name='key'>
	/// Key number, use <see cref="Key"/>.
	/// </param>
	public static bool GetKey(Key key) {
		return GameProcess.Context.GetKey((int) key);
	}
		
	/// <summary>
	/// Returns 'true' if specified key was pressed down during the current frame
	/// </summary>
	/// <param name='key'>
	/// Key number, use <see cref="Key"/>.
	/// </param>
	public static bool GetKeyDown(Key key) {
		return GameProcess.Context.GetKeyDown((int) key);
	}
		
	/// <summary>
	/// Returns 'true' if specified key was released during the current frame
	/// </summary>
	/// <param name='key'>
	/// Key number, use <see cref="Key"/>.
	/// </param>
	public static bool GetKeyUp(Key key) {
		return GameProcess.Context.GetKeyUp((int) key);
	}
		
	/// <summary>
	/// Returns true if any key is currently pressed.
	/// </summary>
	public static bool AnyKey() {
		return GameProcess.Context.AnyKey();
	}

	/// <summary>
	/// Returns true if any key was pressed down during the current frame.
	/// </summary>
	public static bool AnyKeyDown() {
		return GameProcess.Context.AnyKeyDown();
	}

	/// <summary>
	/// Returns 'true' if mousebutton is down, else returns 'false'
	/// </summary>
	/// <param name='button'>
    /// Use <see cref="MouseButton"/>
	/// </param>
	public static bool GetMouseButton(MouseButton button) {
		return GameProcess.Context.GetMouseButton((int) button);
	}
		
	/// <summary>
	/// Returns 'true' if specified mousebutton was pressed down during the current frame
	/// </summary>
	/// <param name='button'>
    /// Use <see cref="MouseButton"/>
	/// </param>
	public static bool GetMouseButtonDown(MouseButton button) {
		return GameProcess.Context.GetMouseButtonDown((int) button);
	}

	/// <summary>
	/// Returns 'true' if specified mousebutton was released during the current frame
	/// </summary>
	/// <param name='button'>
    /// Use <see cref="MouseButton"/>
	/// </param>
	public static bool GetMouseButtonUp(MouseButton button)
	{
		return GameProcess.Context.GetMouseButtonUp((int) button);
	}
		
	/// <summary>
	/// Gets the current mouse position in pixels.
	/// </summary>
	public static Vector2 mousePosition {
		get { return GameProcess.Context.mousePosition; }
	}
}

/// <summary>
/// Describes the state of a button/key.
/// </summary>
internal enum InputState : byte {
    /// <summary>
    /// The key or mouse button was released.
    /// </summary>
    Release,
    /// <summary>
    /// The key or mouse button was pressed.
    /// </summary>
    Press,
    /// <summary>
    /// The key was held down until it repeated.
    /// </summary>
    Repeat
}

/// <summary>
/// Strongly typed enum of all keys
/// </summary>
public enum Key {
    Unknown = -1,
    Space = 0x20,
    Apostrophe = 39,
    Comma = 44,
    Minus = 45,
    Period = 46,
    Slash = 47,
    Alpha0 = 48,
    Alpha1 = 49,
    Alpha2 = 50,
    Alpha3 = 51,
    Alpha4 = 52,
    Alpha5 = 53,
    Alpha6 = 54,
    Alpha7 = 55,
    Alpha8 = 56,
    Alpha9 = 57,
    SemiColon = 59,
    Equal = 61,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    LeftBracket = 91,
    Backslash = 92,
    RightBracket = 93,
    GraveAccent = 96,
    World1 = 161,
    World2 = 162,
    Escape = 0x100,
    Enter = 257,
    Tab = 258,
    Backspace = 259,
    Insert = 260,
    Delete = 261,
    Right = 262,
    Left = 263,
    Down = 264,
    Up = 265,
    PageUp = 266,
    PageDown = 267,
    Home = 268,
    End = 269,
    CapsLock = 280,
    ScrollLock = 281,
    NumLock = 282,
    PrintScreen = 283,
    Pause = 284,
    F1 = 290,
    F2 = 291,
    F3 = 292,
    F4 = 293,
    F5 = 294,
    F6 = 295,
    F7 = 296,
    F8 = 297,
    F9 = 298,
    F10 = 299,
    F11 = 300,
    F12 = 301,
    F13 = 302,
    F14 = 303,
    F15 = 304,
    F16 = 305,
    F17 = 306,
    F18 = 307,
    F19 = 308,
    F20 = 309,
    F21 = 310,
    F22 = 311,
    F23 = 312,
    F24 = 313,
    F25 = 314,
    Numpad0 = 320,
    Numpad1 = 321,
    Numpad2 = 322,
    Numpad3 = 323,
    Numpad4 = 324,
    Numpad5 = 325,
    Numpad6 = 326,
    Numpad7 = 327,
    Numpad8 = 328,
    Numpad9 = 329,
    NumpadDecimal = 330,
    NumpadDivide = 331,
    NumpadMultiply = 332,
    NumpadSubtract = 333,
    NumpadAdd = 334,
    NumpadEnter = 335,
    NumpadEqual = 336,
    LeftShift = 340,
    LeftControl = 341,
    LeftAlt = 342,
    LeftSuper = 343,
    RightShift = 344,
    RightControl = 345,
    RightAlt = 346,
    RightSuper = 347,
    Menu = 348
}

//
// Summary:
//     Strongly-typed enumeration describing mouse buttons.
public enum MouseButton {
    //
    // Summary:
    //     Mouse button 1.
    //     Same as GLFW.MouseButton.Left.
    Button1 = 0,
    //
    // Summary:
    //     Mouse button 2.
    //     Same as GLFW.MouseButton.Right.
    Button2 = 1,
    //
    // Summary:
    //     Mouse button 3.
    //     Same as GLFW.MouseButton.Middle.
    Button3 = 2,
    //
    // Summary:
    //     Mouse button 4.
    Button4 = 3,
    //
    // Summary:
    //     Mouse button 4.
    Button5 = 4,
    //
    // Summary:
    //     Mouse button 5.
    Button6 = 5,
    //
    // Summary:
    //     Mouse button 6.
    Button7 = 6,
    //
    // Summary:
    //     Mouse button 7.
    Button8 = 7,
    //
    // Summary:
    //     The left mouse button.
    //     Same as GLFW.MouseButton.Button1.
    Left = 0,
    //
    // Summary:
    //     The right mouse button.
    //     Same as GLFW.MouseButton.Button2.
    Right = 1,
    //
    // Summary:
    //     The middle mouse button.
    //     Same as GLFW.MouseButton.Button3.
    Middle = 2
}