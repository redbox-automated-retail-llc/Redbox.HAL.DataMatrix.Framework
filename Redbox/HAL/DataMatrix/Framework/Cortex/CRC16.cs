namespace Redbox.HAL.DataMatrix.Framework.Cortex
{
    internal static class CRC16
    {
        private static ushort[] Table = new ushort[256]
        {
      (ushort) 0,
      (ushort) 4129,
      (ushort) 8258,
      (ushort) 12387,
      (ushort) 16516,
      (ushort) 20645,
      (ushort) 24774,
      (ushort) 28903,
      (ushort) 33032,
      (ushort) 37161,
      (ushort) 41290,
      (ushort) 45419,
      (ushort) 49548,
      (ushort) 53677,
      (ushort) 57806,
      (ushort) 61935,
      (ushort) 4657,
      (ushort) 528,
      (ushort) 12915,
      (ushort) 8786,
      (ushort) 21173,
      (ushort) 17044,
      (ushort) 29431,
      (ushort) 25302,
      (ushort) 37689,
      (ushort) 33560,
      (ushort) 45947,
      (ushort) 41818,
      (ushort) 54205,
      (ushort) 50076,
      (ushort) 62463,
      (ushort) 58334,
      (ushort) 9314,
      (ushort) 13379,
      (ushort) 1056,
      (ushort) 5121,
      (ushort) 25830,
      (ushort) 29895,
      (ushort) 17572,
      (ushort) 21637,
      (ushort) 42346,
      (ushort) 46411,
      (ushort) 34088,
      (ushort) 38153,
      (ushort) 58862,
      (ushort) 62927,
      (ushort) 50604,
      (ushort) 54669,
      (ushort) 13907,
      (ushort) 9842,
      (ushort) 5649,
      (ushort) 1584,
      (ushort) 30423,
      (ushort) 26358,
      (ushort) 22165,
      (ushort) 18100,
      (ushort) 46939,
      (ushort) 42874,
      (ushort) 38681,
      (ushort) 34616,
      (ushort) 63455,
      (ushort) 59390,
      (ushort) 55197,
      (ushort) 51132,
      (ushort) 18628,
      (ushort) 22757,
      (ushort) 26758,
      (ushort) 30887,
      (ushort) 2112,
      (ushort) 6241,
      (ushort) 10242,
      (ushort) 14371,
      (ushort) 51660,
      (ushort) 55789,
      (ushort) 59790,
      (ushort) 63919,
      (ushort) 35144,
      (ushort) 39273,
      (ushort) 43274,
      (ushort) 47403,
      (ushort) 23285,
      (ushort) 19156,
      (ushort) 31415,
      (ushort) 27286,
      (ushort) 6769,
      (ushort) 2640,
      (ushort) 14899,
      (ushort) 10770,
      (ushort) 56317,
      (ushort) 52188,
      (ushort) 64447,
      (ushort) 60318,
      (ushort) 39801,
      (ushort) 35672,
      (ushort) 47931,
      (ushort) 43802,
      (ushort) 27814,
      (ushort) 31879,
      (ushort) 19684,
      (ushort) 23749,
      (ushort) 11298,
      (ushort) 15363,
      (ushort) 3168,
      (ushort) 7233,
      (ushort) 60846,
      (ushort) 64911,
      (ushort) 52716,
      (ushort) 56781,
      (ushort) 44330,
      (ushort) 48395,
      (ushort) 36200,
      (ushort) 40265,
      (ushort) 32407,
      (ushort) 28342,
      (ushort) 24277,
      (ushort) 20212,
      (ushort) 15891,
      (ushort) 11826,
      (ushort) 7761,
      (ushort) 3696,
      (ushort) 65439,
      (ushort) 61374,
      (ushort) 57309,
      (ushort) 53244,
      (ushort) 48923,
      (ushort) 44858,
      (ushort) 40793,
      (ushort) 36728,
      (ushort) 37256,
      (ushort) 33193,
      (ushort) 45514,
      (ushort) 41451,
      (ushort) 53516,
      (ushort) 49453,
      (ushort) 61774,
      (ushort) 57711,
      (ushort) 4224,
      (ushort) 161,
      (ushort) 12482,
      (ushort) 8419,
      (ushort) 20484,
      (ushort) 16421,
      (ushort) 28742,
      (ushort) 24679,
      (ushort) 33721,
      (ushort) 37784,
      (ushort) 41979,
      (ushort) 46042,
      (ushort) 49981,
      (ushort) 54044,
      (ushort) 58239,
      (ushort) 62302,
      (ushort) 689,
      (ushort) 4752,
      (ushort) 8947,
      (ushort) 13010,
      (ushort) 16949,
      (ushort) 21012,
      (ushort) 25207,
      (ushort) 29270,
      (ushort) 46570,
      (ushort) 42443,
      (ushort) 38312,
      (ushort) 34185,
      (ushort) 62830,
      (ushort) 58703,
      (ushort) 54572,
      (ushort) 50445,
      (ushort) 13538,
      (ushort) 9411,
      (ushort) 5280,
      (ushort) 1153,
      (ushort) 29798,
      (ushort) 25671,
      (ushort) 21540,
      (ushort) 17413,
      (ushort) 42971,
      (ushort) 47098,
      (ushort) 34713,
      (ushort) 38840,
      (ushort) 59231,
      (ushort) 63358,
      (ushort) 50973,
      (ushort) 55100,
      (ushort) 9939,
      (ushort) 14066,
      (ushort) 1681,
      (ushort) 5808,
      (ushort) 26199,
      (ushort) 30326,
      (ushort) 17941,
      (ushort) 22068,
      (ushort) 55628,
      (ushort) 51565,
      (ushort) 63758,
      (ushort) 59695,
      (ushort) 39368,
      (ushort) 35305,
      (ushort) 47498,
      (ushort) 43435,
      (ushort) 22596,
      (ushort) 18533,
      (ushort) 30726,
      (ushort) 26663,
      (ushort) 6336,
      (ushort) 2273,
      (ushort) 14466,
      (ushort) 10403,
      (ushort) 52093,
      (ushort) 56156,
      (ushort) 60223,
      (ushort) 64286,
      (ushort) 35833,
      (ushort) 39896,
      (ushort) 43963,
      (ushort) 48026,
      (ushort) 19061,
      (ushort) 23124,
      (ushort) 27191,
      (ushort) 31254,
      (ushort) 2801,
      (ushort) 6864,
      (ushort) 10931,
      (ushort) 14994,
      (ushort) 64814,
      (ushort) 60687,
      (ushort) 56684,
      (ushort) 52557,
      (ushort) 48554,
      (ushort) 44427,
      (ushort) 40424,
      (ushort) 36297,
      (ushort) 31782,
      (ushort) 27655,
      (ushort) 23652,
      (ushort) 19525,
      (ushort) 15522,
      (ushort) 11395,
      (ushort) 7392,
      (ushort) 3265,
      (ushort) 61215,
      (ushort) 65342,
      (ushort) 53085,
      (ushort) 57212,
      (ushort) 44955,
      (ushort) 49082,
      (ushort) 36825,
      (ushort) 40952,
      (ushort) 28183,
      (ushort) 32310,
      (ushort) 20053,
      (ushort) 24180,
      (ushort) 11923,
      (ushort) 16050,
      (ushort) 3793,
      (ushort) 7920
        };

        internal static ushort CRC(ushort initialValue, byte[] data)
        {
            int length = data.Length;
            int num1 = 0;
            ushort num2 = initialValue;
            while (length-- > 0)
                num2 = (ushort)((int)num2 << 8 ^ (int)CRC16.Table[(int)num2 >> 8 ^ (int)data[num1++]]);
            return num2;
        }

        internal static ushort CRC(ushort initialValue, byte data)
        {
            return (ushort)((uint)initialValue << 8 ^ (uint)CRC16.Table[(int)initialValue >> 8 ^ (int)data]);
        }
    }
}
