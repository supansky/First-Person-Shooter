using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class WeatherManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    // Add cloud value here
    private NetworkService network;

    public float cloudValue { get; private set; }

    public void Startup(NetworkService service)
    {
        Debug.Log("Weather manager starting...");

        network = service;
        StartCoroutine(network.GetWeatherJSON(OnJSONDataLoaded));

        Status = ManagerStatus.Initializating;
    }
    
    public void OnJSONDataLoaded(string data)
    {
        JObject root = JObject.Parse(data);

        JToken clouds = root["clouds"];
        cloudValue = (float)clouds["all"] / 100f;
        Debug.Log($"Value: {cloudValue}");

        Messenger.Broadcast(GameEvent.WEATHER_UPDATED);

        Status = ManagerStatus.Started;
    }
}
