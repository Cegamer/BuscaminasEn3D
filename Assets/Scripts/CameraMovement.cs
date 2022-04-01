using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Transform cameraParent;
    Vector3 cameraDefault,cameraPosition, objectivePosition, positiveLimits, negativeLimits;
    public float zoomSpeed,movementSpeed;
    bool weelStatus = false;
    // Start is called before the first frame update
    void Start()
    {
        cameraParent = mapGenerator.cameraParent.transform;
        restoreCamera();
        cameraParent.transform.position = cameraDefault;
    }

    void Update()
    {
        if (mapGenerator.playing)
        {
            calculateCameraLimits();
            cameraPosition = cameraParent.position;
            if (mapGenerator.cameraObjective != null)
            {
                cameraPosition = cameraParent.position;
                objectivePosition = mapGenerator.cameraObjective.transform.position;

                if (Input.mouseScrollDelta.y > 0 && cameraPosition.y > negativeLimits.y)
                {
                    float step = zoomSpeed * Time.deltaTime;
                    cameraPosition = Vector3.MoveTowards(cameraPosition, objectivePosition, step);
                }
                else if (Input.mouseScrollDelta.y < 0 && cameraPosition.y < positiveLimits.y)
                {
                    float step = -zoomSpeed * Time.deltaTime;
                    cameraPosition = Vector3.MoveTowards(cameraPosition, objectivePosition, step);
                }

            }

            if (weelStatus)
            {
                if (Input.GetAxis("Mouse X") < 0 && cameraPosition.x > negativeLimits.x)
                {//Mouse Left
                    float newX = cameraPosition.x - movementSpeed * Time.deltaTime;
                    cameraPosition = new Vector3(newX, cameraPosition.y, cameraPosition.z);
                }
                else if (Input.GetAxis("Mouse X") > 0 && cameraPosition.x < positiveLimits.x)
                {//Mouse Right
                    float newX = cameraPosition.x + movementSpeed * Time.deltaTime;
                    cameraPosition = new Vector3(newX, cameraPosition.y, cameraPosition.z);
                }

                if (Input.GetAxis("Mouse Y") > 0 && cameraPosition.z < positiveLimits.z)
                {//Mouse Up
                    float newZ = cameraPosition.z + movementSpeed * Time.deltaTime;
                    cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y, newZ);
                }
                else if (Input.GetAxis("Mouse Y") < 0 && cameraPosition.z > negativeLimits.z)
                {//Mouse Down
                    float newZ = cameraPosition.z - movementSpeed * Time.deltaTime;
                    cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y, newZ);
                }
            }
            cameraRePositioning();
            checkMouseWeelButton();

            cameraParent.position = cameraPosition;
        }
    }

    void checkMouseWeelButton()
    {
        if (Input.GetMouseButtonDown(2))
            weelStatus = true;
        else if (Input.GetMouseButtonUp(2))
            weelStatus = false;
    }
    void cameraRePositioning()
    {
        if (cameraPosition.x > positiveLimits.x)
            cameraPosition.x = positiveLimits.x;

        if (cameraPosition.x < negativeLimits.x)
            cameraPosition.x = negativeLimits.x;

        if (cameraPosition.y > positiveLimits.y)
            cameraPosition.y = positiveLimits.y;

        if (cameraPosition.y < negativeLimits.y)
            cameraPosition.y = negativeLimits.y;

        if(cameraPosition.z < negativeLimits.z)
            cameraPosition.z = negativeLimits.z;

        if (cameraPosition.z > positiveLimits.z)
            cameraPosition.z = positiveLimits.z;
    }
    void calculateCameraLimits()
    {
        float positiveY = mapGenerator.gridSize;
        float positiveX = (cameraPosition.y * -0.5f) + (mapGenerator.gridSize - 0.5f);
        float negativeY = 3;
        float negativeX = (cameraPosition.y * 0.5f) - 0.5f;
        float positiveZ = mapGenerator.gridSize - 5;

        if (cameraPosition.y > mapGenerator.gridSize - 10)
            positiveZ = mapGenerator.gridSize - 5;

        positiveLimits = new Vector3(positiveX,positiveY,positiveZ);
        negativeLimits = new Vector3(negativeX, negativeY, -0.5f);

    }
    public void restoreCamera()
    {
        float gridposition = (mapGenerator.gridSize / 2f) - 0.5f;
        cameraDefault = new Vector3(gridposition, mapGenerator.gridSize, -0.5f);

        cameraParent.position = cameraDefault;
    }
}
