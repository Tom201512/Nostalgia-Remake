using ReelSpinGame_AutoPlay.AI;
using System;
using UnityEngine;
using static ReelSpinGame_Bonus.BonusSystemData;
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
        // �I�[�g�������w��p(��:L, ��:M, �E:R)
        public enum AutoStopOrderOptions { LMR, LRM, MLR, MRL, RLM, RML}
        // �I�[�g���x(�ʏ�A�����A������)
        public enum AutoPlaySpeed { Normal, Fast, Quick}
        // ���݂̃I�[�g���[�h
        public enum AutoPlaySequence { AutoInsert, AutoStopReel}

        // var
        // �I�[�g�v���C����
        public bool HasAuto { get; private set; }
        // �I�[�g���x
        public int AutoSpeedID { get; private set; }
        // �����I�[�g�����҂���
        public bool HasWaitingCancel { get; private set; }
        // �I�[�g���̉�����
        public ReelID[] AutoStopOrders { get; private set; }
        // �I�[�g�������̃I�v�V�������l
        public int AutoOrderID { get; private set; }
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
            HasAuto = false;
            // ��~���Ԃ̔z��쐬(�f�t�H���g�͏�����)
            AutoStopOrders = new ReelID[] { ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight };
            AutoOrderID = (int)AutoStopOrderOptions.LMR;
            AutoSpeedID = (int)AutoPlaySpeed.Normal;

            // ��~�ʒu�̔z��쐬(�e�X�g�p��0�Ŏ~�߂�悤��)
            AutoStopPos = new int[] { 0, 0, 0 };
            HasWaitingCancel = false;
        }

        // func

        // �I�[�g����Ԃ�
        public string GetOrderName() => Enum.ToObject(typeof(AutoStopOrderOptions), AutoOrderID).ToString();

        // �X�s�[�h����Ԃ�
        public string GetSpeedName() => Enum.ToObject(typeof(AutoPlaySpeed), AutoSpeedID).ToString();

        // �I�[�g�d�l�ԍ��̕ύX(�f�o�b�O�p)
        public void ChangeAutoOrder()
        {
            if (!HasAuto)
            {
                if (AutoOrderID + 1 > (int)AutoStopOrderOptions.RML)
                {
                    AutoOrderID = (int)AutoStopOrderOptions.LMR;
                }
                else
                {
                    AutoOrderID += 1;
                }
            }
        }

        // �I�[�g�X�s�[�h�ԍ��ύX
        public void ChangeAutoSpeed()
        {
            if(!HasAuto)
            {
                if (AutoSpeedID + 1 > (int)AutoPlaySpeed.Quick)
                {
                    AutoSpeedID = (int)AutoPlaySpeed.Normal;
                }
                else
                {
                    AutoSpeedID += 1;
                }
            }
        }

        // �I�[�g�@�\�̐؂�ւ�
        public void ChangeAutoMode()
        {
            // �X�s�[�h���[�h�������Ȃ略���o���t�F�[�Y�ɂȂ�܂Ŏ��s(�����̕s���}���邽��)
            if(AutoSpeedID > (int)AutoPlaySpeed.Normal)
            {
                if(!HasAuto)
                {
                    HasAuto = true;
                }
                else
                {
                    Debug.Log("Fast Auto will end when payout is done");
                    HasWaitingCancel = true;
                }
            }
            // �ʏ�X�s�[�h���͂��ł��؂��
            else
            {
                HasAuto = !HasAuto;
            }

            HasStopPosDecided = false;
        }

        // �����I�[�g�I���`�F�b�N
        public bool CheckEndFastAuto()
        {
            if(HasWaitingCancel)
            {
                HasAuto = false;
                HasWaitingCancel = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        // �I�[�g��~�ʒu�����Z�b�g
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)AutoStopOrder.First] = 0;
            AutoStopOrders[(int)AutoStopOrder.Second] = 0;
            AutoStopOrders[(int)AutoStopOrder.Third] = 0;

            //Debug.Log("AutoPos Reset");
        }

        // �I�[�g���������t���O�A�������瓾��
        public void GetAutoStopPos(FlagId flag, BonusType holdingBonus, int bigChanceGames, int remainingJac)
        {
            //Debug.Log("GetPos");
            SetAutoStopOrder();
            AutoStopPos = autoAI.GetStopPos(flag, AutoStopOrders[(int)AutoStopOrder.First], holdingBonus, bigChanceGames, remainingJac);
            //Debug.Log("Pos created");

            HasStopPosDecided = true;
            //Debug.Log("AutoPos Decided");
        }

        // �I�[�g�������̐ݒ蔽�f
        private void SetAutoStopOrder()
        {       
            switch(AutoOrderID)
            {
                case (int)AutoStopOrderOptions.LMR:
                    ChangeAutoStopOrder(ReelID.ReelLeft, ReelID.ReelMiddle, ReelID.ReelRight);
                    break;

                case (int)AutoStopOrderOptions.LRM:
                    ChangeAutoStopOrder(ReelID.ReelLeft, ReelID.ReelRight, ReelID.ReelMiddle);
                    break;

                case (int)AutoStopOrderOptions.MLR:
                    ChangeAutoStopOrder(ReelID.ReelMiddle, ReelID.ReelLeft, ReelID.ReelRight);
                    break;

                case (int)AutoStopOrderOptions.MRL:
                    ChangeAutoStopOrder(ReelID.ReelMiddle, ReelID.ReelRight, ReelID.ReelLeft);
                    break;

                case (int)AutoStopOrderOptions.RLM:
                    ChangeAutoStopOrder(ReelID.ReelRight, ReelID.ReelLeft, ReelID.ReelMiddle);
                    break;

                case (int)AutoStopOrderOptions.RML:
                    ChangeAutoStopOrder(ReelID.ReelRight, ReelID.ReelMiddle, ReelID.ReelLeft);
                    break;
            }
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
                //Debug.Log("Failed to change because there are duplicated orders");
            }
        }
    }
}
