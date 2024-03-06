using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBasicsDemo : MonoBehaviour
{
    public Renderer bodyRendrerer;
    public GameObject visionSphere;

    // Start is called before the first frame update
    void Start()
    {
        visionSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Vector3 smallerScale = new Vector3(0.3f, 0.3f, 0.3f);
        visionSphere.transform.localScale = smallerScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayStartPosition = transform.position;
        rayStartPosition.y += 1.75f; // transform.position + Vector3.up * 1.75f;
        Debug.DrawRay(rayStartPosition, transform.forward * 6);

        transform.Rotate(0, 20 * Time.deltaTime, 0, Space.World);

        Ray ray = new Ray(rayStartPosition, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 6))
        {
            // 'hit' has this useful info:
            //      - collider (because we get access to tghe gameObject
            //      - point (the position the raycast hit
            //      - the "layer" that the gameObject we hit is on
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                bodyRendrerer.material.color = Color.red;

                visionSphere.SetActive(true);
                visionSphere.transform.position = hit.point;
            }
        }
        else
        {
            visionSphere.SetActive(false);
        }
    }
}
