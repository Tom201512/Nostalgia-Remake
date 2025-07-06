using Unity.VisualScripting;
using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;
using static ReelSpinGame_Reels.ReelData;

namespace ReelSpinGame_AutoPlay
{
    public class AutoPlayFunction
    {
        // �I�[�g�v���C�@�\

        // const
        // �I�[�g���x
        public enum AutoPlaySpeed { Normal, Fast, Quick}
        // ���݂̃I�[�g���[�h
        public enum AutoPlaySequence { AutoInsert, AutoStopReel}

        // var
        // �I�[�g�v���C����
        public bool HasAuto { get; private set; }
        // �I�[�g���x
        public AutoPlaySpeed AutoSpeed { get; private set; }
        // �I�[�g���̉�����
        public ReelID[] AutoStopOrders { get; private set; }
        // �I�[�g���̒�~�ʒu
        public int[] AutoStopPos { get; private set; }


        // �R���X�g���N�^
        public AutoPlayFunction()
        {
            Debug.Log("Base Constructor");
            HasAuto = false;
            // ��~���Ԃ̔z��쐬(�f�t�H���g�͏�����)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoSpeed = AutoPlaySpeed.Normal;

            // ��~�ʒu�̔z��쐬(�e�X�g�p��0�Ŏ~�߂�悤��)
            AutoStopPos = new int[] { 0, 0, 0 };

            Debug.Log("Speed:" + AutoSpeed);
            foreach(ReelID order in AutoStopOrders)
            {
                Debug.Log("Order:" + order.ToString());
            }
        }

        // �ݒ肩��ǂݍ��ޏꍇ
        public AutoPlayFunction(AutoPlaySpeed speed, ReelID[] order):this()
        {
            AutoSpeed = speed;
            AutoStopOrders = order;

            Debug.Log("Speed:" + AutoSpeed);
            foreach (ReelID newOrder in AutoStopOrders)
            {
                Debug.Log("Order:" + newOrder.ToString());
            }
        }

        // func

        // �I�[�g�@�\�̐؂�ւ�
        public void ChangeAutoMode()
        {
            HasAuto = !HasAuto;
            Debug.Log("AutoMode:" + HasAuto);
        }

        // �I�[�g�������̕ύX
        public void ChangeAutoStopOrder(ReelID first, ReelID second, ReelID third)
        {
            // �������������Ȃ����`�F�b�N
            if(first != second && second != third)
            {
                AutoStopOrders[0] = first;
                AutoStopOrders[1] = second;
                AutoStopOrders[2] = third;
            }
            else
            {
                Debug.Log("Failed to change because there are duplicated orders");
            }
        }

        // �I�[�g�����ʒu�̕ύX
        public void ChangeAutoStopPos(int left, int middle, int right)
        {
            // ��~�ʒu���I�[�o�[�t���[���Ă��Ȃ����`�F�b�N(0~20)
            if ((left < MaxReelArray && middle < MaxReelArray && right < MaxReelArray) &&
                (left >= 0 && middle >= 0 && right >= 0))
            {
                AutoStopPos[(int)ReelID.ReelLeft] = left;
                AutoStopPos[(int)ReelID.ReelMiddle] = middle;
                AutoStopPos[(int)ReelID.ReelRight] = right;
            }
            else
            {
                Debug.Log("Failed to change because there are duplicated orders");
            }
        }
    }
}
