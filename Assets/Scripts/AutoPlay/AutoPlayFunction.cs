using ReelSpinGame_AutoPlay.AI;
using System;
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
        public enum AutoStopOrder { First, Second, Third }
        // �I�[�g�������w��p(��:L, ��:M, �E:R)
        public enum AutoStopOrderOptions { LMR, LRM, MLR, MRL, RLM, RML }
        // �I�[�g���x(�ʏ�A�����A������)
        public enum AutoPlaySpeed { Normal, Fast, Quick }
        // ���݂̃I�[�g���[�h
        public enum AutoPlaySequence { AutoInsert, AutoStopReel }
        // �I�[�g�I�������Ŏg������ID
        // ����(������, BIG����, REG����, �{�[�i�X����, �{�[�i�X�I��(�ǂ��炩), ���[�`��, �w��Q�[��������)
        public enum AutoEndConditionID { None, BIG, REG, AnyBonus, EndBonus, RiichiPattern, Games }

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

        // �I������
        // �c��I�[�g��
        public int RemainingAutoGames { get; private set; }
        // �I������
        public int autoEndConditionID { get; private set; }

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
            RemainingAutoGames = 0;
            autoEndConditionID = (int)AutoEndConditionID.None;

            // ��~�ʒu�̔z��쐬
            AutoStopPos = new int[] { 0, 0, 0 };
            HasWaitingCancel = false;
        }

        // func

        // �I�[�g����Ԃ�
        public string GetOrderName() => Enum.ToObject(typeof(AutoStopOrderOptions), AutoOrderID).ToString();

        // �X�s�[�h����Ԃ�
        public string GetSpeedName() => Enum.ToObject(typeof(AutoPlaySpeed), AutoSpeedID).ToString();

        // �I�[�g�I����������Ԃ�
        public string GetConditionName() => Enum.ToObject(typeof(AutoEndConditionID), autoEndConditionID).ToString();

        // �Z�p����̗L����Ԃ�
        public bool GetHasTechnicalPlay() => autoAI.HasTechnicalPlay;

        // ���[�`�ڗD�搧��̗L����Ԃ�
        public bool GetHasRiichiStop() => autoAI.HasRiichiStop;

        // ���[�`�ڂ��~�߂�����Ԃ�
        public bool GetHasStoppedRiichiPtn() => autoAI.HasStoppedRiichiPtn;

        // 1���|���{�[�i�X�����̗L����Ԃ�
        public bool GetHasOneBetBonusLineUp() => autoAI.HasOneBetBonusLineUp;

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

        // �I�[�g�X�s�[�h�ԍ��ύX(�f�o�b�O�p)
        public void ChangeAutoSpeed()
        {
            if (!HasAuto)
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

        // �I�[�gBIG�����F�I��
        public void ChangeBigLineUpColor(BigColor color) => autoAI.PlayerSelectedBigColor = color;

        // �I�[�g�@�\�̐؂�ւ�
        public void ChangeAutoMode(AutoEndConditionID conditionID, int conditionGames, bool hasTechnicalPlay, 
            bool hasRiichiStop, bool hasOneBetBonusLineUp, BigColor favoriteBIGColor)
        {
            // �X�s�[�h���[�h�������Ȃ略���o���t�F�[�Y�ɂȂ�܂Ŏ��s(�����̕s���}���邽��)
            if (AutoSpeedID > (int)AutoPlaySpeed.Normal)
            {
                if (!HasAuto)
                {
                    HasAuto = true;
                }
                else
                {
                    HasWaitingCancel = true;
                }
            }
            // �ʏ�X�s�[�h���͂��ł��؂��
            else
            {
                HasAuto = !HasAuto;
            }

            // ��~�ʒu���Z�b�g
            HasStopPosDecided = false;

            // �w�肵���I�[�g�I��������t�^
            autoEndConditionID = (int)conditionID;
            RemainingAutoGames = conditionGames;

            // AI�ݒ�
            // �Z�p���
            autoAI.HasTechnicalPlay = hasTechnicalPlay;

            // �Z�p���������ꍇ�̓��[�`�ڂŎ~�߂�ݒ�A1���|���ő�����ݒ�Ȃǂ�����
            if (hasTechnicalPlay)
            {
                autoAI.HasRiichiStop = (autoEndConditionID == (int)AutoEndConditionID.RiichiPattern || hasRiichiStop);
                autoAI.HasStoppedRiichiPtn = false;
                // �{�[�i�X��1���|���ő��������邩(���[�`�ڐݒ肪�o�Ă�ꍇ�̂ݗL��)
                autoAI.HasOneBetBonusLineUp = hasRiichiStop && hasOneBetBonusLineUp;
            }
            else
            {
                autoAI.HasRiichiStop = false;
                autoAI.HasStoppedRiichiPtn = false;
                autoAI.HasOneBetBonusLineUp = false;
            }

            // �_��BIG�̐F�ݒ�
            autoAI.PlayerSelectedBigColor = favoriteBIGColor;
        }

        // �����I�[�g�I���`�F�b�N
        public void CheckFastAutoCancelled()
        {
            if(HasWaitingCancel)
            {
                FinishAutoForce();
            }
        }

       // �c��I�[�g�Q�[�����`�F�b�N
        public void CheckRemainingAuto()
        {
            if(autoEndConditionID == (int)AutoEndConditionID.Games)
            {
                RemainingAutoGames -= 1;

                if(RemainingAutoGames == 0)
                {
                    FinishAutoForce();
                }
            }
        }

        // �{�[�i�X�����ɂ��I�[�g�I���`�F�b�N
        public void CheckAutoEndByBonus(int bonusTypeID)
        {
            switch(autoEndConditionID)
            {
                // BIG���I
                case (int)AutoEndConditionID.BIG:

                    if(bonusTypeID == (int)BonusTypeID.BonusBIG)
                    {
                        FinishAutoForce();
                    }
                    break;

                // REG���I
                case (int)AutoEndConditionID.REG:
                    if (bonusTypeID == (int)BonusTypeID.BonusREG)
                    {
                        FinishAutoForce();
                    }
                    break;

                // �ǂ��炩���I
                case (int)AutoEndConditionID.AnyBonus:
                    if (bonusTypeID == (int)BonusTypeID.BonusBIG || bonusTypeID == (int)BonusTypeID.BonusREG)
                    {
                        FinishAutoForce();
                    }
                    break;

                // ���[�`��
                case (int)AutoEndConditionID.RiichiPattern:
                    if(autoAI.HasStoppedRiichiPtn)
                    {
                        FinishAutoForce();
                    }
                    break;
            }
        }

        // �{�[�i�X�I���ɂ��I�[�g�I���`�F�b�N
        public void CheckAutoEndByBonusFinish(int bonusStatusID)
        {
            // �ʏ펞�ɖ߂����ꍇ�̓I�[�g�I��
            if(autoEndConditionID == (int)AutoEndConditionID.EndBonus && bonusStatusID == (int)BonusStatus.BonusNone)
            {
                FinishAutoForce();
            }
        }

        // �I�[�g�̋����I��
        public void FinishAutoForce()
        {
            HasAuto = false;
            HasWaitingCancel = false;
            HasStopPosDecided = false;
            RemainingAutoGames = 0;
            autoAI.HasStoppedRiichiPtn = false;
        }
        
        // �I�[�g��~�ʒu�����Z�b�g
        public void ResetAutoStopPos()
        {
            HasStopPosDecided = false;
            AutoStopOrders[(int)AutoStopOrder.First] = 0;
            AutoStopOrders[(int)AutoStopOrder.Second] = 0;
            AutoStopOrders[(int)AutoStopOrder.Third] = 0;
        }

        // �I�[�g���������t���O�A�������瓾��
        public void GetAutoStopPos(FlagId flag, BonusTypeID holdingBonus, int bigChanceGames, int remainingJac, int betAmount)
        {
            SetAutoStopOrder();
            AutoStopPos = autoAI.GetStopPos(flag, AutoStopOrders[(int)AutoStopOrder.First], holdingBonus, bigChanceGames, remainingJac, betAmount);
            HasStopPosDecided = true;
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
        }
    }
}
