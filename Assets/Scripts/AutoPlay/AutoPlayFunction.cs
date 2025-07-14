using ReelSpinGame_AutoPlay.AI;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusBehaviour;
using static ReelSpinGame_Lots.FlagBehaviour;
using static ReelSpinGame_Reels.ReelData;
using static ReelSpinGame_Reels.ReelManagerBehaviour;

namespace ReelSpinGame_AutoPlay
{
    public class AutoPlayFunction
    {
        // �I�[�g�v���C�@�\

        // const
        // �I�[�g�������̎��ʗp
        public enum AutoStopOrder { First, Second, Third}
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
        // �I�[�g�̒�~�ʒu�����߂���
        public bool HasStopPosDecided { get; private set; }

        // �I�[�g���̒�~�ʒu�I��AI
        private AutoPlayAI autoAI;


        // �R���X�g���N�^
        public AutoPlayFunction()
        {
            autoAI = new AutoPlayAI();
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
            HasStopPosDecided = false;
        }
        
        // �I�[�g��~�ʒu�����Z�b�g
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)AutoStopOrder.First] = 0;
            AutoStopOrders[(int)AutoStopOrder.Second] = 0;
            AutoStopOrders[(int)AutoStopOrder.Third] = 0;

            Debug.Log("AutoPos Reset");
        }

        // �I�[�g���������t���O�A�������瓾��
        public void GetAutoStopPos(FlagId flag, BonusType holdingBonus, int bigChanceGames, int remainingJac)
        {
            Debug.Log("GetPos");

            AutoStopPos = autoAI.GetStopPos(flag, AutoStopOrders[(int)AutoStopOrder.First], holdingBonus, bigChanceGames, remainingJac);
            Debug.Log("Pos created");

            HasStopPosDecided = true;
            Debug.Log("AutoPos Decided");
        }

        // �I�[�g�������̕ύX
        private void ChangeAutoStopOrder(ReelID first, ReelID second, ReelID third)
        {
            // �������������Ȃ����`�F�b�N
            if(first != second && second != third)
            {
                AutoStopOrders[(int)AutoStopOrder.First] = first;
                AutoStopOrders[(int)AutoStopOrder.Second] = second;
                AutoStopOrders[(int)AutoStopOrder.Third] = third;
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
