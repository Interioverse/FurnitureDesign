//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//using UnityEngine.Networking;
//using System.Linq;
//using UnityEngine;
//using TMPro;
//using System.IO;
//using System;

//public class JsonReader : MonoBehaviour
//{

//    public GameObject objectToSpawn;
//    public TextMeshProUGUI[] nameText1;
//    float distance = 10;
//    GameObject cube, secondObj;
//    GameObject gm;
//    string jText;
  
//    ModelList list = new ModelList();

// private void Start()
//    {
//        StartCoroutine(GetJsonData());

//        Invoke("GetInfo", 2);
 
//    }



//    [System.Serializable]
//    public class Model
//    {
//        public string name;
//        public float price;
//        public float rotationX;
//        public float rotationY;
//        public float rotationZ;
//        public float scaleX;
//        public float scaleY;
//        public float scaleZ;
//        public float positionX;
//        public float positionY;
//        public float positionZ;

//    }

//    [System.Serializable]
//    public class ModelList
//    {
//        public List<Model> detail;
//    }


//    public ModelList myModelList = new ModelList();
//    ObjectToSpawn ots;

 
//    public void _DropDown(int index)
//    {
//        switch(index)
//        {
//            case 0:
//                      GetInfo();
//                       break;
//            case 1:
//                      AscendingOrder();
//                       break;
//            case 2:   
//                      DescendingOrder();
//                       break;
//        }
//    }

   
//    IEnumerator GetJsonData()
//    {
//        string url = "https://drive.google.com/uc?export=download&id=1FnjotvtMzhT0lO7O9BlnUpnOJcV-Agen";

//        UnityWebRequest request = UnityWebRequest.Get(url);
//        yield return request.SendWebRequest();
//        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
//        {
//            Debug.Log(request.error);
//        }
//        else
//        {
//            jText = request.downloadHandler.text;
//            myModelList = JsonUtility.FromJson<ModelList>(jText);
          
//        }
//    }
//    public void GetInfo()
//    {
//        GameObject[] gam = GameObject.FindGameObjectsWithTag("Player");
//        if (gam.Length != 0)
//        {
//            for (int i = 0; i < gam.Length; i++) { Destroy(gam[i]); }
//        }

//            StartCoroutine(GetJsonData());
     
//        list.detail = myModelList.detail;

//            SortTheObjects(list.detail);
       
//    }

//    public void DescendingOrder()
//    {
//        GameObject[] gam = GameObject.FindGameObjectsWithTag("Player");
//        if (gam.Length != 0)
//        {
//            for (int i = 0; i < gam.Length; i++) { Destroy(gam[i]); }
//        }

//       // StartCoroutine(GetJsonData());

//        list.detail = myModelList.detail.OrderByDescending(Model => Model.price).ToList();

//        SortTheObjects(list.detail);
//    }

 
//    public void AscendingOrder()
//    {

//        GameObject[] gam = GameObject.FindGameObjectsWithTag("Player");
//        if (gam.Length != 0)
//        {
//            for (int i = 0; i < gam.Length; i++) { Destroy(gam[i]); }
//        }

//        //StartCoroutine(GetJsonData());

//        list.detail = myModelList.detail.OrderBy(Model => Model.price).ToList();
//        SortTheObjects(list.detail);
      
//    }

//    private void SortTheObjects(List<Model> detail)
//    {
//        for (int i = 0; i < detail.Count; i++)
//        {
//            float posx = distance * i;
//            gm = Instantiate(objectToSpawn, new Vector3(distance * i, 0, 0), Quaternion.identity) as GameObject;

//            ots = gm.GetComponent<ObjectToSpawn>();
//            cube = ots.cube;
//            secondObj = ots.sphere;


//            cube.transform.Rotate(0, detail[i].rotationY, 0);
//            cube.transform.localScale = new Vector3(detail[i].scaleX, detail[i].scaleY, detail[i].scaleZ);
            
//            secondObj.transform.position = new Vector3(detail[i].positionX+posx, detail[i].positionY, detail[i].positionY);
            
//            Debug.Log(secondObj.transform.position);
//            ots._name.text = "Name : " + detail[i].name;
//            ots.price.text = "Price : " + detail[i].price.ToString();

//        }
//    }
//}
    




