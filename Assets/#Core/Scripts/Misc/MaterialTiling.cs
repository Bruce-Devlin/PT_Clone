using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTiling : MonoBehaviour
{
    //The X value to tile the material with.
    [Tooltip("The X value to be used for tiling. (This should be the Scale X used if stretching the Prefab.)")]
    public float MaterialTilingX = 1f;

    //The Y value to tile the material with. (Named "Z" to match with the scaling of 3D objects.)
    [Tooltip("The Z value to be used for tiling. (This should be the Scale Z used if stretching the Prefab.)")]
    public float MaterialTilingZ = 1f;

    //Holds the "floor" child of the Prefab
    GameObject floor;

    // Start is called before the first frame update
    void Start()
    {
        //Get the Child "floor" GameObject.
        floor = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //If the floors scale is ever changed.
        if (floor.GetComponent<Renderer>().material.mainTextureScale != new Vector2(MaterialTilingX, MaterialTilingZ))
        {
            //We update the tiling of the material.
            floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(MaterialTilingX, MaterialTilingZ);
        }
    }
}
