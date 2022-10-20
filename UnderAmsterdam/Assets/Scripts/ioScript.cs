using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ioScript : MonoBehaviour
{

    public GameObject ioPipe;
    public Transform parent;

    public int width = 22;//x
    public int height = 3;//y
    public int depth = 20;//z

    private int[] wallGridN;
    private int[] wallGridS;
    private int[] wallGridE;
    private int[] wallGridW;

    private bool placedPipe;

    private int wallSelec;
    private int lineSelec;
    private int columnSelec;

    // Start is called before the first frame update
    void Start()
    {

        wallGridN = new int[height * width];
        wallGridS = new int[height * width];
        wallGridE = new int[height * depth];
        wallGridW = new int[height * depth];

        for (int i = 0; i < height * width; i++)
        {
            wallGridN[i] = 0;
            wallGridS[i] = 0;
        }
        for (int i = 0; i < height * depth; i++)
        {
            wallGridE[i] = 0;
            wallGridW[i] = 0;
        }

        addPipe();
        addPipe();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            addPipe();
        }
    }

    private void addPipe()
    {

        while (!placedPipe)
        {

            wallSelec = Random.Range(0, 4);

            switch (wallSelec)
            {
                case 0:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, depth);

                    if (wallGridN[lineSelec * columnSelec] == 0)
                    {
                        placedPipe = true;
                        wallGridN[lineSelec * columnSelec] = 1;
                        Instantiate(ioPipe, new Vector3(width / 4f, 0.5f + lineSelec / 2f, columnSelec/2f - (depth + 1) / 4f), Quaternion.identity, parent);
                    }

                    break;
                case 1:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, depth);

                    if (wallGridS[lineSelec * columnSelec] == 0)
                    {
                        placedPipe = true;
                        wallGridS[lineSelec * columnSelec] = 1;
                        Instantiate(ioPipe, new Vector3(-0.5f-width / 4f, 0.5f + lineSelec / 2f, columnSelec/2f - (depth - 1) / 4f), Quaternion.Euler(0, 180, 0), parent);
                    }

                    break;
                case 2:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, width);

                    if (wallGridE[lineSelec * columnSelec] == 0)
                    {
                        placedPipe = true;
                        wallGridE[lineSelec * columnSelec] = 1;
                        Instantiate(ioPipe, new Vector3(columnSelec / 2f - width / 4, 0.5f + lineSelec / 2f, -(depth + 1) / 4f), Quaternion.Euler(0, 90, 0), parent);
                    }

                    break;
                case 3:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, width);

                    if (wallGridW[lineSelec * columnSelec] == 0)
                    {
                        placedPipe = true;
                        wallGridW[lineSelec * columnSelec] = 1;
                        Instantiate(ioPipe, new Vector3(-0.5f + columnSelec / 2f - width / 4, 0.5f + lineSelec/2f, (depth + 1) / 4), Quaternion.Euler(0, -90, 0), parent);
                    }

                    break;
            }
            Debug.Log(lineSelec.ToString() + ":" + columnSelec.ToString());
        }

        placedPipe = false;

    }
}
