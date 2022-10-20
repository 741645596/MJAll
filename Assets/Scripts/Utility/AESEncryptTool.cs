using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Unity.Utility
{ 

/// <summary>
/// AES加密工具类
/// </summary>
public class AESEncryptTool
{
    public static string AES_KEY = "DZ8DDLL";
    private static string AES_Head = "AESEncrypt";
    private static int AES_Head_Length = 10;

    /// <summary>
    /// 文件加密
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="encrptyKey">密匙</param>
    public static void AESFileEncrypt(string path, string encrptyKey)
    {
        if (!File.Exists(path))
        {
            return;
        }
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fileStream != null)
                {
                    byte[] array = new byte[AES_Head_Length];
                    fileStream.Read(array, 0, array.Length);
                    if (!(Encoding.UTF8.GetString(array) == AES_Head))
                    {
                        fileStream.Seek(0L, SeekOrigin.Begin);
                        byte[] array2 = new byte[fileStream.Length];
                        fileStream.Read(array2, 0, Convert.ToInt32(fileStream.Length));
                        fileStream.Seek(0L, SeekOrigin.Begin);
                        fileStream.SetLength(0L);
                        byte[] bytes = Encoding.UTF8.GetBytes(AES_Head);
                        fileStream.Write(bytes, 0, bytes.Length);
                        byte[] array3 = AESEncrypt(array2, encrptyKey);
                        fileStream.Write(array3, 0, array3.Length);
                    }
                }
            }
        }
        catch (Exception message)
        {
            Debug.LogError(message);
        }
    }

    /// <summary>
    /// 文件解密
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="encrptyKey">密匙</param>
    public static void AESFileDecrypt(string path, string encrptyKey)
    {
        if (!File.Exists(path))
        {
            return;
        }
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fileStream != null)
                {
                    byte[] array = new byte[AES_Head_Length];
                    fileStream.Read(array, 0, array.Length);
                    if (Encoding.UTF8.GetString(array) == AES_Head)
                    {
                        byte[] array2 = new byte[fileStream.Length - (long)array.Length];
                        fileStream.Read(array2, 0, Convert.ToInt32(fileStream.Length - (long)array.Length));
                        fileStream.Seek(0L, SeekOrigin.Begin);
                        fileStream.SetLength(0L);
                        byte[] array3 = AESDecrypt(array2, encrptyKey);
                        fileStream.Write(array3, 0, array3.Length);
                    }
                }
            }
        }
        catch (Exception message)
        {
            Debug.LogError(message);
        }
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="encryptByte">byte[]</param>
    /// <param name="encryptKey">密匙</param>
    /// <returns></returns>
    public static byte[] AESEncrypt(byte[] encryptByte, string encryptKey)
    {
        if (encryptByte.Length == 0)
        {
            throw new Exception("明文不得为空");
        }
        if (string.IsNullOrEmpty(encryptKey))
        {
            throw new Exception("密钥不得为空");
        }
        byte[] rgbIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] rgbSalt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael rijndael = Rijndael.Create();
        byte[] result;
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(encryptKey, rgbSalt);
            ICryptoTransform transform = rijndael.CreateEncryptor(passwordDeriveBytes.GetBytes(32), rgbIV);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(encryptByte, 0, encryptByte.Length);
            cryptoStream.FlushFinalBlock();
            result = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();
            cryptoStream.Close();
            cryptoStream.Dispose();
        }
        catch (IOException ex)
        {
            throw ex;
        }
        catch (CryptographicException ex2)
        {
            throw ex2;
        }
        catch (ArgumentException ex3)
        {
            throw ex3;
        }
        catch (Exception ex4)
        {
            throw ex4;
        }
        finally
        {
            rijndael.Clear();
        }
        return result;
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="decryptByte">byte[]</param>
    /// <param name="decryptKey">密匙</param>
    /// <returns></returns>
    public static byte[] AESDecrypt(byte[] decryptByte, string decryptKey)
    {
        if (decryptByte.Length == 0)
        {
            throw new Exception("密文不得为空");
        }
        if (string.IsNullOrEmpty(decryptKey))
        {
            throw new Exception("密钥不得为空");
        }
        byte[] rgbIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] rgbSalt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael rijndael = Rijndael.Create();
        byte[] result;
        try
        {
            MemoryStream memoryStream = new MemoryStream();
            PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(decryptKey, rgbSalt);
            ICryptoTransform transform = rijndael.CreateDecryptor(passwordDeriveBytes.GetBytes(32), rgbIV);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(decryptByte, 0, decryptByte.Length);
            cryptoStream.FlushFinalBlock();
            result = memoryStream.ToArray();
            memoryStream.Close();
            memoryStream.Dispose();
            cryptoStream.Close();
            cryptoStream.Dispose();
        }
        catch (IOException ex)
        {
            throw ex;
        }
        catch (CryptographicException ex2)
        {
            throw ex2;
        }
        catch (ArgumentException ex3)
        {
            throw ex3;
        }
        catch (Exception ex4)
        {
            throw ex4;
        }
        finally
        {
            rijndael.Clear();
        }
        return result;
    }

    /// <summary>
    /// 解密带有加密头的字节流
    /// </summary>
    /// <param name="decryptByte">byte[]</param>
    /// <param name="decryptKey">密匙</param>
    /// <returns></returns>
    public static byte[] AESDecryptHasHead(byte[] decryptByte, string decryptKey)
    {
        decryptByte = decryptByte.Skip(AES_Head_Length).Take(decryptByte.Length - AES_Head_Length).ToArray();
        var result = AESDecrypt(decryptByte, decryptKey);
        return result;
    }

    /// <summary>
    /// 是否是加密的DLL
    /// </summary>
    /// <param name="bytesData"></param>
    /// <returns></returns>
    public static bool IsEncryptDLL(byte[] bytesData)
    {
        if (bytesData.Length == 0 || bytesData.Length < 10)
        {
            Debug.LogError("数据不正确");
            return false;
        }

        var headByte = bytesData.Skip(0).Take(10).ToArray();
        return Encoding.UTF8.GetString(headByte) == AES_Head;
    }

    public static byte[] ReadFile(string path, string encrptyKey)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        byte[] result = null;
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fileStream != null)
                {
                    byte[] array = new byte[AES_Head_Length];
                    fileStream.Read(array, 0, array.Length);
                    if (Encoding.UTF8.GetString(array) == AES_Head)
                    {
                        byte[] array2 = new byte[fileStream.Length - (long)array.Length];
                        fileStream.Read(array2, 0, Convert.ToInt32(fileStream.Length - (long)array.Length));
                        result = AESDecrypt(array2, encrptyKey);
                    }
                }
            }
        }
        catch (Exception message)
        {
            Debug.LogError(message);
        }
        return result;
    }

    /// <summary>
    /// 字符串加密
    /// </summary>
    /// <param name="encryptString"></param>
    /// <param name="encryptKey"></param>
    /// <returns></returns>
    public static string AESEncrypt(string encryptString, string encryptKey)
    {
        return Convert.ToBase64String(AESEncrypt(Encoding.Default.GetBytes(encryptString), encryptKey));
    }

    public static string AESDecrypt(string decryptString, string decryptKey)
    {
        return Convert.ToBase64String(AESDecrypt(Encoding.Default.GetBytes(decryptString), decryptKey));
    }
}

}