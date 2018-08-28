using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StateChecker
{
    private static MD5 _md5;

    public static byte[] ToByteArray(object state)
    {
        string json = JsonUtility.ToJson(state);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }
    
    static public string GetMd5Hash(byte[] byteList)
    {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5.ComputeHash(byteList);

        StringBuilder sBuilder = new StringBuilder();
        for(int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    static public string GetMd5Hash(object state)
    {
        byte[] input = ToByteArray(state);
        return GetMd5Hash(input);
    }

    // Verify a hash against a string.
    static public bool VerifyMd5Hash(string a, string b)
    {
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        if(comparer.Compare(a, b) == 0)
        {
            return true;
        }

        return false;
    }

    private static MD5 md5
    {
        get
        {
            if(_md5 == null)
            {
                _md5 = MD5.Create();
            }
            return _md5;
        }
    }
}
