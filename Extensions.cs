using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
namespace Extensions
{
    public static class Ext
    {
        public static BigInteger GetPrime(int size)
        {
            bool flag;
            BigInteger rndNum;
            rndNum = GetRndNum(size);

            flag = false;

            while (flag == false)
            {

                if ((rndNum == 2 || rndNum % 2 != 0) && (rndNum == 5 || rndNum % 5 != 0) && MillerPrimaryTest(rndNum, 40) == true)
                {
                    flag = true;
                }
                else
                {
                    rndNum++;
                }

            }

            return rndNum;
        }

        public static BigInteger GetPrime()
        {

            return GetPrime(16);
        
        }

        public static BigInteger GetRndNum(int size)
        {
            if (size < 8)
                throw new Exception("Минимальная битовая длинна генерируемого числа - 8 бит");
            
            byte[] b = new byte[size / 8+1];
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider(new CspParameters());
            rand.GetBytes(b);
            BigInteger bi = 128;

            for (int i = 0; i < b.Length-1; i++)
            {
                bi = bi | b[i];
                bi = bi << 8;
                
            }
            bi = bi >> 8;

            int offset = size - (b.Length-1)*8;
            if (offset != 0 && size>8)
            {
                bi = bi << offset;
                bi = bi | b[b.Length - 1];
            }

            return bi;
        
        }

        public static BigInteger GetRndNum()
        {
            
            return GetRndNum(16);
        }


        public static bool MillerPrimaryTest(BigInteger num, int round = 20)
        {
            int t;
            BigInteger x, s, a;
            bool flag;

            if (num == 2 || num==3)
            {
                return true;
            }

            if (num < 2 || num % 2 == 0)
            {
                return false;
            }

            s = num - 1;
            t = 0;
            while (s % 2 == 0)
            {
                s /= 2;
                t++;
            }

            for (int i = 0; i < round; i++)
            {

                flag = false;
                Random rnd = new Random();

                int l = num.ToByteArray().Length;

                do
                {

                    byte[] arr = new byte[l];
                    rnd.NextBytes(arr);
                    a = new BigInteger(arr);
                    a = BigInteger.Abs(a);

                }
                while (a < 2 || a >= num - 2);

                x = BigInteger.ModPow(a, s, num);
                x = x % num;

                if (x == 1 || x == num - 1)
                    continue;

                for (int j = 1; j < t; j++)
                {

                    x = x * x;
                    x = x % num;

                    if (x == 1)
                        return false;

                    if (x == num - 1)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag == true)
                    continue;

                return false;


            }
            return true;
        }

        public static void ExtEuclid(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y, out BigInteger d)
        {

            BigInteger q, r, x1, y1, x2, y2;

            if (b == 0)
            {
                x = 1;
                y = 0;
                d = x;
            }
            else
            {

                x1 = 0;
                y1 = 1;

                x2 = 1;
                y2 = 0;

                while (b > 0)
                {
                    q = a / b;
                    r = a - q * b;

                    a = b;
                    b = r;

                    x = x2 - q * x1;
                    y = y2 - q * y1;
                    x2 = x1;
                    x1 = x;

                    y2 = y1;
                    y1 = y;

                }

                d = a;
                x = x2;
                y = y2;
            }

        }

        public static List<Tuple<BigInteger,int>> CountConsecutiveSame(List<BigInteger> seq)
        {
            
            seq.Sort();
            List<Tuple<BigInteger,int>> lst = new List<Tuple<BigInteger,int>>();
            BigInteger elem=seq[0];
            int counter=1;
            for (int i = 1; i <seq.Count;i++ )
            {
                if (seq[i] == elem)
                {
                    counter++;
                }
                else
                {
                    lst.Add(new Tuple<BigInteger,int>(elem,counter));
                    elem = seq[i];
                    counter = 1;
                }
            }
            
            lst.Add(new Tuple<BigInteger, int>(elem, counter));

            return lst;
            
        }

        public static List<BigInteger> Factorization(BigInteger num, int start = 2)
        {
            if (num < 1)
                return null;

            BigInteger d = start;
            List<BigInteger> lst = new List<BigInteger>();

            while (num >= d * d)
            {
                if (num % d == 0)
                {
                    lst.Add(d);
                    num = num / d;

                }
                else if (d >= 1024)
                {
                    if (MillerPrimaryTest(num) == true)
                        break;

                    d = RHOPollard(num);
                    lst.Add(d);
                    num = num / d;

                }
                else
                {

                    d = d + 1 + d % 2;
                }

            }


            lst.Add(num);
            return lst;

        }


        public static BigInteger RHOPollard(BigInteger n)
        {
      
            Byte[] b = n.ToByteArray();
            Random rnd = new Random();
            int newLen = rnd.Next(1, b.Length);
            BigInteger x = BigInteger.Abs(new BigInteger(CreateSubArray<byte>(b, 0, newLen)));

            BigInteger y = 1, i = 0, stage = 2;

            while (BigInteger.GreatestCommonDivisor(n, BigInteger.Abs(x - y)) == 1)
            {
                if (i == stage)
                {
                    y = x;
                    stage = stage * 2;
                }
                x = (x * x + 1) % n;
                i += 1;
            }


            return BigInteger.GreatestCommonDivisor(n, BigInteger.Abs(x - y));

        }

        public static BigInteger ChineseRemainder(List<Tuple<BigInteger, BigInteger>> pairs)
        {
            BigInteger k,x,y,d;
            BigInteger a=pairs[0].Item1;
            BigInteger m=pairs[0].Item2;
            for (int i = 1; i < pairs.Count;i++ )
            {
                ExtEuclid(m, pairs[i].Item2, out x, out y, out d);
                k = ((pairs[i].Item1 - a) * x) % pairs[i].Item2;

                if (k < 0)
                    k = pairs[i].Item2 + k;
                a = (a + m * k) % (m * pairs[i].Item2);
                m *= pairs[i].Item2;
            }


            return a;
        
        }

     

        public static T[] CreateSubArray<T>(T[] input,int index,int length)
        {
            T[] newArr = new T[length];

            Array.Copy(input, index,newArr,0,length);

            return newArr;
        
        }

    }
}
