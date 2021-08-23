using UnityEngine;
using UnityEditor;
using System.IO;

namespace GeoVfx {

[UnityEditor.AssetImporters.ScriptedImporter(1, "geodata")]
class GeoDataImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    #region ScriptedImporter implementation

    public override void
      OnImportAsset(UnityEditor.AssetImporters.AssetImportContext context)
    {
        var data = ImportGeoData(context.assetPath);
        if (data == null) return;

        context.AddObjectToAsset("data", data);
        context.SetMainObject(data);
    }

    #endregion

    #region Reader implementation

    GeoData ImportGeoData(string path)
    {
        try
        {
            var stream = File.Open
              (path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var reader = new BinaryReader(stream);

            // Original map dimensions
            var dims = (x: reader.ReadUInt16(), y: reader.ReadUInt16());
            var scale = (x: 1.0f / dims.x, y: 1.0f / dims.y);

            // Element count
            var count = reader.ReadUInt32();

            // Element readout
            var array = new Vector3[count];
            for (var i = 0; i < count; i++)
            {
                var px = reader.ReadUInt16() * scale.x;
                var py = reader.ReadUInt16() * scale.y;
                var data = reader.ReadSingle();
                array[i] = new Vector3(px, py, data);
            }

            return GeoData.CreateAsset(array);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed importing {path}. {e.Message}");
            return null;
        }
    }

    #endregion
}

} // namespace GeoVfx
