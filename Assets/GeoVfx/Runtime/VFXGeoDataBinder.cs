using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace GeoVfx {

[AddComponentMenu("VFX/Property Binders/GeoVfx/GeoData Binder")]
[VFXBinder("GeoVfx/GeoData")]
sealed class VFXGeoDataBinder : VFXBinderBase
{
    public string Property
      { get => (string)_property; set => _property = value; }

    [VFXPropertyBinding("UnityEngine.GraphicsBuffer"), SerializeField]
    ExposedProperty _property = "DataSet";

    public GeoData Source = null;

    public override bool IsValid(VisualEffect component)
      => Source != null && component.HasGraphicsBuffer(_property);

    public override void UpdateBinding(VisualEffect component)
      => component.SetGraphicsBuffer(_property, Source.Buffer);

    public override string ToString()
      => $"GeoData : '{_property}' -> {Source?.name ?? "(null)"}";
}

} // namespace GeoVf
