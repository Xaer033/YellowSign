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
    

    static string GetMd5Hash(object state)
    {
        byte[] input = ToByteArray(state);
        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5.ComputeHash(input);
        
        StringBuilder sBuilder = new StringBuilder();
        for(int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    // Verify a hash against a string.
    static bool VerifyMd5Hash(string a, string b)
    {
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        if(0 == comparer.Compare(a, b))
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
