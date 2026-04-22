using UnityEngine;

public interface IBookwormParent
{
    public Transform GetBookwormTransform();
    public void SetBookworm(Bookworm bookworm);
    public Bookworm GetBookworm();
    public void ClearBookworm();
    public bool HasBookworm();
}
