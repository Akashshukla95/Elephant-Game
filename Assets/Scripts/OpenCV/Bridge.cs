// using System;
// using UnityEngine;
// using System.Collections;

// public class Bridge : MonoBehaviour
// {
// #pragma warning disable 649
//     [SerializeField] private ObjectDetector objectDetector;
//     [SerializeField] private bool autoInit;
// #pragma warning restore 649

//     private bool isBridgeCreated = false;
//     private float linkDelayTime = 2;

//     private void Start()
//     {
//         if (autoInit) Init();
//     }


//     private IEnumerator CreateBrigdeLink()
//     {
//         yield return new WaitForSeconds(linkDelayTime);
//         objectDetector.InitProcessing();
//         yield return new WaitForEndOfFrame();
//         isBridgeCreated = true;
//     }

//     private void DisconnectBridgeLink()
//     {
//         isBridgeCreated = false;
//         objectDetector.Disconnect();
//     }

//     public void Init()
//     {
//         StartCoroutine(CreateBrigdeLink());
//     }

//     public void Close()
//     {
//         DisconnectBridgeLink();
//     }

//     public Tuple<string, bool> GetWordData()
//     {
//         string word = objectDetector.GetArData().word.ToString();
//         bool status = objectDetector.GetArData().status;
//         return new Tuple<string, bool>(word, status);
//     }

// }
