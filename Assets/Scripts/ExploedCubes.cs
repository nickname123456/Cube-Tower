using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploedCubes : MonoBehaviour
{
    public GameObject restartButton, explosion;
    public float explosionForce = 70f;
    public float explosionRadius = 5f;
    private bool _collisionSet;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cube" && !_collisionSet)
        {
            for (int i = collision.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, Vector3.up, explosionRadius);
                child.SetParent(null);
            }

            restartButton.SetActive(true);
            // Camera.main.transform.localPosition -= new Vector3(0, 0, 3f);

            Camera.main.gameObject.AddComponent<CameraShake>();

            Instantiate(explosion, new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z), Quaternion.identity);

            // Destroy(collision.gameObject);
            _collisionSet = true;
        }
    }
}
