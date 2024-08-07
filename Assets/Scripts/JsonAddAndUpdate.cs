//using Newtonsoft.Json.Linq;
//using System;
//using System.IO;
//using System.Linq;
//using TMPro;
//using UnityEngine;

//public class JsonAddAndUpdate : MonoBehaviour
//{
//    private string jsonFile;
//    private void Start()
//    {
//        jsonFile = Application.dataPath + "/StreamingAssets/User.json";
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            //AddCompany();
//            AddComponent();
//        }
//        if (Input.GetKeyDown(KeyCode.U))
//        {
//            UpdateCompany();
//        }
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            DeleteCompany();
//        }
//        if (Input.GetKeyDown(KeyCode.G))
//        {
//            GetUserDetails();
//        }
//    }

//    private void AddCompany()
//    {
//        var companyId = 111;
//        var companyName = "Interio";
//        var newCompanyMember = "{ 'companyid': " + companyId + ", 'companyname': '" + companyName + "'}";
//        try
//        {
//            var json = File.ReadAllText(jsonFile);
//            var jsonObj = JObject.Parse(json);
//            var experienceArrary = jsonObj.GetValue("experiences") as JArray;
//            var newCompany = JObject.Parse(newCompanyMember);
//            experienceArrary.Add(newCompany);
//            print("Added");
//            jsonObj["experiences"] = experienceArrary;
//            string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
//            File.WriteAllText(jsonFile, newJsonResult);
//            print("Executed");
//        }
//        catch (Exception ex)
//        {
//           Debug.Log("Add Error : " + ex.Message.ToString());
//        }
//    }

//    public void AddComponent()
//    {
//        var id = 123;
//        var name = "Manju";
//        var street = "2nd street";
//        var city = "RNR";
//        var zipcode = 560083;
//        var companyId = 888;
//        var companyName = "Interio";
//        var phoneNumber = 1515151515;
//        var role = "Developer";
//        var experinces = "[{ 'companyid': " + companyId + ", 'companyname': '" + companyName + "'}]";
//        var address = "{ 'street': '" + street + "', 'city': '" + city + "', 'zipcode': '" + zipcode + "'}";
//        var newdata = "{'id': " + id+ ", 'name': '" + name + "', 'address': " + address + ", 'experinces': " + experinces + ", 'phoneNumber': '" + phoneNumber + "', 'role': '" + role + "'}";

//        try
//        {
//            var json = File.ReadAllText(jsonFile);
//            var jsonObj = JObject.Parse(json);

//            var components = jsonObj.GetValue("Components") as JArray;
//            var freshData = JObject.Parse(newdata);
//            components.Add(freshData);
//            jsonObj["Components"] = components;
//            string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
//            File.WriteAllText(jsonFile, newJsonResult);
//            print("Executed");
//        }
//        catch (Exception ex)
//        {
//            Debug.Log("Add Error : " + ex.Message.ToString());
//        }
//    }

//    private void UpdateCompany()
//    {
//        string json = File.ReadAllText(jsonFile);

//        try
//        {
//            var jObject = JObject.Parse(json);
//            JArray experiencesArrary = (JArray)jObject["experiences"];
//            Console.Write("Enter Company ID to Update Company : ");
//            var companyId = Convert.ToInt32(Console.ReadLine());

//            if (companyId > 0)
//            {
//                Console.Write("Enter new company name : ");
//                var companyName = Convert.ToString(Console.ReadLine());

//                foreach (var company in experiencesArrary.Where(obj => obj["companyid"].Value<int>() == companyId))
//                {
//                    company["companyname"] = !string.IsNullOrEmpty(companyName) ? companyName : "";
//                }

//                jObject["experiences"] = experiencesArrary;
//                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
//                File.WriteAllText(jsonFile, output);
//            }
//            else
//            {
//                Console.Write("Invalid Company ID, Try Again!");
//                UpdateCompany();
//            }
//        }
//        catch (Exception ex)
//        {

//            Console.WriteLine("Update Error : " + ex.Message.ToString());
//        }
//    }

//    private void DeleteCompany()
//    {
//        var json = File.ReadAllText(jsonFile);
//        try
//        {
//            var jObject = JObject.Parse(json);
//            JArray experiencesArrary = (JArray)jObject["experiences"];
//            Console.Write("Enter Company ID to Delete Company : ");
//            var companyId = Convert.ToInt32(Console.ReadLine());

//            if (companyId > 0)
//            {
//                var companyName = string.Empty;
//                var companyToDeleted = experiencesArrary.FirstOrDefault(obj => obj["companyid"].Value<int>() == companyId);

//                experiencesArrary.Remove(companyToDeleted);

//                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
//                File.WriteAllText(jsonFile, output);
//            }
//            else
//            {
//                Console.Write("Invalid Company ID, Try Again!");
//                UpdateCompany();
//            }
//        }
//        catch (Exception)
//        {

//            throw;
//        }
//    }

//    private void GetUserDetails()
//    {
//        var json = File.ReadAllText(jsonFile);
//        try
//        {
//            var jObject = JObject.Parse(json);

//            if (jObject != null)
//            {
//                print("ID :" + jObject["id"].ToString());
//                print("Name :" + jObject["name"].ToString());

//                var address = jObject["address"];
//                print("Street :" + address["street"].ToString());
//                print("City :" + address["city"].ToString());
//                print("Zipcode :" + address["zipcode"]);
//                JArray experiencesArrary = (JArray)jObject["experiences"];
//                if (experiencesArrary != null)
//                {
//                    foreach (var item in experiencesArrary)
//                    {
//                        print("company Id :" + item["companyid"]);
//                        print("company Name :" + item["companyname"].ToString());
//                    }

//                }
//                print("Phone Number :" + jObject["phoneNumber"].ToString());
//                print("Role :" + jObject["role"].ToString());

//            }
//        }
//        catch (Exception)
//        {

//            throw;
//        }
//    }
    
//}
