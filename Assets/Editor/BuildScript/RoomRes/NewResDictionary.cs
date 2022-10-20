using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
// using NUnit.Framework.Internal;
using UnityEngine;


public class SingleNewResInfo
{
    /// <summary>
    /// 这个文件的名字
    /// </summary>
    public string name;

    /// <summary>
    /// 这个文件的路径
    /// </summary>
    public string path;

    /// <summary>
    /// 这个文件的md5散列码
    /// </summary>
    public string md5;

    /// <summary>
    /// 这个文件在哪个模块
    /// </summary>
    public string module;

    /// <summary>
    /// 这个文件文件大小
    /// </summary>
    public int size;

    /// <summary>
    /// 这个文件的构建时间
    /// </summary>
    public int buildtime;
}

public class NewResDictionary
{
    // public Dictionary<string, SingleNewResInfo>                   ResDictionary;
    public int                                                    version;
    public Dictionary<string,Dictionary<string,SingleNewResInfo>> resdictionary;

    // public NewResDictionary()
    // {
    //     resdictionary = new Dictionary<string,Dictionary<string,SingleNewResInfo>>();
    // }

    public static NewResDictionary Deserialize(string jsonStr)
    {
        NewResDictionary ret = new NewResDictionary();
        try
        {
            ret = LitJson.JsonMapper.ToObject<NewResDictionary>(jsonStr);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return ret;
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="dic">  </param>
    /// <param name="path"> json要输出的路径 </param>
    public void Serialize(NewResDictionary dic, string path)
    {
        try
        {
            string jsonStr = LitJson.JsonMapper.ToJson(dic);
            File.WriteAllText(path, jsonStr);
        }
        catch (Exception e)
        {
            Debug.LogError("Serialize Error");
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// json2数据信息
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Dictionary<string,Dictionary<string,SingleNewResInfo>> GetInfo(string path)
    {
        return Deserialize(path).resdictionary;
    }

    /// <summary>
    /// json2版本号
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetVersion(string path)
    {
        return Deserialize(path).version;
    }



}

