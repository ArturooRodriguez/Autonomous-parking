using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationManager : MonoBehaviour
{
    // Variabili che possono essere configurate nell'editor di Unity 
    [SerializeField] private List<ParkingLot> parkingLots; // Lista dei parcheggi
    [SerializeField] private List<GameObject> carPrefabs; // Lista dei prefabs delle auto 
    [SerializeField] private AutoParkAgent agent;  // Oggetto AutoParkAgent

    // Tiene traccia delle auto parcheggiate 
    private List<GameObject> parkedCars;

    // Limiti dello spazio in cui vengono posizionate casualmente le auto e l'agente all'inizio della simulazione 
    private float spawnZMin = 0;
    private float spawnZMax = 11;
    private float spawnXMin = -3;
    private float spawnXMax = 0;

    // Restituisce lo stato di completamento dell'inizializzazione della simulazione 
    private bool _initComplete = false;

    public bool InitComplete => _initComplete;

    // Viene inizializzata la lista 'parkedCars'
    private void Awake()
    {
        parkedCars = new List<GameObject>();
    }

    // Inizializza la simulazione 
    public void InitializeSimulation()
    {
        _initComplete = false;
        StartCoroutine(OccupyParkingSlotsWithRandomCars()); // Occupa casualmente gli stalli del parcheggio con le auto 
        RepositionAgentRandom(); // Posiziona casualmente l'agente 
    }

    // Il metodo riposiziona casualmente l'agente all'interno della simulazione 
    public void RepositionAgentRandom()
    {
        if (agent != null)
        {   
            // Imposta la velocità e l'angolo di sterzata su zero 
            agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
            agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            agent.GetComponent<AutoCONTROL>().CurrentSteeringAngle = 0f;
            agent.GetComponent<AutoCONTROL>().CurrentAcceleration = 0f;
            agent.GetComponent<AutoCONTROL>().CurrentBrakeTorque = 0f;

            // Posiziona l'agente in una posizione casuale all'interno dei limiti specificati 
                    // Posiziona l'agente in una posizione casuale all'interno dei limiti specificati 
        int randomRotation = Random.Range(0, 4); // Genera un numero casuale tra 0 e 3

        // Ruota l'agente di 90 gradi sull'asse Y in base al valore casuale generato
        switch(randomRotation)
        {
            case 0:
                agent.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                agent.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                agent.transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            case 3:
                agent.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }
            agent.transform.position = transform.parent.position + new Vector3(Random.Range(spawnXMin,spawnXMax),0.001f,Random.Range(spawnZMin,spawnZMax));
        }
    }

    // Resetta la simulazione
    public void ResetSimulation()
    {
        foreach (GameObject parkedCar in parkedCars)
        {
            // Vengono distrutte le auto parcheggiate
            Destroy(parkedCar);
        }

        foreach (ParkingLot parkingLot in parkingLots)
        {
            // Reimposta lo stato di occupazione dei parcheggi a false 
            parkingLot.IsOccupied = false;
        }
        parkedCars.Clear();
    }

    // La coroutine occupa casualmente gli stalli di parcheggio con auto 
    public IEnumerator OccupyParkingSlotsWithRandomCars()
    {
        foreach (ParkingLot parkingLot in parkingLots)
        {
            // Inizialmente viene azzerato lo stato di tutti i parcheggi 
            parkingLot.IsOccupied = false;
        }
        // Si aspetta un secondo per assicurarsi che gli stalli siano pronti 
        yield return new WaitForSeconds(1);

        // Viene generato un numero casuale che rappresenta il numero di auto da parcheggiare 
        int total = 5;
        for (int i = 0; i < total; i++)
        {
            // Viene selezionato un parcheggio vuoto casuale dalla lista 'parkingLot'
            ParkingLot lot = parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            
            // Se viene trovato un parcheggio vuoto viene istanziata un'auto casuale 
            // e viene posizionata nel parcheggio corrispondente 
            if (lot != null)
            {
                GameObject carInstance = Instantiate(carPrefabs[Random.Range(0, 4)]);

                // Si ottiene la rotazione dell'oggetto 'lot'
                Quaternion lotRotation = lot.transform.rotation;

                // Assegna la stessa rotazione all'auto
                carInstance.transform.rotation = lotRotation;

                carInstance.transform.position = new Vector3(lot.transform.position.x, 0f, lot.transform.position.z);
                // L'auto viene aggiunta alla lista parkedCars e lo stato di occupazione del parcheggio settato a 'true'
                parkedCars.Add(carInstance);
                lot.IsOccupied = true;

                // Se il numero di auto parcheggiate raggiunge o supera il totale viene interrotto il ciclo 
                if (parkedCars.Count >= total)
                    break;
            }
        }
        
        // Viene impostata la variabile a 'true' indicando che la simulazione è stata inizializzata 
        _initComplete = true;
    }

    // Restituisce casualmente uno stallo di parcheggio vuoto 
    public ParkingLot GetRandomEmptyParkingSlot()
    {
        return parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
    }
}
