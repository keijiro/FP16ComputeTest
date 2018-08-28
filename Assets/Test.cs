using UnityEngine;

class Test : MonoBehaviour
{
    [SerializeField] int _vectorSize = 1024;
    [SerializeField] ComputeShader _compute;

    ComputeBuffer _vectorA32;
    ComputeBuffer _vectorB32;
    ComputeBuffer _matrixA32;
    ComputeBuffer _matrixB32;

    ComputeBuffer _vectorA16;
    ComputeBuffer _vectorB16;
    ComputeBuffer _matrixA16;
    ComputeBuffer _matrixB16;

    float[] _inputVector;
    float[] _outputVector;
    float[] _dctMatrix;
    float[] _idctMatrix;

    void Start()
    {
        _compute.SetInt("VectorSize", _vectorSize);

        _vectorA32 = new ComputeBuffer(_vectorSize, sizeof(float));
        _vectorB32 = new ComputeBuffer(_vectorSize, sizeof(float));
        _matrixA32 = new ComputeBuffer(_vectorSize * _vectorSize, sizeof(float));
        _matrixB32 = new ComputeBuffer(_vectorSize * _vectorSize, sizeof(float));

        _vectorA16 = new ComputeBuffer(_vectorSize / 2, sizeof(float));
        _vectorB16 = new ComputeBuffer(_vectorSize / 2, sizeof(float));
        _matrixA16 = new ComputeBuffer(_vectorSize * _vectorSize / 2, sizeof(float));
        _matrixB16 = new ComputeBuffer(_vectorSize * _vectorSize / 2, sizeof(float));

        _inputVector  = new float[_vectorSize];
        _outputVector = new float[_vectorSize];
        _dctMatrix  = new float[_vectorSize * _vectorSize];
        _idctMatrix = new float[_vectorSize * _vectorSize];

        // sawtooth wave
        for (var i = 0; i < _vectorSize; i++)
            _inputVector[i] = 2.0f * i / (_vectorSize - 1) - 1;

        // DCT matrix
        var s = Mathf.Sqrt(2.0f / _vectorSize);
        for (var row = 0; row < _vectorSize; row++)
            for (var col = 0; col < _vectorSize; col++)
                _dctMatrix[row * _vectorSize + col] =
                    s * Mathf.Cos((row + 0.5f) * col * Mathf.PI / _vectorSize);

        // IDCT matrix
        s = Mathf.Sqrt(2.0f / _vectorSize);

        for (var col = 0; col < _vectorSize; col++)
            _idctMatrix[col] = s * 0.5f;

        for (var row = 1; row < _vectorSize; row++)
            for (var col = 0; col < _vectorSize; col++)
                _idctMatrix[row * _vectorSize + col] =
                    s * Mathf.Cos(row * (col + 0.5f) * Mathf.PI / _vectorSize);
    }

    void OnDestroy()
    {
        _vectorA32.Release();
        _vectorB32.Release();
        _matrixA32.Release();
        _matrixB32.Release();

        _vectorA16.Release();
        _vectorB16.Release();
        _matrixA16.Release();
        _matrixB16.Release();
    }

    void Update()
    {
        _vectorA32.SetData(_inputVector);
        _matrixA32.SetData(_dctMatrix);
        _matrixB32.SetData(_idctMatrix);

        var kernel = _compute.FindKernel("Encode16");
        _compute.SetBuffer(kernel, "Input32", _vectorA32);
        _compute.SetBuffer(kernel, "Output16", _vectorA16);
        _compute.Dispatch(kernel, _vectorSize / 64, 1, 1);

        _compute.SetBuffer(kernel, "Input32", _matrixA32);
        _compute.SetBuffer(kernel, "Output16", _matrixA16);
        _compute.Dispatch(kernel, _vectorSize * _vectorSize / 64, 1, 1);

        _compute.SetBuffer(kernel, "Input32", _matrixB32);
        _compute.SetBuffer(kernel, "Output16", _matrixB16);
        _compute.Dispatch(kernel, _vectorSize * _vectorSize / 64, 1, 1);

        kernel = _compute.FindKernel("Multiply32");
        _compute.SetBuffer(kernel, "Input32", _vectorA32);
        _compute.SetBuffer(kernel, "Matrix32", _matrixA32);
        _compute.SetBuffer(kernel, "Output32", _vectorB32);
        _compute.Dispatch(kernel, _vectorSize / 32, 1, 1);

        _compute.SetBuffer(kernel, "Input32", _vectorB32);
        _compute.SetBuffer(kernel, "Matrix32", _matrixB32);
        _compute.SetBuffer(kernel, "Output32", _vectorA32);
        _compute.Dispatch(kernel, _vectorSize / 32, 1, 1);

        kernel = _compute.FindKernel("Multiply16");
        _compute.SetBuffer(kernel, "Input16", _vectorA16);
        _compute.SetBuffer(kernel, "Matrix16", _matrixA16);
        _compute.SetBuffer(kernel, "Output16", _vectorB16);
        _compute.Dispatch(kernel, _vectorSize / 64, 1, 1);

        _compute.SetBuffer(kernel, "Input16", _vectorB16);
        _compute.SetBuffer(kernel, "Matrix16", _matrixB16);
        _compute.SetBuffer(kernel, "Output16", _vectorA16);
        _compute.Dispatch(kernel, _vectorSize / 64, 1, 1);

        _vectorA32.GetData(_outputVector);
    }
}
