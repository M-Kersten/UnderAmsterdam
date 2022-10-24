using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ioScript : MonoBehaviour
{

    public GameObject ioPipe;
    public Transform parent;

    public int width = 16;//x
    public int height = 3;//y
    public int depth = 23;//z

    private GameObject[,,] wallGridNS;
    private GameObject[,,] wallGridEW;

    private bool placedPipe;

    private int wallSelec;
    private int lineSelec;
    private int columnSelec;

    // Start is called before the first frame update
    void Start()
    {

        wallGridNS = new GameObject[2, height, depth];
        wallGridEW = new GameObject[2, height, width];

        /*
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
        */

        //Adding the two first Input and Output
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

                    if (!wallGridNS[0, lineSelec, columnSelec])
                    {
                        placedPipe = true;
                        wallGridNS[0, lineSelec, columnSelec] = Instantiate(ioPipe, transform.position + new Vector3(width / 4f, lineSelec / 2f, columnSelec / 2f - (depth + 1) / 4f), Quaternion.identity, parent);
                    }

                    break;
                case 1:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, depth);

                    if (!wallGridNS[1, lineSelec, columnSelec])
                    {
                        placedPipe = true;
                        wallGridNS[1, lineSelec, columnSelec] = Instantiate(ioPipe, transform.position + new Vector3(-0.5f - width / 4f, lineSelec / 2f, columnSelec / 2f - (depth - 1) / 4f), Quaternion.Euler(0, 180, 0), parent);
                    }

                    break;
                case 2:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, width);

                    if (!wallGridEW[0, lineSelec, columnSelec])
                    {
                        placedPipe = true;
                        wallGridEW[0, lineSelec, columnSelec] = Instantiate(ioPipe, transform.position + new Vector3(columnSelec / 2f - width / 4, lineSelec / 2f, -(depth + 1) / 4f), Quaternion.Euler(0, 90, 0), parent);
                    }

                    break;
                case 3:
                    lineSelec = Random.Range(0, height);
                    columnSelec = Random.Range(0, width);

                    if (!wallGridEW[1, lineSelec, columnSelec])
                    {
                        placedPipe = true;
                        wallGridEW[1, lineSelec, columnSelec] = Instantiate(ioPipe, transform.position + new Vector3(-0.5f + columnSelec / 2f - width / 4, lineSelec / 2f, (depth + 1) / 4), Quaternion.Euler(0, -90, 0), parent);
                    }

                    break;
            }
        }

        placedPipe = false;

    }
}
