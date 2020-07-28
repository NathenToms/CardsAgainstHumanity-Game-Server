using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the aspect ratio for my camera witch is required
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraAspectController : MonoBehaviour
{
    /// <summary> Target aspect ratio for the game </summary>
    public readonly float targetAspectRatio = 16f / 9f;

	//public read only float targetAspectRatio = 4f / 3f;

	/// <summary>
	///  Starts a coroutine for changing aspect ratio all the time
	/// </summary>
	void Start()
    {
        StartCoroutine(UpdateAspectRatio());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Coroutine updates the aspect ratio at the end of frame
    /// </summary>
    /// <returns>yield end of frame</returns>
    IEnumerator UpdateAspectRatio()
    {
        Camera camera = GetComponent<Camera>();

        while (true)
        {
            float currentAspectRatio = (float)Screen.width / (float)Screen.height;
            float scaleHeight = currentAspectRatio / targetAspectRatio;

            Rect rect = camera.rect;

            // Letter box
            if (scaleHeight < 1f)
            {
                rect.width = 1f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1f - scaleHeight) / 2f;
                camera.rect = rect;
            }
            // Pillar box
            else
            {
                float scaleWidth = 1f / scaleHeight;

                rect.width = scaleWidth;
                rect.height = 1f;
                rect.x = (1f - scaleWidth) / 2f;
                rect.y = 0;
                camera.rect = rect;
			}

            yield return new WaitForEndOfFrame();
        }
    }
}
