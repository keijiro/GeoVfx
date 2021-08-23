using UnityEngine;

namespace GeoVfx {

[PreferBinarySerialization]
public sealed class DataSet : ScriptableObject
{
    [SerializeField] Vector3[] _data;

    GraphicsBuffer _buffer;

    public GraphicsBuffer buffer => _buffer;

    void OnEnable()
    {
        if (_buffer == null) _buffer = CreateBuffer();
    }

    void OnDisable()
    {
        if (_buffer != null) _buffer.Dispose();
        _buffer = null;
    }

    public GraphicsBuffer CreateBuffer()
    {
        var buffer = new GraphicsBuffer
          (GraphicsBuffer.Target.Structured, _data.Length, sizeof(float) * 3);
        buffer.SetData(_data);
        return buffer;
    }

    public static DataSet CreateAsset(Vector3[] source)
    {
        var asset = ScriptableObject.CreateInstance<DataSet>();
        asset._data = source;
        return asset;
    }
}

} // namespace GeoVfx {
