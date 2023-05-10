using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationSpawn : MonoBehaviour
{

    Material material1;
    Material material2;
    [SerializeField] MeshRenderer mr1;
    [SerializeField] MeshRenderer mr2;
    [SerializeField] AnimationCurve curve1;
    [SerializeField] AnimationCurve curve2;
    [SerializeField] string propertyName = "_grow";
    [SerializeField] float duration1 = 1f;
    [SerializeField] float duration2 = 1f;
    [SerializeField] float predelay = 7f;

    private void Start()
    {
        transform.SetParent(null);
        material1 = mr1.material;
        material2 = mr2.material;
        material1.SetFloat(propertyName, 0);
        material2.SetFloat(propertyName, 0);
        StartCoroutine(LerpMaterial(material1, curve1, duration1));
        StartCoroutine(LerpMaterial(material2, curve2, duration2));
    }

    IEnumerator LerpMaterial(Material material, AnimationCurve curve, float duration)
    {

        yield return new WaitForSeconds(predelay);
        float time = 0f;

        while (time <= duration)
        {
            float t = time / duration;
            float value = curve.Evaluate(t);
            material.SetFloat(propertyName, value);

            time += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDisable()
    {
        material1.SetFloat(propertyName, 0);
        material2.SetFloat(propertyName, 0);
    }
}

