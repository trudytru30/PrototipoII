using UnityEngine;

public class ActionController : MonoBehaviour
{
    private LowerBodyState lowerBodyState = LowerBodyState.Idle;
    private UpperBodyState upperBodyState = UpperBodyState.None;
    private FullBodyState fullBodyState = FullBodyState.None;

    public bool CanUseLowerBody()
    {
        // MODIFICADO: añadida comprobación de lowerBodyState == Idle
        // Antes solo comprobaba fullBodyState, permitiendo encolar movimientos mientras el player ya se movía
        return lowerBodyState == LowerBodyState.Idle &&
               fullBodyState == FullBodyState.None;
    }
 
    public bool CanUseUpperBody()
    {
        // MODIFICADO: añadida comprobación de fullBodyState == None
        // Antes no bloqueaba el upper body si había una acción de cuerpo completo en curso
        return upperBodyState == UpperBodyState.None &&
               fullBodyState == FullBodyState.None;
    }
    
    public bool CanUseFullBody()
    {
        return lowerBodyState == LowerBodyState.Idle || 
               fullBodyState == FullBodyState.None;
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