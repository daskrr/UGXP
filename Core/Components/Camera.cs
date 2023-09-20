using OpenTK.Mathematics;
using UGXP.Core.Render;
using UGXP.Game;
using UGXP.Reference;
using UGXP.Tools;
using UGXP.Util;

namespace UGXP.Core.Components;
public class Camera : Component, IReferenceable<Camera>
{
    public enum RenderTarget {
        /// <summary>
        /// Outputs the camera view to the screen/window
        /// </summary>
        SCREEN,
        /// <summary>
        /// Outputs the camera view to a texture.
        /// </summary>
        TEXTURE
    }

    /// <summary>
    /// The main camera that outputs to the window.
    /// </summary>
    public static Camera Main { get; private set; }

    /// <summary>
    /// What to do with what this camera sees.
    /// </summary>
    public RenderTarget renderTarget = RenderTarget.SCREEN;
    /// <summary>
    /// The texture to output the camera view to.<br/>
    /// The <see cref="renderTarget"/> must be <see cref="RenderTarget.TEXTURE"/>!
    /// </summary>
    public Texture2D renderTexture = null;
    // /\ TODO IMPLEMENT

    /// <summary>
    /// The color of the background.
    /// </summary>
    public Color32 background = new Color32(38, 77, 153, 255);

    private float _orthoSize = 5f;
    /// <summary>
    /// This value represents half of the vertical size in game units.
    /// </summary>
    public float orthographicSize {
        get {
            return _orthoSize;
        }
        set {
            _orthoSize = value;
            RefreshUnitSize();
        }
    }
    /// <summary>
    /// The render range of the camera. Everything between these planes (on the Z axis) is rendered.<br/>
    /// The order is X: near depth plane; Y: far depth plane
    /// <i>This is a formality as all game objects are defaulted to a Z coordinate of 0f.</i> (but is used for X and Y rotations!)<br/>
    /// The default planes are: -10, 1000 (the near plane is -10 so that Y rotated sprites will show. Decrease the near plane in order to allow bigger sprites to show in the camera view and not clip through the screen) <br/>
    /// (keep in mind that the planes are automatically inverted as the camera is looking towards -Z!)<br/>
    /// </summary>
    public Vector2 clippingPlanes = new Vector2(-10f, 1000f);

    private Vector2 _unitSize = null;
    /// <summary>
    /// Gets the size of the screen in game units as width (x) and height (y).
    /// </summary>
    public Vector2 unitSize {
        get {
            if (_unitSize == null) {
                Vector2 screenSize = GameProcess.Context.windowSize;
                float aspect = screenSize.x / screenSize.y;

                float heightUnits = orthographicSize;
                float widthUnits = heightUnits * aspect;

                _unitSize = new Vector2(widthUnits, heightUnits);
            }

            return _unitSize;
        }
    }

    private void Awake() {
        if (Main == null && renderTarget == RenderTarget.SCREEN)
            Main = this;
    }

    // TESTING
    private void Update() {
        //Gizmos.DrawRect(Vector2.zero, new Vector2(5, 5));
        //Gizmos.DrawCircle(Vector2.zero - 2, 3);
        //Gizmos.DrawRect(Vector2.zero + 2, new Vector2(5, 5) + 2);
        //Gizmos.DrawRect(Vector2.zero - 2.5f, new Vector2(5, 5) - 2.5f);
        //Gizmos.DrawCircle(Vector2.zero - 5, 3);
    }

    /// <summary>
    /// Creates a view matrix for this camera, applying the camera's position and looking towards negative Z.
    /// </summary>
    /// <returns></returns>
    public Matrix4 GetViewMatrix() {
        return Matrix4.LookAt(transform.position.ToTKVec3(), transform.position.ToTKVec3() - Vector3.UnitZ, Vector3.UnitY);
    }

    /// <summary>
    /// Creates an orhographic projection matrix
    /// </summary>
    /// <param name="refresh">Whether to recalculate the unit size</param>
    /// <returns>The projection matrix</returns>
    internal Matrix4 GetOrthographicProjection(bool refresh = false) {
        if (refresh)
            RefreshUnitSize();

        // commented code is due to me forgetting there is a view matrix.
        // the view matrix handles the camera's position and rotation (which is none, and points towards -Z)
        Vector4 rect = new(
            /*transform.position.x*/ - unitSize.x,
            /*transform.position.x +*/ unitSize.x,
            /*transform.position.y*/ - unitSize.y,
            /*transform.position.y +*/ unitSize.y
        );

        return Matrix4.CreateOrthographicOffCenter(
            //  left        right
            rect.X,    rect.Y,
            //  bottom       top
            rect.Z,    rect.W,

            //    near              far
            -clippingPlanes.x, -clippingPlanes.y
        );
    }

    internal void RefreshUnitSize() {
        _unitSize = null;
        _ = unitSize;
    }

    protected new ObjectReference<Camera> reference;
    ObjectReference<Camera> IReferenceable<Camera>.GetReference() {
        reference ??= ReferenceManager.Create<Camera>(referenceName, this);
        return reference;
    }
}
