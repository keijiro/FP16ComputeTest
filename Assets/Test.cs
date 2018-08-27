using UnityEngine;

class Test : MonoBehaviour
{
    [SerializeField] int _vectorSize = 1024;
    [SerializeField] ComputeShader _compute;

    ComputeBuffer _vector32;
    ComputeBuffer _vector16;

    ComputeBuffer _matrix32;
    ComputeBuffer _matrix16;

    ComputeBuffer _output32;
    ComputeBuffer _output16;

    void Start()
    {
        _vector32 = new ComputeBuffer(_vectorSize    , sizeof(float));
        _vector16 = new ComputeBuffer(_vectorSize / 2, sizeof(uint));

        _matrix32 = new ComputeBuffer(_vectorSize * _vectorSize    , sizeof(float));
        _matrix16 = new ComputeBuffer(_vectorSize * _vectorSize / 2, sizeof(uint));

        _output32 = new ComputeBuffer(_vectorSize    , sizeof(float));
        _output16 = new ComputeBuffer(_vectorSize / 2, sizeof(uint));

        _compute.SetInt("VectorSize", _vectorSize);

        var vector = new float[_vectorSize];
        var matrix = new float[_vectorSize * _vectorSize];

        // sawtooth wave
        for (var i = 0; i < _vectorSize; i++)
            vector[i] = 2.0f * i / (_vectorSize - 1) - 1;

        // cosine bases
        for (var row = 0; row < _vectorSize; row++)
            for (var col = 0; col < _vectorSize; col++)
                matrix[row * _vectorSize + col] =
                    Mathf.Cos(Mathf.PI / _vectorSize * (row + 0.5f) * col);

        _vector32.SetData(vector);
        _matrix32.SetData(matrix);
    }

    void OnDestroy()
    {
        _vector16.Release();
        _vector32.Release();

        _matrix16.Release();
        _matrix32.Release();

        _output16.Release();
        _output32.Release();
    }

    void Update()
    {
        var kernel = _compute.FindKernel("Convert");
        _compute.SetBuffer(kernel, "Input32", _vector32);
        _compute.SetBuffer(kernel, "Output16", _vector16);
        _compute.Dispatch(kernel, _vectorSize / 32 / 2, 1, 1);

        _compute.SetBuffer(kernel, "Input32", _matrix32);
        _compute.SetBuffer(kernel, "Output16", _matrix16);
        _compute.Dispatch(kernel, _vectorSize * _vectorSize / 32 / 2, 1, 1);

        kernel = _compute.FindKernel("Integrate32");
        _compute.SetBuffer(kernel, "Input32", _vector32);
        _compute.SetBuffer(kernel, "Matrix32", _matrix32);
        _compute.SetBuffer(kernel, "Output32", _output32);
        _compute.Dispatch(kernel, _vectorSize / 32, 1, 1);

        kernel = _compute.FindKernel("Integrate16");
        _compute.SetBuffer(kernel, "Input16", _vector16);
        _compute.SetBuffer(kernel, "Matrix16", _matrix16);
        _compute.SetBuffer(kernel, "Output16", _output16);
        _compute.Dispatch(kernel, _vectorSize / 32 / 2, 1, 1);
    }
}
