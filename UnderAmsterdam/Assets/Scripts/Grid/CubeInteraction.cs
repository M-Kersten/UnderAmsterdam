using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction {Up, Down, Left, Right, Front, Behind };

    [Networked(OnChanged = nameof(OnPipeChanged))] public bool isPiped { get; set; }

    private bool isHover = false;
    private bool pipeTrigger = false;
    private bool rotationTrigger = false;
    private bool stateDownGrabPinch = false;
    private bool stateDownGrabGrip = false;
    private bool isSpawned = false;

    public InputActionProperty input;

    private GameObject Pipe;
    private GridDisplay Grid; //contain an array with all cubes GameObject

    private MeshRenderer CubePreviewRenderer;
    private Renderer[] PipeChildsRenderer;
    private LineRenderer CubeLineRenderer;
    [SerializeField]
    private NetworkObject[] neighbor;

    private void Awake()
    {
        var bindings = new List<string> { "triggerPressed" };
        input.EnableWithDefaultXRBindings(rightBindings: bindings);
    }

    public override void Spawned()
    {

        Pipe = GetComponentsInChildren<Transform>()[1].gameObject; //Always place the Pipe prefab in the top of the children in the cube prefab (Avoid find function)
        Grid = GetComponentInParent<GridDisplay>();

        CubePreviewRenderer = GetComponent<MeshRenderer>();
        PipeChildsRenderer = Pipe.GetComponentsInChildren<Renderer>();
        CubeLineRenderer = GetComponentInChildren<LineRenderer>();

        //Hide all the cubes and pipes
        CubePreviewRenderer.enabled = false;
        CubeLineRenderer.enabled = false;
        Pipe.SetActive(false);

        //Children = new GameObject[6];
        neighbor = new NetworkObject[6];
        FindChildren();

        isSpawned = true;

    }

    public override void FixedUpdateNetwork()
    {
        stateDownGrabGrip = input.action.triggered;

        //Double if with trigger avoid multiple clicks
        if (isHover && !isPiped && (stateDownGrabGrip || Input.GetMouseButton(0))) /*Pipe placing*/
            pipeTrigger = true;           

        if (pipeTrigger && !(stateDownGrabGrip || Input.GetMouseButton(0))) /*Pipe placing*/
        {
            Pipe.SetActive(true);
            isPiped = true;
            pipeTrigger = false;

            CubePreviewRenderer.enabled = false;

            //foreach because recolorablePipe prefab have multiple child so mutiple renderer // Could be changed if new pipes have just one renderer
            foreach (Renderer renderer in PipeChildsRenderer)
                foreach (Material mat in renderer.materials)
                    mat.color = new Color(0, 1, 0, 1f); // Change mat color transparency to 1 when pipe is placed
        }        


        if (isHover && !isPiped && (stateDownGrabPinch || Input.GetMouseButton(1))) /*Pipe Rotate with rhight mouse button or grip controller button */
            rotationTrigger = true;

        if (rotationTrigger && !(stateDownGrabPinch || Input.GetMouseButton(1))) /*Pipe Rotate*/
        {
            Pipe.transform.Rotate(new Vector3(90, 0, 0));
            rotationTrigger = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isSpawned)
            return;
        if (!isPiped && other.CompareTag("RightHand"))
        {
            Pipe.SetActive(true);
            foreach (MeshRenderer material in PipeChildsRenderer)
                foreach (Material PipeMat in material.materials)
                    PipeMat.color = new Color(0, 0.8f, 0, 0.2f); // Preview transparency if the pipe isn't placed
        }

        CubePreviewRenderer.enabled = true;
        CubeLineRenderer.enabled = true;
        isHover = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isSpawned)
            return;
        CubePreviewRenderer.enabled = false;
        CubeLineRenderer.enabled = false;

        if (!isPiped)
            Pipe.SetActive(false);

        isHover = false;
    }

    private void FindChildrenName() 
    {
        string[] splitArray = this.name.Split(" "); //Split the cube name

        int.TryParse(splitArray[1], out int currentX);
        int.TryParse(splitArray[2], out int currentY);
        int.TryParse(splitArray[3], out int currentZ);


        neighbor[(int)Direction.Up] = Grid.GridA[currentX, currentY + 1, currentZ];
        neighbor[(int)Direction.Down] = Grid.GridA[currentX, currentY - 1, currentZ];
        neighbor[(int)Direction.Left] = Grid.GridA[currentX + 1, currentY, currentZ];
        neighbor[(int)Direction.Right] = Grid.GridA[currentX - 1, currentY, currentZ];
        neighbor[(int)Direction.Front] = Grid.GridA[currentX, currentY, currentZ + 1];
        neighbor[(int)Direction.Behind] = Grid.GridA[currentX, currentY, currentZ - 1];
    }
    
    private void FindChildren() 
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, Vector3.up, Color.black, 30f); //to draw the ray during 30 sec
        if (Physics.Raycast(transform.position, Vector3.up , out hit))
            neighbor[(int)Direction.Up] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbor[(int)Direction.Up] = null;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            neighbor[(int)Direction.Down] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbor[(int)Direction.Down] = null;

        if (Physics.Raycast(transform.position, Vector3.left, out hit))
            neighbor[(int)Direction.Left] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbor[(int)Direction.Left] = null;

        if (Physics.Raycast(transform.position, Vector3.right, out hit))
            neighbor[(int)Direction.Right] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbor[(int)Direction.Right] = null;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
            neighbor[(int)Direction.Front] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbor[(int)Direction.Front] = null;

        if (Physics.Raycast(transform.position, Vector3.back, out hit))
            neighbor[(int)Direction.Behind] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbor[(int)Direction.Behind] = null;
    }

    static void OnPipeChanged(Changed<CubeInteraction> changed) // static because o
    {
        Debug.Log($"{Time.time} OnPipeChanged value {changed.Behaviour.isPiped}");
        bool isPipedCurrent = changed.Behaviour.isPiped;

        //Load the old value of isPiped
        changed.LoadOld();

        bool isPipedPrevious = changed.Behaviour.isPiped;

        if (isPipedCurrent && !isPipedPrevious)
            changed.Behaviour.OnPipeRender();
    }

    void OnPipeRender()
    {
        if (!Object.HasInputAuthority && isPiped)
        {
            Pipe.SetActive(true);
            //foreach because recolorablePipe prefab have multiple child so mutiple renderer // Could be changed if new pipes have just one renderer
            foreach (Renderer renderer in PipeChildsRenderer)
                foreach (Material mat in renderer.materials)
                    mat.color = new Color(0, 1, 0, 1f); // Change mat color transparency to 1 when pipe is placed
        }

    }
}
