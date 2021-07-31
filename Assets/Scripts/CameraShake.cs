using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    private float shakeDur = 1f, 
        shakeAmount = 0.04f, 
        decreaseFactor = 1.5f;
    private Vector3 originPos; //Изначальная позиция

    
    private void start()
    {
        camTransform = GetComponent<Transform>();
        originPos = camTransform.localPosition;
    }


    private void Update()
    {
        if(shakeDur > 0){
            Debug.Log(originPos);
            Debug.Log(Random.insideUnitSphere);
            Debug.Log(shakeAmount);
            if((originPos == null) || (Random.insideUnitSphere == null) || (shakeAmount == null) || (camTransform.localPosition == null)){
                Debug.Log("Иф");
            }else{
                Debug.Log("не Иф");
            }

            camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
            shakeDur -= Time.deltaTime * decreaseFactor;
        }else{
            shakeDur = 0;
            camTransform.localPosition = originPos;
        }
    }
}
