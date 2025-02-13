using UnityEngine;

public class GridColumn : MonoBehaviour
{

    private void OnMouseEnter()
    {
        Debug.Log("Over " + name);

        int index = GameManagerConnect.Instance.GetGridColumnList().IndexOf(this);
        GameManagerConnect.Instance.HoverOnColumnRpc(index, GameManagerConnect.Instance.GetLocalPlayerType());
    }

    private void OnMouseDown()
    {
        Debug.Log("Down " + name);

        int index = GameManagerConnect.Instance.GetGridColumnList().IndexOf(this);
        GameManagerConnect.Instance.SelectColumnRpc(index, GameManagerConnect.Instance.GetLocalPlayerType());
    }
}
