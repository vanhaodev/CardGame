using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class EncryptionAes
{
    private static readonly string keyString = "Lx8jOwJeJpq7hVUB6Owf2U5lNblUkFJe"; // Đổi key này thành key thực tế của bạn
    //aesAlg.GenerateIV() cho ra kết quả mã hoá khác nhau cho dù tài khoản mà mật khẩu giống nhau 
    public string EncryptString(string plainText)
    {
        try
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(keyString);
                aesAlg.GenerateIV(); // Tạo IV ngẫu nhiên mới cho mỗi lần mã hóa

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);

                using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    byte[] ivBytes = aesAlg.IV;
                    byte[] encryptedBytes = msEncrypt.ToArray();

                    // Kết hợp IV và dữ liệu đã được mã hóa thành một byte array
                    byte[] combinedBytes = new byte[ivBytes.Length + encryptedBytes.Length];
                    Array.Copy(ivBytes, 0, combinedBytes, 0, ivBytes.Length);
                    Array.Copy(encryptedBytes, 0, combinedBytes, ivBytes.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(combinedBytes);
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    public string DecryptString(string cipherText)
    {
        try
        {
            byte[] combinedBytes = Convert.FromBase64String(cipherText);

            // Tách IV và dữ liệu đã được mã hóa từ combinedBytes
            byte[] ivBytes = new byte[16]; // Kích thước của IV là 16 bytes
            byte[] encryptedBytes = new byte[combinedBytes.Length - ivBytes.Length];
            Array.Copy(combinedBytes, 0, ivBytes, 0, ivBytes.Length);
            Array.Copy(combinedBytes, ivBytes.Length, encryptedBytes, 0, encryptedBytes.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(keyString);
                aesAlg.IV = ivBytes; // Sử dụng IV đã được trích xuất từ combinedBytes

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (System.IO.StreamReader srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch { return string.Empty; }
    }
}