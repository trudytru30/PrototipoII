using UnityEngine;

public class ActionController : MonoBehaviour
{
    private LowerBodyState lowerBodyState = LowerBodyState.Idle;
    private UpperBodyState upperBodyState = UpperBodyState.None;
    private FullBodyState fullBodyState = FullBodyState.None;

    public bool CanUseLowerBody()
    {
        return fullBodyState == FullBodyState.None;
    }
    
    public bool CanUseUpperBody()
    {
        return upperBodyState == UpperBodyState.None;
    }
    
    public bool CanUseFullBody()
    {
        return lowerBodyState == LowerBodyState.Idle && 
               upperBodyState == UpperBodyState.None && fullBodyState == FullBodyState.None;
    }
    
    // ===== Getters y Setters =====
    public LowerBodyState GetLowerBodyState()
    {
        return lowerBodyState;
    }
    
    public void SetLowerBodyState(LowerBodyState state)
    {
        lowerBodyState = state;
    }
    
    public UpperBodyState GetUpperBodyState()
    {
        return upperBodyState;
    }
    
    public void SetUpperBodyState(UpperBodyState state)
    {
        upperBodyState = state;
    }
    
    public FullBodyState GetFullBodyState()
    {
        return fullBodyState;
    }
    
    public void SetFullBodyState(FullBodyState state)
    {
        fullBodyState = state;
    }
}