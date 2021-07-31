using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    public float shakeDur = 1f, //Время, за которое камера будет трястись
                shakeAmount = 0.04f, //Насколько сильно камера будет трястись
                decreaseFactor = 1.5f; //Сколько камера будет трястись
    private Vector3 originPos;

    private void Start()
    {
        camTransform = GetComponent<Transform>();
        originPos = camTransform.localPosition;
    }


    private void Update()
    {
        if(shakeDur > 0){
            camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
            shakeDur -= Time.deltaTime * decreaseFactor;
        }else{
            shakeDur = 0;
            camTransform.localPosition = originPos;
        }
    }

}
