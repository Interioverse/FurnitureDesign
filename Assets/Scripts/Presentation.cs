using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Presentation : MonoBehaviourPun
{
    [SerializeField] GameObject cube, productCard, content;
    [SerializeField] GameObject scrollView, nothingToDisplay;
    [SerializeField] Transform designHolder;
    int id, selectedProduct;
    public Material[] laminateMaterials;

    void Start()
    {
        selectedProduct = 0;
    }

    public void GetAllMyProducts(JObject jsonObj, string productsData)
    {
        if (!designHolder)
        {
            designHolder = GameObject.Find("Product Spawner").transform;
        }
        if (laminateMaterials.Length <= 0)
        {
            laminateMaterials = UniversalData.Instance.laminatesMaterials;
        }

        JArray productsArrary = (JArray)jsonObj["Products"];

        if (productsArrary.Count > 0)
        {
            scrollView.SetActive(true);
            nothingToDisplay.SetActive(false);
            foreach (var product in productsArrary)
            {
                GameObject card = Instantiate(productCard, content.transform) as GameObject;
                card.name = (string)product["productName"];
                card.GetComponent<Button>().onClick.AddListener(() => Present((int)product["id"], product));
                card.GetComponentInChildren<TextMeshProUGUI>().text = (string)product["productName"];
            }
        }
        else
        {
            id = 0;
            scrollView.SetActive(false);
            nothingToDisplay.SetActive(true);
        }
    }

    public void StopPresentation()
    {
        if (!designHolder)
        {
            designHolder = GameObject.Find("Product Spawner").transform;
        }

        selectedProduct = 0;

        if (designHolder.childCount > 0)
        {
            foreach (Transform item in designHolder)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }

    int chunkSize = 32760;

    void Present(int value, JToken _product)
    {
        if (selectedProduct == value)
        {
            return;
        }
        else
        {
            string stringProduct = _product.ToString();
            Encoding encoding = Encoding.UTF8; // Choose the encoding you want to use
            byte[] bytes = encoding.GetBytes(stringProduct);
            int byteSize = bytes.Length;

            if (byteSize < 32760)
            {
                photonView.RPC("PresentThisDesign", RpcTarget.All, value, bytes);
            }
            else
            {
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(stringProduct);

                for (int i = 0; i < utf8Bytes.Length; i += chunkSize)
                {
                    int remainingBytes = Mathf.Min(chunkSize, utf8Bytes.Length - i);
                    byte[] chunk = new byte[remainingBytes];
                    Array.Copy(utf8Bytes, i, chunk, 0, remainingBytes);

                    // Send the chunk over the network using Photon RPC
                    photonView.RPC("ReceiveChunk", RpcTarget.All, i, chunk);
                }

                photonView.RPC("PresentThisDesign", RpcTarget.All, value, receivedData);
            }
        }
    }

    private byte[] receivedData;

    [PunRPC]
    public void ReceiveChunk(int i, byte[] chunk)
    {
        if (i == 0)
        {
            receivedData = null;
        }

        // Append the received chunk to the data
        if (receivedData == null)
        {
            receivedData = chunk;
        }
        else
        {
            byte[] newData = new byte[receivedData.Length + chunk.Length];
            Array.Copy(receivedData, newData, receivedData.Length);
            Array.Copy(chunk, 0, newData, receivedData.Length, chunk.Length);
            receivedData = newData;
        }
    }

    //[PunRPC]
    //public void DataTransferCompleted()
    //{
    //    Debug.Log(receivedData.Length);
    //    string receivedString = Encoding.UTF8.GetString(receivedData);
    //}

    [PunRPC]
    public void PresentThisDesign(int value, byte[] productBytes)
    {
        //if (selectedProduct == value)
        //{
        //    return;
        //}
        if (!designHolder)
        {
            designHolder = GameObject.Find("Product Spawner").transform;
        }
        if (laminateMaterials.Length <= 0)
        {
            laminateMaterials = UniversalData.Instance.laminatesMaterials;
        }

        if (designHolder.childCount > 0)
        {
            foreach (Transform item in designHolder)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        Encoding encoding = Encoding.UTF8;
        string restoreString = encoding.GetString(productBytes);
        JToken product = JToken.Parse(restoreString);

        id = value;
        selectedProduct = id;
        JArray componentsArrary = (JArray)product["components"];
        GameObject furniture = new GameObject();
        furniture.transform.parent = designHolder;

        foreach (var element in componentsArrary)
        {
            GameObject newCube = Instantiate(cube, furniture.transform) as GameObject;
            furniture.name = product["productName"].ToString();
            newCube.transform.localPosition = new Vector3(element["positionValue"]["x"].ToObject<float>(), element["positionValue"]["y"].ToObject<float>(), element["positionValue"]["z"].ToObject<float>());
            newCube.transform.localEulerAngles = new Vector3(element["rotationValues"]["x"].ToObject<float>(), element["rotationValues"]["y"].ToObject<float>(), element["rotationValues"]["z"].ToObject<float>());
            newCube.transform.localScale = new Vector3(element["scaleValues"]["x"].ToObject<float>(), element["scaleValues"]["y"].ToObject<float>(), element["scaleValues"]["z"].ToObject<float>());

            Vector3 originalRotation = newCube.transform.localEulerAngles;
            newCube.transform.localEulerAngles = Vector3.zero;
            CubeFacePlanes cubeFacePlanes = newCube.GetComponent<CubeFacePlanes>();
            cubeFacePlanes.planesCreated = true;
            JObject componentObj = (JObject)element;
            JArray smMaterialPropertiesArray = (JArray)componentObj["smMaterialProperties"];

            // Access the details within the smMaterialProperties array
            foreach (JToken property in smMaterialPropertiesArray)
            {
                GameObject currentFace = null;
                JObject propertyObj = (JObject)property;
                string propertyName = (string)propertyObj["faceSide"];


                foreach (var face in cubeFacePlanes.sideFaces)
                {
                    if (face.name == propertyName)
                    {
                        currentFace = face;
                    }
                }

                propertyName = propertyName.Replace(" Face", "");
                Material newSunMat = currentFace.GetComponent<Renderer>().material;
                newSunMat.name = (string)propertyObj["sunmicaMatName"];
                JToken jSunToken = propertyObj["sunmicaMatColor"];
                foreach (Material _material in laminateMaterials)
                {
                    if (_material.name == newSunMat.name && _material.mainTexture)
                    {
                        newSunMat.mainTexture = _material.mainTexture;
                    }
                }
                newSunMat.color = new Color(jSunToken["r"].ToObject<float>(), jSunToken["g"].ToObject<float>(), jSunToken["b"].ToObject<float>(), jSunToken["a"].ToObject<float>());
            }

            newCube.transform.localEulerAngles = originalRotation;
        }

        furniture.transform.localPosition = new Vector3(0, 0, 0);
        furniture.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}