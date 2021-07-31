using UnityEngine;

public class RotateCamera : MonoBehaviour
{
   public float speed = 5f;
   public Transform _rotator;

   private void start()
   {
       _rotator = GetComponent<Transform>();

   }

   private void Update()
   {
       _rotator.Rotate(0f, speed * Time.deltaTime, 0f);
   }
}
