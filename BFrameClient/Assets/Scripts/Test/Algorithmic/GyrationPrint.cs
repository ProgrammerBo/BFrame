using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GyrationPrint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Main();
    }

    // Update is called once per frame
    void Main()
    {
        int n = 5;
        int [,] arr = Calc(n);
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                s.Append(arr[i,j]);
                s.Append("\t");
            }
            s.Append("\n");
        }
        Debug.Log(s.ToString());
    }

    int[,] Calc(int n)
    {
        int[,] arr = new int[n,n];


        int i, j;
        int start = 0;
        int end = n-1;

        int num = 1;
        while (true)
        {

            //上
            for (i = j = start; j <= end; j++)
            {
                arr[i,j] = num++;
            }
            //右
            for (i++,j--; i <= end; i++)
            {
                arr[i, j] = num++;
            }
            //下
            for (i--,j--; j >= start; j--)
            {
                arr[i, j] = num++;
            }
            //左
            for (i--, j++; i > start; i--)
            {
                arr[i, j] = num++;
            }
            start++;
            end--;

            if (start > end)
            {
                break;
            }
        }



        return arr;
    }
}
