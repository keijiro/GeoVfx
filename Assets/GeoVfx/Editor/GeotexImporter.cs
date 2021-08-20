using UnityEngine;
using UnityEditor;

using System.IO;

namespace GeoVfx
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "geotex")]
    class GeotexImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        #region ScriptedImporter implementation

        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext context)
        {
            var data = ImportAsPackedPointCloud(context.assetPath);
            if (data != null)
            {
                context.AddObjectToAsset("texture", data);
                context.SetMainObject(data);
            }
        }

        #endregion

        #region Reader implementation

        Texture2D ImportAsPackedPointCloud(string path)
        {
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var reader = new BinaryReader(stream);

                // Original map dimensions
                var dims = (x: reader.ReadUInt16(), y: reader.ReadUInt16());
                var scale = (x: 1.0f / dims.x, y: 1.0f / dims.y);

                // Element count
                var count = reader.ReadUInt32();

                // Container texture
                var width = Mathf.CeilToInt(Mathf.Sqrt(count));
                var texture = new Texture2D(width, width, TextureFormat.RGBAFloat, false);
                texture.name = Path.GetFileNameWithoutExtension(path);
                texture.filterMode = FilterMode.Point;

                // Points
                var i = 0;
                for (var y = 0; y < width; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        if (i++ < count)
                        {
                            var px = reader.ReadUInt16() * scale.x;
                            var py = reader.ReadUInt16() * scale.y;
                            var data = reader.ReadSingle();
                            texture.SetPixel(x, y, new Color(px, py, data, 0));
                        }
                        else
                        {
                            texture.SetPixel(x, y, Color.black);
                        }
                    }
                }

                texture.Apply(false, true);
                return texture;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }
    }

    #endregion
}
