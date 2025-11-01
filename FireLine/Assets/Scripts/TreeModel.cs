using UnityEngine;

public class TreeModel
{
    public  GameObject RealTree { get; set; }
    public TreeInstance TreeInstance { get; set; }
    public Vector3 RealWorldPos { get; set; }
    public Quaternion RealWorldRotation { get; set; }


    public void setRealTreeCorrectScale()
    {
        Vector3 baseScale = RealTree.transform.localScale;
        var tmp = new Vector3(
            baseScale.x * TreeInstance.widthScale,
            baseScale.y * TreeInstance.heightScale,
            baseScale.z * TreeInstance.widthScale
        );
        RealTree.transform.localScale = tmp;
    }
}
