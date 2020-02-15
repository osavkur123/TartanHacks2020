using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sanicball
{
    [RequireComponent(typeof(Camera))]
    public class CameraSplitter : MonoBehaviour
    {
        private Camera cam;
        private AudioListener listener;

        public int SplitscreenIndex { get; set; }
        public Camera[] cameras;
        private void Start()
        {
            int count = Camera.allCamerasCount;
            listener = GetComponent<AudioListener>();
            int index = 0;
            cameras = Camera.allCameras;
            foreach (Camera cam in Camera.allCameras) {
                if (count == 2)
                {
                    if (index == 0)
                    {
                        cam.rect = new Rect(0f, 0f, 1f, 0.5f);
                    }
                    else
                    {
                        cam.rect = new Rect(0f, 0.5f, 1f, 0.5f);
                    }
                }
                else
                {
                    cam.rect = new Rect(0f, 0f, 1f, 1f);
                }
                /*switch (count)
                {
                    case 2:
                        switch (index)
                        {
                            case 0:
                                cam.rect = new Rect(0f, 0f, 1f, 0.5f);
                                break;

                            case 1:
                                cam.rect = new Rect(0.5f, 0f, 1f, 0.5f);
                                break;
                        }
                        break;

                    case 3:
                        switch (index)
                        {
                            case 0:
                                cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                                break;

                            case 1:
                                cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                                break;

                            case 2:
                                cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                                break;
                        }
                        break;

                    case 4:
                        switch (index)
                        {
                            case 0:
                                cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                                break;

                            case 1:
                                cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                                break;

                            case 2:
                                cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                                break;

                            case 3:
                                cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                                break;
                        }
                        break;

                    default:
                        cam.rect = new Rect(0, 0, 1, 1);
                        break;
                }*/
                index++;
            }

            

            if (listener)
            {
                listener.enabled = index == 0;
            }
        }
    }
}