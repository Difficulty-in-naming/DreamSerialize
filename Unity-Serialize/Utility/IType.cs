namespace DreamSerialize.Utility
{
    public enum IType :uint
    {
        Variable32 = 0,
        Variable64 = 1,
        Fixed32 = 2,
        Fixed64 = 3,
        Dynamic = 4,
        EndGroup = 5,
    }
}