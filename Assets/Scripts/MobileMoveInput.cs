using UnityEngine;

public class MobileMoveInput : MonoBehaviour
{
    public FallingBlockController currentBlock;

    private bool holdLeft;
    private bool holdRight;

    public void LeftDown()
    {
        holdLeft = true;
    }

    public void LeftUp()
    {
        holdLeft = false;
    }

    public void RightDown()
    {
        holdRight = true;
    }

    public void RightUp()
    {
        holdRight = false;
    }

    private void Update()
    {
        if (currentBlock == null) return;

        float move = 0f;

        if (holdLeft)
        {
            move = -1f;
        }

        if (holdRight)
        {
            move = 1f;
        }

        currentBlock.SetHorizontalInput(move);
    }
}
