using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ground : MonoBehaviour
{
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        //ger reference 
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        //get the speed from the game manager and make sure to devide by scale of the mesh 
        float speed = GameManager.Instance.gameSpeed / transform.localScale.x;
        //increase to animate moving to the left
        meshRenderer.material.mainTextureOffset += Vector2.right * speed * Time.deltaTime;
    }
}
