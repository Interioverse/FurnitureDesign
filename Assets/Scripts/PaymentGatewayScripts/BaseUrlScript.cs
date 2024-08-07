using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUrlScript 
{
    public static string baseUrl = "https://backend.interioverse.io/api/common";
    public static string paymentgateway = baseUrl + "/createPaymentLink";
    public static string getwllet = baseUrl + "/get-wallet/";
}
