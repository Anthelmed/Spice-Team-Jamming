using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

[ScriptedImporter(1, "vat")]
public class CombineVatData : ScriptedImporter
{
    [System.Serializable]
    public struct Animation
    {
        public Texture2D vertexVat;
        public Texture2D normalVat;
        public float fps;
        public float duration;
    }

    public Vector2 attackHitRange = Vector2.up;
    public Animation[] animations = new Animation[0];

    public override void OnImportAsset(AssetImportContext ctx)
    {
        if (animations.Length == 0)
        {
            var failImport = new TextAsset("No animations!");
            ctx.AddObjectToAsset("error", failImport);
            ctx.SetMainObject(failImport);
            return;
        }

        var totalHeight = 0;

        foreach (var anim in animations)
        {
            totalHeight += anim.vertexVat.height + 1;
        }

        // Create the combined textures
        var newVertex = new Texture2D(animations[0].vertexVat.width, totalHeight, animations[0].vertexVat.format, false);
        newVertex.name = name + "_vtxVat";
        var newNormal = new Texture2D(animations[0].normalVat.width, totalHeight, animations[0].vertexVat.format, false);
        newNormal.name = name + "_nmlVat";

        // Fill the textures
        int offset = 0;
        foreach (var anim in animations)
        {
            for (int i = 0; i < anim.vertexVat.width; ++i)
            {
                for (int j = 0; j < anim.vertexVat.height; ++j)
                {
                    newVertex.SetPixel(i, j + offset, anim.vertexVat.GetPixel(i, j));
                    newNormal.SetPixel(i, j + offset, anim.normalVat.GetPixel(i, j));
                }
                newVertex.SetPixel(i, anim.vertexVat.height + offset, anim.vertexVat.GetPixel(i, 0));
                newNormal.SetPixel(i, anim.vertexVat.height + offset, anim.normalVat.GetPixel(i, 0));
            }
            offset += anim.vertexVat.height + 1;
        }

        newVertex.Apply(false, true);
        newNormal.Apply(false, true);

        // Create a new object and fill it
        var resultObject = ScriptableObject.CreateInstance<VatData>();
        resultObject.name = name;

        resultObject.vertexVat = newVertex;
        resultObject.normalVat = newNormal;
        resultObject.attackHitRange = attackHitRange;

        resultObject.animations = new Vector4[animations.Length];
        offset = 0;
        for (int i = 0; i < animations.Length; ++i)
        {
            resultObject.animations[i] = new Vector4(animations[i].duration, animations[i].fps, offset);
            offset += animations[i].vertexVat.height + 1;
        }

        // Setup the results for Unity
        ctx.AddObjectToAsset("data", resultObject);
        ctx.AddObjectToAsset("vertex", newVertex);
        ctx.AddObjectToAsset("normal", newNormal);
        ctx.SetMainObject(resultObject);
    }

    [MenuItem("Assets/Create/Combined VAT data")]
    private static void Create()
    {
        ProjectWindowUtil.CreateAssetWithContent("VatData.vat", "Vat mock asset, see meta for contents");
    }
}
