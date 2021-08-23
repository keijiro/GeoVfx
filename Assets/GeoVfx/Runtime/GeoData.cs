using UnityEngine;

namespace GeoVfx {

[PreferBinarySerialization]
public sealed class GeoData : ScriptableObject
{
    // Serialized point array
    [SerializeField] Vector3[] _pointArray;

    // Public accessor to the internal GraphicBuffer instance
    public GraphicsBuffer Buffer => _buffer ?? (_buffer = CreateBuffer());

    // Internal GraphicsBuffer instance
    GraphicsBuffer _buffer;

    // GraphicsBuffer initialization
    GraphicsBuffer CreateBuffer()
    {
        var buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,
                                        _pointArray.Length, sizeof(float) * 3);
        buffer.SetData(_pointArray);
        return buffer;
    }

    // OnDisable (ScriptableObject implementation)
    void OnDisable()
    {
        if (_buffer != null) _buffer.Dispose();
        _buffer = null;
    }

    // Asset instantiation method (used from the scripted importer)
    public static GeoData CreateAsset(Vector3[] source)
    {
        var asset = ScriptableObject.CreateInstance<GeoData>();
        asset._pointArray = source;
        return asset;
    }
}

} // namespace GeoVfx
