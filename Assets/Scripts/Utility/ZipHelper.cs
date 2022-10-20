// @Author: tanjinhua
// @Date: 2021/3/15  15:04


using System;
using System.IO;
using zlib;
using Unity.Utility;

/// <summary>
/// ����ڶ��߳���ʹ����ǵ� lock 
/// </summary>
public static class ZipHelper
{
    private static byte[] _byteCache = new byte[1024];

    /// <summary>
    /// zlib ѹ��
    /// </summary>
    /// <param name="inData"></param>
    /// <param name="inLen"></param>
    /// <returns></returns>
    public static void Compress(byte[] inData, int inLen, ref byte[] outData, ref int outLen)
    {
        //         MemoryStream inputStream = new MemoryStream(inData);
        // 
        //         MemoryStream outStream = new MemoryStream();
        //         ZOutputStream streamZOut = new ZOutputStream(outStream, zlibConst.Z_DEFAULT_COMPRESSION);
        //         CopyStream(inputStream, streamZOut);
        //         streamZOut.finish();
        // 
        //         byte[] outPutByteArray = new byte[outStream.Length];
        //         outStream.Position = 0;
        //         outStream.Read(outPutByteArray, 0, outPutByteArray.Length);
        //         outStream.Close();
        //         inputStream.Close();
        //         return outPutByteArray;

        outData = null;
        outLen = 0;
        if (inData == null || inLen <= 0)
            return;

        MemoryStream memOut = new MemoryStream();
        ZOutputStream outStream = new ZOutputStream(memOut, zlibConst.Z_DEFAULT_COMPRESSION);
        outStream.Write(inData, 0, inLen);
        outStream.Flush();
        outStream.finish();

        outLen = (int)memOut.Length;
        outData = GetByteCache(outLen);

        memOut.Position = 0;
        memOut.Read(outData, 0, outLen);

        memOut.Close();
        outStream.Close();
    }

    public static void Decompress(byte[] inData, int offset, int inLen, ref byte[] outData, ref int outLen)
    {
        //         MemoryStream inputStream = new MemoryStream(sourceByte);
        // 
        //         MemoryStream outputStream = new MemoryStream();
        //         ZOutputStream outZStream = new ZOutputStream(outputStream);
        //         CopyStream(inputStream, outZStream);
        //         outZStream.finish();
        // 
        //         byte[] outputBytes = new byte[outputStream.Length];
        //         outputStream.Position = 0;
        //         outputStream.Read(outputBytes, 0, outputBytes.Length);
        //         outputStream.Close();
        //         inputStream.Close();
        //         return outputBytes;

        outData = null;
        outLen = 0;
        if (inData == null || inLen <= 0)
            return;

        MemoryStream memOut = new MemoryStream();
        ZOutputStream outStream = new ZOutputStream(memOut);
        outStream.Write(inData, offset, inLen);
        outStream.Flush();
        outStream.finish();

        outLen = (int)memOut.Length;
        outData = GetByteCache(outLen);

        memOut.Position = 0;
        memOut.Read(outData, 0, outLen);

        memOut.Close();
        outStream.Close();
    }

//     private static void CopyStream(Stream input, Stream output)
//     {
//         lock (_lock)
//         {
//             //byte[] buffer = new byte[2000];
//             Array.Clear(_buffer, 0, _bufferSize);
// 
//             int len = input.Read(_buffer, 0, _bufferSize);
//             while (len > 0)
//             {
//                 output.Write(_buffer, 0, len);
//                 len = input.Read(_buffer, 0, _bufferSize);
//             }
//             output.Flush();
//         }
//     }

    private static byte[] GetByteCache(int byteLen)
    {
        if (byteLen <= 0)
            return null;

        if(byteLen >= _byteCache.Length)
        {
            //WLDebugTrace.Trace("[ZipHelper] GetByteCache, byteLen=" + byteLen);
            int len = byteLen + 512;
            _byteCache = new byte[len];
        }

        return _byteCache;
    }
}
