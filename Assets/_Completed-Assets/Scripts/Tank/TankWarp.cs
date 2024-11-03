using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TankWarp : MonoBehaviour
{
    private Renderer[]  _target;  // 
    public float cycle = 0.5f;  //
    public float blinkingTime = 4f;
    private float timer = 0f;    //
    private bool isBlinking = false; //
    public bool canWarp = true;
    
     void Start()
    {
        _target = GetComponentsInChildren<Renderer>();
    }
        private void Update()
    {
        if (isBlinking)
        {

            timer += Time.deltaTime;

            var repeatValue = Mathf.Repeat((float)timer, cycle);
            foreach (Renderer renderer in _target) renderer.enabled = repeatValue >= cycle * 0.5f;

            if (timer >= blinkingTime) StopBlinking();
        }
    }

    public void StartBlinking()
    {
        isBlinking = true;
        timer = 0f;
        canWarp = false;     
    }


    public void StopBlinking()
    {
        isBlinking = false;
        foreach (Renderer renderer in _target) renderer.enabled = true;
        canWarp = true;
    }

}

