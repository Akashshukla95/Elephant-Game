
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using OpenCVForUnity.CoreModule;
// using OpenCVForUnity.DnnModule;
// using OpenCVForUnity.ImgprocModule;
// using OpenCVForUnity.UnityUtils;
// using OpenCVForUnity.UnityUtils.Helper;
// using System.Threading;
// using System.Text;

// public class ArData
// {
//     public StringBuilder word;
//     public bool status;

//     public ArData()
//     {
//         word = new StringBuilder();
//         status = true;
//     }
// }

// public class ObjectDetector : MonoBehaviour
// {
// #pragma warning disable 649
//     [SerializeField] private CameraVision cameraVision;
//     [SerializeField] private RawImage opencvTexture;
// #pragma warning restore 649

//     public string model;
//     public string config;
//     public string classes;
//     public List<string> classesList;
//     public float confThreshold = 0.5f;
//     public float nmsThreshold = 0.4f;
//     public float scale = 1.0f;
//     public Scalar mean = new Scalar(0, 0, 0, 0);
//     public bool swapRB = false;
//     public int inpWidth = 320;
//     public int inpHeight = 320;

//     protected Texture2D texture;
//     //public RawImage detectedMatTexture;
//     protected Mat bgrMat;
//     protected Net net;

//     protected List<string> classNames;
//     protected List<string> outBlobNames;
//     protected List<string> outBlobTypes;

//     protected string classes_filepath;
//     protected string config_filepath;
//     protected string model_filepath;

//     private Thread openCVThread;
//     public bool isGameRunning = false;
//     private Mat currentMat;
//     private ArData arData;
//     private bool isMat = false;

//     private void OnDisable()
//     {
//         KillProcess();
//     }

//     private void Start()
//     {
//         if (!string.IsNullOrEmpty(classes)) classes_filepath = Utils.getFilePath("dnn/" + classes);
//         if (!string.IsNullOrEmpty(config)) config_filepath = Utils.getFilePath("dnn/" + config);
//         if (!string.IsNullOrEmpty(model)) model_filepath = Utils.getFilePath("dnn/" + model);
//         Run();
//     }

//     private void Update()
//     {
//         if (isGameRunning)
//         {
//             if (isMat)
//             {
//                 isMat = false;
//                 currentMat = cameraVision.GetMat();
//                 Utils.fastMatToTexture2D(currentMat, texture);
//             }
//             opencvTexture.texture = texture;

//             if (Input.GetKeyDown(KeyCode.A))
//             {
//                 KillProcess();
//             }
//         }
//     }

//     protected virtual void Run()
//     {
//         Utils.setDebugMode(true);
//         if (!string.IsNullOrEmpty(classes))
//         {
//             classNames = readClassNames(classes_filepath);
//             if (classNames == null)
//             {
//                 Debug.LogError(classes_filepath + " is not loaded. Please see \"StreamingAssets/dnn/setup_dnn_module.pdf\". ");
//             }
//         }
//         else if (classesList.Count > 0)
//         {
//             classNames = classesList;
//         }

//         if (string.IsNullOrEmpty(model_filepath))
//         {
//             Debug.LogError(model_filepath + " is not loaded. Please see \"StreamingAssets/dnn/setup_dnn_module.pdf\". ");
//         }
//         else
//         {
//             net = Dnn.readNet(model_filepath, config_filepath);
//             outBlobNames = getOutputsNames(net);
//             outBlobTypes = getOutputsTypes(net);
//         }
//         cameraVision.Initialize();
//         arData = new ArData();
//         Debug.Log("Initialization Complete");
//     }

//     public virtual void OnWebCamTextureToMatHelperInitialized()
//     {
//         Debug.Log("OnWebCamTextureToMatHelperInitialized");
//         Mat webCamTextureMat = cameraVision.GetMat();
//         texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
//         //gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
//         Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);
//         float width = webCamTextureMat.width();
//         float height = webCamTextureMat.height();
//         bgrMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);
//     }

//     public virtual void OnWebCamTextureToMatHelperDisposed()
//     {
//         KillProcess();
//         Debug.Log("OnWebCamTextureToMatHelperDisposed");
//         if (bgrMat != null)
//             bgrMat.Dispose();

//         if (texture != null)
//         {
//             Texture2D.Destroy(texture);
//             texture = null;
//         }
//     }

//     #region  --------------------------------- Running on Seperate Thread --------------------------------------
//     private void ProcessLetters()
//     {
//         while (isGameRunning)
//         {
//             isMat = true;
//             Thread.Sleep(100); // Make Thread sleep for 0.1 second

//             if (net == null)
//             {
//                 Imgproc.putText(currentMat, "model file is not loaded.", new Point(5, currentMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
//                 Imgproc.putText(currentMat, "Please read console message.", new Point(5, currentMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
//             }
//             else
//             {
//                 Imgproc.cvtColor(currentMat, bgrMat, Imgproc.COLOR_RGBA2BGR);
//                 Size inpSize = new Size(inpWidth > 0 ? inpWidth : bgrMat.cols(),
//                                    inpHeight > 0 ? inpHeight : bgrMat.rows());
//                 Mat blob = Dnn.blobFromImage(bgrMat, scale, inpSize, mean, swapRB, false);
//                 net.setInput(blob);
//                 List<Mat> outs = new List<Mat>();
//                 net.forward(outs, outBlobNames);
//                 postprocess(currentMat, outs, net, Dnn.DNN_BACKEND_OPENCV);
//                 for (int i = 0; i < outs.Count; i++)
//                 {
//                     outs[i].Dispose();
//                 }
//                 blob.Dispose();
//             }
//             isMat = true;
//         }
//     }

//     #endregion -------------------------------------------------------------------------------------------

//     void OnDestroy()
//     {
//         cameraVision.Dispose();
//         if (net != null)
//             net.Dispose();

//         Utils.setDebugMode(false);
//     }

//     protected virtual List<string> getOutputsNames(Net net)
//     {
//         List<string> names = new List<string>();
//         MatOfInt outLayers = net.getUnconnectedOutLayers();
//         for (int i = 0; i < outLayers.total(); ++i)
//         {
//             names.Add(net.getLayer(new DictValue((int)outLayers.get(i, 0)[0])).get_name());
//         }
//         outLayers.Dispose();
//         return names;
//     }

//     protected virtual List<string> getOutputsTypes(Net net)
//     {
//         List<string> types = new List<string>();
//         MatOfInt outLayers = net.getUnconnectedOutLayers();
//         for (int i = 0; i < outLayers.total(); ++i)
//         {
//             types.Add(net.getLayer(new DictValue((int)outLayers.get(i, 0)[0])).get_type());
//         }
//         outLayers.Dispose();
//         return types;
//     }

//     protected virtual List<string> readClassNames(string filename)
//     {
//         List<string> classNames = new List<string>();
//         System.IO.StreamReader cReader = null;
//         try
//         {
//             cReader = new System.IO.StreamReader(filename, System.Text.Encoding.Default);
//             while (cReader.Peek() >= 0)
//             {
//                 string name = cReader.ReadLine();
//                 classNames.Add(name);
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError(ex.Message);
//             return null;
//         }
//         finally
//         {
//             if (cReader != null)
//                 cReader.Close();
//         }
//         return classNames;
//     }

//     private void postprocess(Mat frame, List<Mat> outs, Net net, int backend = Dnn.DNN_BACKEND_OPENCV)
//     {
//         MatOfInt outLayers = net.getUnconnectedOutLayers();
//         string outLayerType = outBlobTypes[0];

//         List<int> classIdsList = new List<int>();
//         List<float> confidencesList = new List<float>();
//         List<Rect2d> boxesList = new List<Rect2d>();
//         if (outLayerType == "Region")
//         {
//             for (int i = 0; i < outs.Count; ++i)
//             {
//                 float[] positionData = new float[5];
//                 float[] confidenceData = new float[outs[i].cols() - 5];
//                 for (int p = 0; p < outs[i].rows(); p++)
//                 {
//                     outs[i].get(p, 0, positionData);
//                     outs[i].get(p, 5, confidenceData);

//                     int maxIdx = confidenceData.Select((val, idx) => new { V = val, I = idx }).Aggregate((max, working) => (max.V > working.V) ? max : working).I;
//                     float confidence = confidenceData[maxIdx];
//                     if (confidence > confThreshold)
//                     {
//                         float centerX = positionData[0] * frame.cols();
//                         float centerY = positionData[1] * frame.rows();
//                         float width = positionData[2] * frame.cols();
//                         float height = positionData[3] * frame.rows();
//                         float left = centerX - width / 2;
//                         float top = centerY - height / 2;

//                         classIdsList.Add(maxIdx);
//                         confidencesList.Add((float)confidence);
//                         boxesList.Add(new Rect2d(left, top, width, height));
//                     }
//                 }
//             }
//         }
//         else
//         {
//             Debug.Log("Unknown output layer type: " + outLayerType);
//         }

//         if (outLayers.total() > 1 || (outLayerType == "Region" && backend != Dnn.DNN_BACKEND_OPENCV))
//         {
//             Dictionary<int, List<int>> class2indices = new Dictionary<int, List<int>>();
//             for (int i = 0; i < classIdsList.Count; i++)
//             {
//                 if (confidencesList[i] >= confThreshold)
//                 {
//                     if (!class2indices.ContainsKey(classIdsList[i]))
//                         class2indices.Add(classIdsList[i], new List<int>());

//                     class2indices[classIdsList[i]].Add(i);
//                 }
//             }

//             List<Rect2d> nmsBoxesList = new List<Rect2d>();
//             List<float> nmsConfidencesList = new List<float>();
//             List<int> nmsClassIdsList = new List<int>();
//             foreach (int key in class2indices.Keys)
//             {
//                 List<Rect2d> localBoxesList = new List<Rect2d>();
//                 List<float> localConfidencesList = new List<float>();
//                 List<int> classIndicesList = class2indices[key];
//                 for (int i = 0; i < classIndicesList.Count; i++)
//                 {
//                     localBoxesList.Add(boxesList[classIndicesList[i]]);
//                     localConfidencesList.Add(confidencesList[classIndicesList[i]]);
//                 }
//                 using (MatOfRect2d localBoxes = new MatOfRect2d(localBoxesList.ToArray()))
//                 using (MatOfFloat localConfidences = new MatOfFloat(localConfidencesList.ToArray()))
//                 using (MatOfInt nmsIndices = new MatOfInt())
//                 {
//                     Dnn.NMSBoxes(localBoxes, localConfidences, confThreshold, nmsThreshold, nmsIndices);
//                     for (int i = 0; i < nmsIndices.total(); i++)
//                     {
//                         int idx = (int)nmsIndices.get(i, 0)[0];
//                         nmsBoxesList.Add(localBoxesList[idx]);
//                         nmsConfidencesList.Add(localConfidencesList[idx]);
//                         nmsClassIdsList.Add(key);
//                     }
//                 }
//             }

//             boxesList = nmsBoxesList;
//             classIdsList = nmsClassIdsList;
//             confidencesList = nmsConfidencesList;
//         }
//         arData.word.Clear();
//         for (int idx = 0; idx < boxesList.Count; ++idx)
//         {
//             Rect2d box = boxesList[idx];
//             drawPred(classIdsList[idx], confidencesList[idx], box.x, box.y,
//                 box.x + box.width, box.y + box.height, frame);
//         }
//     }

//     private void drawPred(int classId, float conf, double left, double top, double right, double bottom, Mat frame)
//     {
//         //Imgproc.rectangle(frame, new Point(left, top), new Point(right, bottom), new Scalar(0, 255, 0, 255), 2);

//         string label = conf.ToString();
//         if (classNames != null && classNames.Count != 0)
//         {
//             if (classId < (int)classNames.Count)
//             {
//                 //label = classNames[classId] + ": " + label;
//                 label = classNames[classId];
//             }
//         }
//         Debug.Log("Word is:" + label);
//         arData.word.Append(label);
//         // int[] baseLine = new int[1];
//         // Size labelSize = Imgproc.getTextSize(label, Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, 1, baseLine);

//         // top = Mathf.Max((float)top, (float)labelSize.height);
//         // Imgproc.rectangle(frame, new Point(left, top - labelSize.height),
//         //     new Point(left + labelSize.width, top + baseLine[0]), Scalar.all(255), Core.FILLED);
//         // Imgproc.putText(frame, label, new Point(left, top), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(0, 0, 0, 255));
//     }

//     public void InitProcessing()
//     {
//         Debug.Log("Init Thread");
//         isGameRunning = true;
//         openCVThread = new Thread(new ThreadStart(ProcessLetters));
//         openCVThread.Start();
//     }

//     public void KillProcess()
//     {
//         isGameRunning = false;
//         if (openCVThread != null) openCVThread.Abort();
//     }

//     public void Disconnect()
//     {
//         KillProcess();
//         arData = new ArData();
//     }

//     public ArData GetArData()
//     {
//         return arData;
//     }
// }