using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TCG.Utility
{
    public class Objects
    {
        public static object ConvertToAny(string input, Type target)
        {
            if (target == typeof(string))
                return input;
            // handle common types
            if (target == typeof(int))
                return int.Parse(input);
            if (target == typeof(double))
                return double.Parse(input);
            // handle enums
            if (target.BaseType == typeof(Enum))
                return Enum.Parse(target, input);
            // handle anything with a static Parse(string) function
            var parse = target.GetMethod("Parse", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new[] { typeof(string) }, null);
            if (parse != null)
                return parse.Invoke(null, new object[] { input });
            // handle types with constructors that take a string
            var constructor = target.GetConstructor(new[] { typeof(string) });
            if (constructor != null)
                return constructor.Invoke(new object[] { input });
            else
                throw new ArgumentOutOfRangeException(string.Format("The input {0} could not be converted to type {1}", input, target.BaseType));
        }
        public static object ConvertAny<T>(string input)
        {
            return ConvertToAny(input, typeof(T));
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);

                return obj;
            }
        }

        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);

                return obj;
            }
        }

        public static void ShuffleArray<T>(T[] arr)
        {
            var random = new Random();

            for (int i = arr.Length - 1; i > 0; i--)
            {
                int r = random.Next(0, i);
                T tmp = arr[i];
                arr[i] = arr[r];
                arr[r] = tmp;
            }
        }

        public static void ShuffleList<T>(List<T> list)
        {
            var random = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int r = random.Next(0, i);
                T tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }
    }

    public class Tuple<T0, T1>
    {
        public T0 First { get; set; }
        public T1 Second { get; set; }

        public Tuple(T0 f, T1 s)
        {
            this.First = f;
            this.Second = s;
        }
    }

    public class Triple<T0, T1, T2>
    {
        public T0 First { get; set; }
        public T1 Second { get; set; }
        public T2 Third { get; set; }

        public Triple(T0 f, T1 s, T2 t)
        {
            this.First = f;
            this.Second = s;
            this.Third = t;
        }
    }
}