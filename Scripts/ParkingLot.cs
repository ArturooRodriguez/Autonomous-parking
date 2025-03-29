using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingLot : MonoBehaviour
{
    // indica se lo stallo di parcheggio è occupato o vuoto 
    public bool IsOccupied { get; set; } 
    
    //direzione veso cui l'oggetto stallo di parcheggio è rivolto 
    public Vector3 Orientation => transform.forward;

    //componente 'collider' associato allo stallo di parcheggio
    private Collider fullEndCollider;

    private void Awake()
    {
        //viene ottenuto nella funzione 'Awake' e serve per controllare la collisione con altre entità 
        fullEndCollider = GetComponent<Collider>();
    }

    //in questa funzione viene gestita la logica di collisione quando un'altra entità entra nel parcheggio 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent")) //viene verificato se l'entità che entra nel parcheggio ha il tag 'agent'
        {
            //si verifica che il collider del parcheggio e quello dell'agente siano intersecati 
            if (fullEndCollider.bounds.Intersects(other.bounds))
            {
                //se lo stallo di parcheggio non è occupato viene calcolato un bonus di parcheggio 
                if (!IsOccupied)
                {
                    //viene verificata la posizione della macchina una volta entrata nel parcheggio 
                    float bonusfactor = 0.2f;
                    //L'allineamento viene calcolato utilizzando il prodotto scalare tra il vettore "right" dello stallo di parcheggio
                    //e il vettore "up" dell'agente che sta cercando di parcheggiare
                    float alignment = Vector3.Dot(gameObject.transform.right,
                        other.gameObject.transform.up);
                    //se la macchina è rivolta verso il muro ha una ricompensa di 0.2 
                    //se la macchina è rivolta verso la strada ha una ricompensa maggiore, di 0.8
                    if (alignment > 0)  
                        bonusfactor = 0.8f;

                    //il bonus viene calcolato moltiplicando 'bonusfactor' per il valore assoluto dell'allineamento  
                    float bonus = bonusfactor * Mathf.Abs(alignment);

                    //viene avviata la coroutine sull'oggetto genitore dell'entità che sta parcheggiando
                    //la funzione 'jackpot' è definita nella classe 'AutoParkAgent' e fornisce la ricompensa al jackpot per un parcheggio riuscito 
                    StartCoroutine( other.gameObject.transform.parent.GetComponent<AutoParkAgent>().JackpotReward(bonus));
                }
            }
        }
    }

}
