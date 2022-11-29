using UnityEngine;
using TMPro;
using System.Collections;

public class VisualCountDownLocal : MonoBehaviour
{
    [SerializeField] private GameObject counterObject;
    [SerializeField] private TextMeshProUGUI counter;
    void Start()
    {
        Gamemanager.Instance.CountDownStart.AddListener(delegate { StartCoroutine(StartLocalCountDown()); } );
    }

    private IEnumerator StartLocalCountDown()
    {
        counterObject.SetActive(true);
        counter.text = "3";
        yield return new WaitForSeconds(1);
        counter.text = "2";
        yield return new WaitForSeconds(1);
        counter.text = "1";
        yield return new WaitForSeconds(1);
        counter.text = "Start!";
        yield return new WaitForSeconds(1);
        counterObject.SetActive(false);
    }
}
