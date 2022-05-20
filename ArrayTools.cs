using System;
using System.Linq;

namespace Pico.Arrays
{
	public static class ArrayTools
    {
        public static int SpecificIndexOf(string text, char character, int number)
        {
            if (text == null || number < 1) return -1;
            int last = 0;
            for (int i = 0; i < number;)
            {
                int index = text.IndexOf(character, last + 1);
                if (index != -1) last = index;
                else return -1;
                i++;
            }
            return last;
        }

        public static bool ArraysAreSame(byte[] arr1, byte[] arr2)  //Add more types?
        {
            if (arr1 == null && arr2 == null) return true;
            if (arr1 == null && arr2 != null) return false;
            if (arr1 != null && arr2 == null) return false;
            if (arr1.Length != arr2.Length) return false;
            for (int i = 0; i < arr1.Length; i++) if (arr1[i] != arr2[i]) return false;
            return true;
        }
        
    }
}