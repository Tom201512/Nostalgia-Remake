using UnityEngine;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_AutoPlay
{
    public class AutoPlayFunction
    {
        // �I�[�g�v���C�@�\

        // const
        // �I�[�g���x
        public enum AutoPlaySpeed { Normal, Fast, Quick}

        // var
        // �I�[�g�v���C����
        public bool HasAuto { get; private set; }
        public AutoPlaySpeed AutoSpeed { get; private set; }
        public ReelID[] AutoStopOrders { get; private set; }
        // �I�[�g�̑��x


        // �R���X�g���N�^
        public AutoPlayFunction()
        {
            Debug.Log("Base Constructor");
            HasAuto = false;
            // ��~�ʒu�̔z��쐬(�f�t�H���g�͏�����)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoSpeed = AutoPlaySpeed.Normal;

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
    }
}
