using System.Collections;
using UnityEngine;

public class Warphole : MonoBehaviour
{
    public LayerMask m_TankMask;
    public float warpDelay = 2f;    // ���[�v�܂ł̑ҋ@����
    public Transform transHole;     // ���[�v��̈ʒu
    private bool isWarping = false; // ���[�v�����ǂ����̃t���O

    // OnTriggerEnter�Ƀ^�C�}�[�����ƃ��[�v�����𓝍�
    private void OnTriggerEnter(Collider other)
    {
        // �Փ˂����I�u�W�F�N�g��m_TankMask�Ɋ܂܂�Ă��邩�`�F�b�N
        if (((1 << other.gameObject.layer) & m_TankMask) != 0 && !isWarping)
        {
            TankWarp tankWarp = other.GetComponent<TankWarp>();

            if (tankWarp.canWarp)
            {
                // ��Ԃ̓_�ł��J�n���A���[�v����
                tankWarp.StartBlinking();

                // ���[�v���ł��邱�Ƃ������t���O��ݒ�
                isWarping = true;

                // �R���[�`�����J�n���āA�^�C�}�[��Ƀ��[�v�����s
                StartCoroutine(WarpAfterDelay(other.transform));
            }
        }
    }

    // ���[�v����܂ł̑ҋ@�������s���R���[�`��
    private IEnumerator WarpAfterDelay(Transform target)
    {
        // �w�肵�����Ԃ����ҋ@
        yield return new WaitForSeconds(warpDelay);

        // ���[�v�������J�n
        StartWarp(target);

        // ���[�v�����������̂Ńt���O�����Z�b�g
        isWarping = false;
    }

    // ���[�v�������s��
    private void StartWarp(Transform target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (transHole != null)
        {
            // ���[�v��̈ʒu�Ƀ^�[�Q�b�g���ړ�
            targetRigidbody.MovePosition(transHole.position);
        }
    }
}

