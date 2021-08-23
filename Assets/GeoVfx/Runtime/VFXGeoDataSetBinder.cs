using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace GeoVfx {

[AddComponentMenu("VFX/Property Binders/GeoVfx/Data Set Binder")]
[VFXBinder("GeoVfx/Data Set")]
sealed class VFXGeoDataSetBinder : VFXBinderBase
{
    public string Property
      { get => (string)_property; set => _property = value; }

    [VFXPropertyBinding("UnityEngine.GraphicsBuffer"), SerializeField]
    ExposedProperty _property = "DataSet";

    public DataSet Source = null;

    public override bool IsValid(VisualEffect component)
      => Source != null && component.HasGraphicsBuffer(_property);

    public override void UpdateBinding(VisualEffect component)
      => component.SetGraphicsBuffer(_property, Source.buffer);

    public override string ToString()
      => $"Data Set : '{_property}' -> {Source?.name ?? "(null)"}";
}

} // namespace GeoVfx {
