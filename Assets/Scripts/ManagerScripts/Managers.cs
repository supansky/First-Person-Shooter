using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WeatherManager))]
[RequireComponent (typeof (ImagesManager))]
[RequireComponent(typeof(AudioManager))]


public class Managers : MonoBehaviour
{
    public static WeatherManager Weather { get; private set; }
    public static ImagesManager Images { get; private set; }
    public static AudioManager Audio { get; private set; }

    private List<IGameManager> startSequence;

    private void Awake()
    {
        Weather = GetComponent<WeatherManager>();
        Images = GetComponent<ImagesManager>();
        Audio = GetComponent<AudioManager>();
        startSequence = new List<IGameManager>();
        startSequence.Add(Weather);
        startSequence.Add(Images);
        startSequence.Add (Audio);

        StartCoroutine(StartupManagers());
    }
    private IEnumerator StartupManagers()
    {
        NetworkService network = new NetworkService();

        foreach(IGameManager manager in startSequence)
            manager.Startup(network);
        
        yield return null;

        int total = startSequence.Count;
        int started = 0;

        while (started < total)
        {
            int lastLoop = started;
            started = 0;

            foreach(IGameManager manager in startSequence)
            {
                if (manager.Status == ManagerStatus.Started)
                    started++;
            }
            if (started > lastLoop)
                Debug.Log($"Progress: {started}/{total}");
            yield return null;
        }

        Debug.Log("All managers started up");

    }
}
