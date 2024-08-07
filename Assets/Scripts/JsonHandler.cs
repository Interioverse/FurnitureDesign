//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class JsonHandler : MonoBehaviour
//{
//    private void Start()
//    {
//        TextAsset targetFile = Resources.Load<TextAsset>("Files/Products");
//        string jsonString = targetFile.text;
//        Mystore mystore = JsonUtility.FromJson<Mystore>(jsonString);
//    }
//}

//[System.Serializable]
//public class FurnitureCollection
//{
//    public string id;
//    public string productcode;
//    public string price;
//    public string description;
//}

//[System.Serializable]
//public class Product
//{
//    public string id;
//    public string title;
//    public string defaultName;
//    public string priceRange;
//    public List<FurnitureCollection> furnitureCollections;
//}

//[System.Serializable]
//public class Mystore
//{
//    public List<Product> Products;
//}