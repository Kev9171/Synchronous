using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnEffect : MonoBehaviour {

    public float spawnEffectTime = 3;
    public float pause = 1;
    public Material material;
    public GameObject prefab;
    public AnimationCurve fadeIn;

    ParticleSystem ps;
    float timer = 0;
    Renderer _renderer;

    int shaderProperty;

	void Start ()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        ps = GetComponentInChildren <ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;

        //ps.Play();

    }
	
	void Update ()
    {
        //if (timer < spawnEffectTime + pause)
        //{
        //    timer += Time.deltaTime;
        //}
        //else
        //{
        //    ps.Play();
        //    timer = 0;
        //}


        //_renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, timer)));

    }

    [PunRPC]
    public void PlayEffect()
    {
        ps.Play();
        material.SetFloat(shaderProperty, fadeIn.Evaluate(Mathf.InverseLerp(0, spawnEffectTime, 3)));
        //Instantiate(prefab, transform.parent);
    }
}
