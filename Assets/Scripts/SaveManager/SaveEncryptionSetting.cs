using System.Security.Cryptography;

namespace ReelSpinGame_Save.Encryption
{
    // ƒZ[ƒuˆÃ†‰»‚Ìİ’è
    public class SaveEncryptionSetting
    {
        public const int BlockSize = 128;
        public const int KeySize = 256;
        public const CipherMode Mode = CipherMode.CBC;
        public const PaddingMode Padding = PaddingMode.PKCS7;
    }
}