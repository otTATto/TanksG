using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TankWarp : MonoBehaviour
{
    private Renderer[]  _target;  // �_�ł�����I�u�W�F�N�g��Renderer
    public float cycle = 0.5f;  // �_�ł̎���
    public float blinkingTime = 4f;
    private float timer = 0f;    // �����̎��Ԃ�ǐ�
    private bool isBlinking = false; // �_�Œ����ǂ����������t���O
    public bool canWarp = true;
    
     void Start()
    {
        // �q�I�u�W�F�N�g��Renderer���擾
        //isBlinking = true;
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

