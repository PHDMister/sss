using System;
using UnityEngine;

public class AutoincrementSeeds
{
    //自增数
    protected uint _s_uint_code;
    protected uint _uInt_code;
    public uint uIntCode
    {
        get
        {
            if (_uInt_code + 1 >= int.MaxValue) _uInt_code = _s_uint_code;
            _uInt_code++;
            return _uInt_code;
        }
    }

    protected int _s_int_code;
    protected int _int_code;
    public int IntCode
    {
        get
        {
            if (_int_code + 1 >= int.MaxValue) _int_code = _s_int_code;
            _int_code++;
            return _int_code;
        }
    }

    //随机数 随机种子
    protected int RandomSeeds;
    protected int RandomTims;
    protected System.Random random;

    public AutoincrementSeeds()
    {
        _uInt_code = 0;
        _int_code = 0;
    }
    public AutoincrementSeeds(int source, uint sourceCode)
    {
        _s_int_code = source;
        _s_uint_code = sourceCode;

        _int_code = source;
        _uInt_code = sourceCode;
    }
    public AutoincrementSeeds(int randomSeed)
    {
        RandomSeeds = randomSeed;
        random = new System.Random(randomSeed);
    }

    public void ResetRandomSeed(int seed)
    {
        RandomTims = 0;
        RandomSeeds = seed;
        random = new System.Random(seed);
    }
    public void ResetAutoAddtive()
    {
        _uInt_code = _s_uint_code;
        _int_code = _s_int_code;
    }
    public int Random(int max, string flag = "")
    {
        RandomTims++;
        int rand = random.Next(max);
        if (!string.IsNullOrEmpty(flag))
        {
            Debug.Log($" AutoincrementSeeds  {flag}  RandomTims:{RandomTims}  rand:{rand}");
        }
        return rand;
    }
    public int Random(int min, int max, string flag = "")
    {
        RandomTims++;
        int rand = random.Next(min, max);
        if (!string.IsNullOrEmpty(flag))
        {
            Debug.Log($" AutoincrementSeeds  {flag}  RandomTims:{RandomTims}  rand:{rand}");
        }
        return rand;
    }



    public override string ToString()
    {
        string log = $" AutoincrementSeeds  uIntCode:{_uInt_code}  IntCode :{_int_code}  " +
                     $"RandomSeeds:{RandomSeeds} RandomTims:{RandomTims}";
        return log;
    }
}
