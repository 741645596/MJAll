using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class RTSystem : MonoBehaviour
{
    [Serializable]
    public enum ProjectionType
    {
        Orthographic,
        Perspective
    }

    [Serializable]
    public class CameraSettings
    {
        internal event Action onDirty;

        [SerializeField]
        private Vector3 _position = new Vector3(0, 0, -10);
        public Vector3 position
        {
            get => _position;
            set
            {
                if (_position == value)
                {
                    return;
                }
                _position = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private Vector3 _euler = Vector3.zero;
        public Vector3 euler
        {
            get => _euler;
            set
            {
                if (_euler == value)
                {
                    return;
                }
                _euler = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private Color _backgroundColor = Color.clear;
        public Color backgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value)
                {
                    return;
                }
                _backgroundColor = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private ProjectionType _projectionType = ProjectionType.Perspective;
        public ProjectionType projectionType
        {
            get => _projectionType;
            set
            {
                if (_projectionType == value)
                {
                    return;
                }
                _projectionType = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private float _orthographicSize = 5f;
        public float orthographicSize
        {
            get => _orthographicSize;
            set
            {
                if (_orthographicSize == value)
                {
                    return;
                }
                _orthographicSize = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private float _fieldOfView = 30;
        public float fieldOfView
        {
            get => _fieldOfView;
            set
            {
                if (_fieldOfView == value)
                {
                    return;
                }
                _fieldOfView = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private float _near = 0.01f;
        public float near
        {
            get => _near;
            set
            {
                if (_near == value)
                {
                    return;
                }
                _near = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private float _far = 1000f;
        public float far
        {
            get => _far;
            set
            {
                if (_far == value)
                {
                    return;
                }
                _far = value;
                onDirty?.Invoke();
            }
        }

        [SerializeField]
        private Transform _lookTarget;
        public Transform lookTarget
        {
            get => _lookTarget;
            set
            {
                if (_lookTarget == value)
                {
                    return;
                }
                _lookTarget = value;
                onDirty?.Invoke();
            }
        }

        public void Reset()
        {
            _position = new Vector3(0, 0, -10);
            _euler = Vector3.zero;
            _backgroundColor = Color.clear;
            _projectionType = ProjectionType.Perspective;
            _orthographicSize = 5f;
            _fieldOfView = 30f;
            _near = 0.01f;
            _far = 1000f;
            _lookTarget = null;
        }
    }


    [SerializeField]
    private bool _useDynamicTargetTexture = false;
    public bool useDynamicTargetTexture
    {
        get => _useDynamicTargetTexture;
        set
        {
            if (_useDynamicTargetTexture == value)
            {
                return;
            }
            _useDynamicTargetTexture = value;
            SetupTargetTexture();
        }
    }

    [SerializeField]
    private Vector2Int _dynamicTargetTextureSize = new Vector2Int(256, 256);
    public Vector2Int dynamicTargetTextureSize
    {
        get => _dynamicTargetTextureSize;
        set
        {
            if (_dynamicTargetTextureSize == value)
            {
                return;
            }
            _dynamicTargetTextureSize = new Vector2Int(Mathf.Max(2, value.x), Mathf.Max(2, value.y));
            SetupTargetTexture();
        }
    }

    // 动态RT，不能序列化
    private RenderTexture _dynamicTexture = null;

    [SerializeField]
    private RenderTexture _targetTexture;
    public RenderTexture targetTexture
    {
        get
        {
            if (_useDynamicTargetTexture)
            {
                return _dynamicTexture;
            }
            else
            {
                return _targetTexture;
            }
        }
        set
        {
            if (_targetTexture == value)
            {
                return;
            }
            _targetTexture = value;
            SetupTargetTexture();
        }
    }

    [SerializeField]
    private int _layerIndex = 0;
    public int layerIndex
    {
        get => _layerIndex;
        set
        {
            if (_layerIndex == value)
            {
                return;
            }
            _layerIndex = value;
            SetupLayer();
        }
    }


    [SerializeField]
    private CameraSettings _cameraSettings;
    public CameraSettings cameraSettings
    {
        get
        {
            if (_cameraSettings == null)
            {
                _cameraSettings = new CameraSettings();
            }
            return _cameraSettings;
        }
    }


    [SerializeField]
    [HideInInspector]
    private Camera _camera;


    internal event Action onTextureUpdate;

    private void Awake()
    {
        cameraSettings.onDirty += SetupCamera;

        SetupLayer();

        SetupCamera();

        SetupTargetTexture();
    }

    [ExecuteInEditMode]
    private void LateUpdate()
    {
        MakeCameraLookAtTarget();
    }

    private void SetupLayer()
    {
        if (_camera)
        {
            _camera.cullingMask = LayerMask.GetMask(LayerMask.LayerToName(_layerIndex));
        }

        gameObject.layer = _layerIndex;
        var children = transform.GetAllChildren();
        children.ForEach(c => c.gameObject.layer = gameObject.layer);
    }


    private void SetupTargetTexture()
    {
        if (_useDynamicTargetTexture)
        {
            _dynamicTexture = new RenderTexture(new RenderTextureDescriptor
            {
                dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
                width = _dynamicTargetTextureSize.x,
                height = _dynamicTargetTextureSize.y,
                graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                msaaSamples = 4,
                volumeDepth = 1
            });
        }
        else
        {
            _dynamicTexture = null;
        }

        if (_camera)
        {
            _camera.targetTexture = targetTexture;
        }

        onTextureUpdate?.Invoke();
    }

    private void SetupCamera()
    {
        if (!_camera)
        {
            return;
        }

        _camera.gameObject.hideFlags = HideFlags.HideInHierarchy;
        _camera.transform.localPosition = cameraSettings.position;
        _camera.transform.localEulerAngles = cameraSettings.euler;
        _camera.backgroundColor = cameraSettings.backgroundColor;
        _camera.orthographic = cameraSettings.projectionType == ProjectionType.Orthographic;
        _camera.orthographicSize = cameraSettings.orthographicSize;
        _camera.fieldOfView = cameraSettings.fieldOfView;
        _camera.nearClipPlane = cameraSettings.near;
        _camera.farClipPlane = cameraSettings.far;
        MakeCameraLookAtTarget();
    }

    private void MakeCameraLookAtTarget()
    {
        if (!_camera)
        {
            return;
        }

        if (_cameraSettings == null)
        {
            return;
        }

        if (!_cameraSettings.lookTarget)
        {
            return;
        }

        _camera.transform.LookAt(_cameraSettings.lookTarget); 

        _cameraSettings.euler = _camera.transform.localEulerAngles;
    }


#if UNITY_EDITOR
    internal void OnValidate()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(IEDelayApply());
    }

    private IEnumerator IEDelayApply()
    {
        yield return null;

        SetupLayer();

        SetupCamera();

        SetupTargetTexture();
    }

    private void Reset()
    {
        cameraSettings?.Reset();
        useDynamicTargetTexture = false;
        dynamicTargetTextureSize = new Vector2Int(256, 256);
        _dynamicTexture = null;
        targetTexture = null;
        layerIndex = 0;
        OnValidate();
    }
#endif

    public static GameObject CreateTemplate()
    {
        var obj = new GameObject("RTSystem");
        var sys = obj.AddComponent<RTSystem>();

        var camObj = new GameObject("camera");
        camObj.transform.SetParent(obj.transform);
        camObj.hideFlags = HideFlags.HideInHierarchy;
        sys._camera = camObj.AddComponent<Camera>();
        sys._camera.clearFlags = CameraClearFlags.SolidColor;
        sys._camera.backgroundColor = Color.clear;
        sys._cameraSettings = new CameraSettings();

        return obj;
    }
}
