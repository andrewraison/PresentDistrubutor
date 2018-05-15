using System;
namespace PresentDistributor
{
    public static class ThreeWayQuickSort
    {
        public static void QuickSort(ref int[] keys, int beginIndex, int endIndex)
        {
            if (endIndex <= beginIndex) { return; }

            int startOfPartition=0, endOfPartition=0;

            Partition(ref keys, beginIndex, endIndex,ref startOfPartition,ref endOfPartition);

            QuickSort(ref keys, beginIndex, startOfPartition);
            QuickSort(ref keys, endOfPartition, endIndex);
        }

        private static void Partition(ref int[] keys, int beginIndex, int endIndex, ref int startOfPartition, ref int endOfPartition)
        {
            
        }
    }
}
