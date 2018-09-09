using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable
{
    /*
     * Metodo para marcar el objeto cuando el jugador lo mira
     * */
    void StartUsing(GameObject user);

    void EndUsing();

    void OnStart();
    void OnEnd();
}


public abstract class AbstractUsable : MonoBehaviour, IUsable
{
    protected GameObject user;
    protected bool isUsing;

    public void StartUsing(GameObject user)
    {
        this.user = user;
        isUsing = true;
        OnStart();
    }
    public void EndUsing()
    {
        OnEnd();
        this.user = null;
        isUsing = false;
    }

    public abstract void OnStart();
    public abstract void OnEnd();
}