//#define USE_FMOD_AUDIO
#define STRETCH_ON_RESIZE

using System.Drawing;
using GLFW;
using OpenGL;
using UGXP.Game;

namespace UGXP.Core
{

    class WindowSize
    {
        public static WindowSize instance = new WindowSize();
        public int width, height;
    }

    public class GLContext
    {

        const int MAXKEYS = 65535;
        const int MAXBUTTONS = 255;

        //private static bool[] keys = new bool[MAXKEYS + 1];
        //private static bool[] keydown = new bool[MAXKEYS + 1];
        //private static bool[] keyup = new bool[MAXKEYS + 1];
        //private static bool[] buttons = new bool[MAXBUTTONS + 1];
        //private static bool[] mousehits = new bool[MAXBUTTONS + 1];
        //private static bool[] mouseup = new bool[MAXBUTTONS + 1]; //mouseup kindly donated by LeonB

        private static InputState[] keys = new InputState[MAXKEYS + 1];
        private static InputState[] mouseButtons = new InputState[MAXKEYS + 1];

        private static int keyPressedCount = 0;
        private static bool anyKeyDown = false;

        public static double mouseX = 0D;
        public static double mouseY = 0D;

        private Window window;
        private GameProcess _owner;
        private static ISoundSystem _soundSystem;

        private int _targetFrameRate = 60;
        private long _lastFrameTime = 0;
        private long _lastFPSTime = 0;
        private int _frameCount = 0;
        private int _lastFPS = 0;
        private bool _vsyncEnabled = false;

        private static double _realToLogicWidthRatio;
        private static double _realToLogicHeightRatio;

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public GLContext(GameProcess owner)
        {
            _owner = owner;
            _lastFPS = _targetFrameRate;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Width
        //------------------------------------------------------------------------------------------------------------------------
        public int width
        {
            get { return WindowSize.instance.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Height
        //------------------------------------------------------------------------------------------------------------------------
        public int height
        {
            get { return WindowSize.instance.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SoundSystem
        //------------------------------------------------------------------------------------------------------------------------
        public static ISoundSystem soundSystem
        {
            get {
                if (_soundSystem == null) {
                    InitializeSoundSystem();
                }

                return _soundSystem;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														setupWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public void CreateWindow(string name, int width, int height, GameSettings.FullscreenMode fullScreen, bool vSync, int realWidth, int realHeight)
        {
            // This stores the "logical" width, used by all the game logic:
            WindowSize.instance.width = width;
            WindowSize.instance.height = height;
            _realToLogicWidthRatio = (double)realWidth / width;
            _realToLogicHeightRatio = (double)realHeight / height;
            _vsyncEnabled = vSync;

            //GL.glfwInit();
            Glfw.Init();

            //GL.glfwOpenWindowHint(GL.GLFW_FSAA_SAMPLES, 8);
            Glfw.WindowHint(Hint.Samples, 8);
            Glfw.WindowHint(Hint.RedBits, 8);
            Glfw.WindowHint(Hint.GreenBits, 8);
            Glfw.WindowHint(Hint.BlueBits, 8);
            Glfw.WindowHint(Hint.AlphaBits, 8);
            Glfw.WindowHint(Hint.DepthBits, 24);
            Glfw.WindowHint(Hint.StencilBits, 0);

            // removed because it caused the game to take over the primary monitor when in full screen, not allowing anything to display over it or minimizing
            //Glfw.WindowHint(Hint.AutoIconify, Constants.False);

            //GL.glfwOpenWindow(realWidth, realHeight, 8, 8, 8, 8, 24, 0, (fullScreen ? GL.GLFW_FULLSCREEN : GL.GLFW_WINDOWED));
            if (fullScreen == GameSettings.FullscreenMode.FULLSCREEN)
                window = Glfw.CreateWindow(realWidth, realHeight, name, Glfw.PrimaryMonitor, Window.None);
            else if (fullScreen == GameSettings.FullscreenMode.BORDERLESS) { 
                Glfw.WindowHint(Hint.Decorated, false);
                window = Glfw.CreateWindow(realWidth, realHeight, name, GLFW.Monitor.None, Window.None);
            }
            else { 
                Glfw.WindowHint(Hint.Decorated, true);
                window = Glfw.CreateWindow(realWidth, realHeight, name, GLFW.Monitor.None, Window.None);
            }
            //GL.glfwSetWindowTitle("Game");
            //GL.glfwSwapInterval(vSync);
            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(vSync ? 1 : 0);

            // import GL from GLFW for bindings
            GL.Import(Glfw.GetProcAddress);

            //GL.glfwSetKeyCallback(
            //    (int _key, int _mode) => {
            //        bool press = (_mode == 1);
            //        if (press) { keydown[_key] = true; anyKeyDown = true; keyPressedCount++; }
            //        else { keyup[_key] = true; keyPressedCount--; }
            //        keys[_key] = press;
            //    });

            Glfw.SetKeyCallback(window, (window, key, scanCode, state, mods) =>
            {
                if (state == InputState.Press || state == InputState.Repeat)
                {
                    anyKeyDown = true;
                    keyPressedCount++;
                }
                else
                    keyPressedCount--;

                keys[(int)key] = state;
            });

            Glfw.SetMouseButtonCallback(window, (window, button, state, modifiers) =>
            {
                //bool press = (_mode == 1);
                //if (press) mousehits[_button] = true;
                //else mouseup[_button] = true;
                //buttons[_button] = press;

                mouseButtons[(int)button] = state;
            });

            //GL.glfwSetMouseButtonCallback(
            //    (int _button, int _mode) => {
            //        bool press = (_mode == 1);
            //        if (press) mousehits[_button] = true;
            //        else mouseup[_button] = true;
            //        buttons[_button] = press;
            //    });

            //    GL.glfwSetWindowSizeCallback((int newWidth, int newHeight) => {
            //        GL.Viewport(0, 0, newWidth, newHeight);
            //        GL.Enable(GL.MULTISAMPLE);
            //        GL.Enable(GL.TEXTURE_2D);
            //        GL.Enable(GL.BLEND);
            //        GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            //        GL.Hint(GL.PERSPECTIVE_CORRECTION, GL.FASTEST);
            //        //GL.Enable (GL.POLYGON_SMOOTH);
            //        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            //        // Load the basic projection settings:
            //        GL.MatrixMode(GL.PROJECTION);
            //        GL.LoadIdentity();

            //        #if STRETCH_ON_RESIZE
            //            _realToLogicWidthRatio = (double)newWidth / WindowSize.instance.width;
            //            _realToLogicHeightRatio = (double)newHeight / WindowSize.instance.height;
            //        #endif
            //        // Here's where the conversion from logical width/height to real width/height happens: 
            //        GL.Ortho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);
            //        #if !STRETCH_ON_RESIZE
            //lock (WindowSize.instance) {
            // WindowSize.instance.width = (int)(newWidth/_realToLogicWidthRatio);
            // WindowSize.instance.height = (int)(newHeight/_realToLogicHeightRatio);
            //}
            //        #endif

            //        if (GameProcess.Main != null) {
            //            GameProcess.Main.RenderRange = new Rectangle(0, 0, WindowSize.instance.width, WindowSize.instance.height);
            //        }
            //    });

            Glfw.SetWindowSizeCallback(window, (window, width, height) =>
            {
                GL.glViewport(0, 0, width, height);
                GL.glEnable(GL.GL_MULTISAMPLE);
                GL.glEnable(GL.GL_TEXTURE_2D);
                GL.glEnable(GL.GL_BLEND);
                GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
                GL.glHint(GL.PERSPECTIVE_CORRECTION, GL.GL_FASTEST);
                //GL.glEnable (GL.GL_POLYGON_SMOOTH);
                GL.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);

                // Load the basic projection settings:
                GL.glMatrixMode(GL.GL_PROJECTION);
                GL.glLoadIdentity();

#if STRETCH_ON_RESIZE
                _realToLogicWidthRatio = (double)width / WindowSize.instance.width;
                _realToLogicHeightRatio = (double)height / WindowSize.instance.height;
#endif
                // Here's where the conversion from logical width/height to real width/height happens: 
                LegacyGL.Ortho(0.0f, width / _realToLogicWidthRatio, height / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);
#if !STRETCH_ON_RESIZE
				    lock (WindowSize.instance) {
					    WindowSize.instance.width = (int)(width/_realToLogicWidthRatio);
					    WindowSize.instance.height = (int)(height/_realToLogicHeightRatio);
				    }
#endif

                if (GameProcess.Main != null)
                {
                    GameProcess.Main.RenderRange = new Rectangle(0, 0, WindowSize.instance.width, WindowSize.instance.height);
                }
            });

            InitializeSoundSystem();
        }

        private static void InitializeSoundSystem()
        {
#if USE_FMOD_AUDIO
			    _soundSystem = new FMODSoundSystem();
#else
            _soundSystem = new SoloudSoundSystem();
#endif

            _soundSystem.Init();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowCursor()
        //------------------------------------------------------------------------------------------------------------------------
        public void ShowCursor(bool enable)
        {
            if (enable)
            {
                //GL.glfwEnable(GL.GLFW_MOUSE_CURSOR);
                Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Normal);
            }
            else
            {
                //GL.glfwDisable(GL.GLFW_MOUSE_CURSOR);
                Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Normal);
            }
        }

        public void SetVSync(bool enableVSync)
        {
            _vsyncEnabled = enableVSync;
            //GL.glfwSwapInterval(_vsyncEnabled);
            Glfw.SwapInterval(_vsyncEnabled ? 1 : 0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetScissor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetScissor(int x, int y, int width, int height)
        {
            if (width == WindowSize.instance.width && height == WindowSize.instance.height)
            {
                //GL.Disable(GL.SCISSOR_TEST);
                GL.glDisable(GL.GL_SCISSOR_TEST);
            }
            else
            {
                //GL.Enable(GL.SCISSOR_TEST);
                GL.glEnable(GL.GL_SCISSOR_TEST);
            }

            GL.glScissor(
                (int)(x * _realToLogicWidthRatio),
                (int)(y * _realToLogicHeightRatio),
                (int)(width * _realToLogicWidthRatio),
                (int)(height * _realToLogicHeightRatio)
            );
            //GL.glScissor(x, y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Close()
        //------------------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            _soundSystem.Deinit();
            //GL.glfwCloseWindow();
            Glfw.SetWindowShouldClose(window, true);
            //GL.glfwTerminate();
            Glfw.Terminate();
            Environment.Exit(0);

            // add GameProcess.Close() method to do cleanup
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Run()
        //------------------------------------------------------------------------------------------------------------------------
        public void Run()
        {
            //GL.glfwSetTime(0.0);
            Glfw.Time = 0.0D;
            do
            {
                if (_vsyncEnabled || Time.time - _lastFrameTime > 1000 / _targetFrameRate)
                {
                    _lastFrameTime = Time.time;

                    //actual fps count tracker
                    _frameCount++;
                    if (Time.time - _lastFPSTime > 1000)
                    {
                        _lastFPS = (int)(_frameCount / ((Time.time - _lastFPSTime) / 1000.0f));
                        _lastFPSTime = Time.time;
                        _frameCount = 0;
                    }

                    UpdateMouseInput();
                    _owner.Step();
                    _soundSystem.Step();

                    ResetHitCounters();

                    // TODO move to different thread
                    Display();

                    Time.newFrame();
                    //GL.glfwPollEvents();
                    Glfw.PollEvents();
                }


            //} while (GL.glfwGetWindowParam(GL.GLFW_ACTIVE) == 1);
            //} while (Glfw.GetWindowAttribute(window, WindowAttribute.Focused)); // changed due to clicking outside window closed the process :S
            } while (!Glfw.WindowShouldClose(window));

            // the process is being terminated
            this.Close();
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														display()
        //------------------------------------------------------------------------------------------------------------------------
        private void Display()
        {
            //GL.Clear(GL.COLOR_BUFFER_BIT);
            GL.glClear(GL.GL_COLOR_BUFFER_BIT);

            //GL.MatrixMode(GL.MODELVIEW);
            GL.glMatrixMode(LegacyGL.MODELVIEW);
            GL.glLoadIdentity();

            _owner.Render();

            //GL.glfwSwapBuffers();
            Glfw.SwapBuffers(window);

            // why is this a thing??? 
            // Alt+F4 can be used to close the game, or specific buttons
            // this functionality imposes itself onto the user's options on using the ESC key
            // REMOVED
            //if (GetKey(Key.ESCAPE)) this.Close();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetColor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetColor(byte r, byte g, byte b, byte a)
        {
            LegacyGL.Color4ub(r, g, b, a);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PushMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PushMatrix(float[] matrix)
        {
            //GL.glPushMatrix();
            LegacyGL.PushMatrix();
            //GL.MultMatrixf(matrix);
            LegacyGL.MultMatrixf(matrix);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PopMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PopMatrix()
        {
            //GL.PopMatrix();
            LegacyGL.PopMatrix();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawQuad()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawQuad(float[] vertices, float[] uv)
        {
            //GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            LegacyGL.EnableClientState(LegacyGL.TEXTURE_COORD_ARRAY);
            //GL.EnableClientState(GL.VERTEX_ARRAY);
            LegacyGL.EnableClientState(LegacyGL.VERTEX_ARRAY);
            //GL.TexCoordPointer(2, GL.FLOAT, 0, uv);
            LegacyGL.TexCoordPointer(2, GL.GL_FLOAT, 0, uv);
            //GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            LegacyGL.VertexPointer(2, GL.GL_FLOAT, 0, vertices);
            //GL.DrawArrays(GL.QUADS, 0, 4);
            GL.glDrawArrays(LegacyGL.QUADS, 0, 4);
            //GL.DisableClientState(GL.VERTEX_ARRAY);
            LegacyGL.DisableClientState(LegacyGL.VERTEX_ARRAY);
            //GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);
            LegacyGL.DisableClientState(LegacyGL.TEXTURE_COORD_ARRAY);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKey()
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the specified key is pressed (<see cref="InputState.Repeat"/> || <see cref="InputState.Press"/>)
        /// </summary>
        /// <param name="key">The id of the key (<see cref="Keys"/>)</param>
        /// <param name="strict">Whether to accept <see cref="InputState.Press"/> or not</param>
        /// <returns>true if the key is pressed</returns>
        public static bool GetKey(Keys key, bool strict = false)
        {
            if (strict)
                return keys[(int)key] == InputState.Repeat;

            return keys[(int)key] == InputState.Repeat || keys[(int)key] == InputState.Press;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyDown()
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the specified key is pressed (down) (<see cref="InputState.Press"/>)
        /// </summary>
        /// <param name="key">The id of the key (<see cref="Keys"/>)</param>
        /// <returns>true if the key is pressed</returns>
        public static bool GetKeyDown(Keys key)
        {
            return keys[(int)key] == InputState.Press;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyUp()
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the specified key is released (<see cref="InputState.Release"/>)
        /// </summary>
        /// <param name="key">The id of the key (<see cref="Keys"/>)</param>
        /// <returns>true if the key is released</returns>
        public static bool GetKeyUp(Keys key)
        {
            return keys[(int)key] == InputState.Release;
        }

        public static bool AnyKey()
        {
            return keyPressedCount > 0;
        }

        public static bool AnyKeyDown()
        {
            return anyKeyDown;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButton()
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the specified mouse button is pressed (<see cref="InputState.Repeat"/> || <see cref="InputState.Press"/>)
        /// </summary>
        /// <param name="button">The id of the mouse button (<see cref="MouseButton"/>)</param>
        /// <param name="strict">Whether to accept <see cref="InputState.Press"/> or not</param>
        /// <returns>true if the key is pressed</returns>
        public static bool GetMouseButton(MouseButton button, bool strict = false)
        {
            if (strict)
                return mouseButtons[(int)button] == InputState.Repeat;

            return mouseButtons[(int)button] == InputState.Repeat || mouseButtons[(int)button] == InputState.Press;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonDown()
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the specified mouse button is pressed (down) (<see cref="InputState.Press"/>)
        /// </summary>
        /// <param name="button">The id of the mouse button (<see cref="MouseButton"/>)</param>
        /// <returns>true if the key is pressed</returns>
        public static bool GetMouseButtonDown(MouseButton button)
        {
            return mouseButtons[(int)button] == InputState.Press;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonUp()
        //------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Checks if the specified mouse button is released (<see cref="InputState.Release"/>)
        /// </summary>
        /// <param name="button">The id of the mouse button (<see cref="MouseButton"/>)</param>
        /// <returns>true if the key is released</returns>
        public static bool GetMouseButtonUp(MouseButton button)
        {
            return mouseButtons[(int)button] == InputState.Release;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ResetHitCounters()
        //------------------------------------------------------------------------------------------------------------------------
        public static void ResetHitCounters()
        {
            //Array.Clear(keydown, 0, MAXKEYS);
            //Array.Clear(keyup, 0, MAXKEYS);/**/
            //Array.Clear(mousehits, 0, MAXBUTTONS);
            //Array.Clear(mouseup, 0, MAXBUTTONS);

            Array.Clear(keys, 0, MAXKEYS);
            Array.Clear(mouseButtons, 0, MAXBUTTONS);

            anyKeyDown = false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateMouseInput()
        //------------------------------------------------------------------------------------------------------------------------
        // changed from static to instance (since we need the instance window)
        public /*static*/ void UpdateMouseInput()
        {
            //GL.glfwGetMousePos(out mouseX, out mouseY);
            Glfw.GetCursorPosition(window, out mouseX, out mouseY);
            mouseX = (int)(mouseX / _realToLogicWidthRatio);
            mouseY = (int)(mouseY / _realToLogicHeightRatio);
        }

        public int currentFps
        {
            get { return _lastFPS; }
        }

        public int targetFps
        {
            get { return _targetFrameRate; }
            set
            {
                if (value < 1)
                {
                    _targetFrameRate = 1;
                }
                else
                {
                    _targetFrameRate = value;
                }
            }
        }

    }

}