using ReelSpinGame_Reels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ReelSpinGame_Reels.ReelData;

public class SymbolChecker
{
    // �}���̔���p

    // const

    // var

    // �����o�����C���̃f�[�^
    class PayoutLineData
    {
        // var

        // �����o�����C��(�����t��byte)
        public sbyte[] PayoutLine { get; private set; }

        // �L���ɕK�v�ȃx�b�g����
        public byte BetCondition { get; private set; }


        //�R���X�g���N�^
        public PayoutLineData(sbyte[] buffer)
        {
            // �Ō�̍s�ȊO�͕����o�����C���̃f�[�^�Ȃ̂Ŕz��ɂ���
            PayoutLine = new sbyte[buffer.Length - 1]; 
            Array.Copy(buffer, PayoutLine, buffer.Length - 1);

            // �Ō�̍s����f�[�^��ǂݍ���
            if (buffer[buffer.Length - 1] <= 0)
            {
                throw new Exception("Invalid Data at BetCondition, It must be within 1-3");
            }
            this.BetCondition = (byte)buffer[buffer.Length - 1];
        }
    }

    // �e�����o�����C���̃f�[�^
    private List<PayoutLineData> payoutLineDatas;


    // �R���X�g���N�^
    public SymbolChecker(StreamReader payoutLineData)
    {
        // �����o�����C���̓ǂݍ���
        payoutLineDatas = new List<PayoutLineData>();

        // �f�[�^�ǂݍ���
        while(!payoutLineData.EndOfStream)
        {
            sbyte[] byteBuffer = Array.ConvertAll(payoutLineData.ReadLine().Split(','), sbyte.Parse);
            payoutLineDatas.Add(new PayoutLineData(byteBuffer));
        }

        // �f�o�b�O�p
        foreach (PayoutLineData data in payoutLineDatas)
        {
            string line = "";
            foreach (sbyte b in data.PayoutLine)
            {
                line += b.ToString();
            }
            Debug.Log(line + "," + data.BetCondition);
        }
        Debug.Log("PayoutLine Data loaded");
    }

    // func

    // ���C������
    public int CheckPayout(ReelObject[] reelObjects, int betAmount)
    {
        // �����o������(�ő�15���܂�)
        int payoutAmounts = 0;

        // �e���C�����略���o���̃`�F�b�N������
        foreach (PayoutLineData lineData in payoutLineDatas)
        {
            // �x�b�g�����̏����𖞂����Ă��邩�`�F�b�N
            if (betAmount >= lineData.BetCondition)
            {
                // ���ʂ����X�g�ɂ܂Ƃ߂�
                List<ReelSymbols> result = new List<ReelData.ReelSymbols>();

                // �e���[���̕����o�����`�F�b�N
                for(int i = 0; i < reelObjects.Length; i++)
                {
                    result.Add(reelObjects[i].ReelData.GetReelSymbol(lineData.PayoutLine[i]));
                }

                // �f�o�b�O�p
                string lineBuffer = "";
                foreach(byte b in lineData.PayoutLine)
                {
                    lineBuffer += b.ToString();
                }

                string resultBuffer = "";
                foreach (ReelSymbols symbol in result)
                {
                    resultBuffer += symbol.ToString();
                }
                Debug.Log(lineBuffer + "," + resultBuffer);

                // �}���\�����X�g�ƌ���ׂĊY��������̂�����Γ��I�B�����o���A�{�[�i�X�A���v���C����������B
                // �{�[�i�X�͔񓖑I�ł��X�g�b�N�����

                // �f�o�b�O�p
                // �S�ē����}���������Ă�����HIT��Ԃ�
                if (result[0] == result[1] && result[0] == result[2])
                {
                    Debug.Log("HIT!");

                    payoutAmounts = 1;
                }
            }
        }

        // �ŏI�I�ȕ����o��������Ԃ�
        return payoutAmounts;
    }
}
