using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents.Actuators;


[RequireComponent(typeof(AutoCONTROL))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SimulationManager))]
//La classe eredita da Agent di MlAgents 
public class AutoParkAgent : Agent
{
    //variabili private per memorizzare i riferimenti ai componenti 
    private Rigidbody _rigitBody;
    private AutoCONTROL _controller;
    private SimulationManager _simulationManager;
    private ActionSegment<float> _lastActions;
    private ParkingLot _nearestLot;

    //i riferimenti ai componenti vengono poi inizializzati 
    public override void Initialize()
    {
        _rigitBody = GetComponent<Rigidbody>();
        _controller = GetComponent<AutoCONTROL>();
        _simulationManager = GetComponent<SimulationManager>();
        _simulationManager.InitializeSimulation();
    }

    //il metodo resetta la simulazione, inizializza nuovamente il simulatore
    //e azzera il riferimento al parcheggio più vicino 
    public override void OnEpisodeBegin()
    {
        _simulationManager.ResetSimulation();
        _simulationManager.InitializeSimulation();
        _nearestLot = null;
    }

    //vengono ricevute le azioni dell'agente e vengono applicate al componente '_controller'
    //l'agente riceve 3 segnali: la quantità di sterzate, di accelerazione e di frenata applicate ad ogni step di simulazione 
    public override void OnActionReceived(ActionBuffers actions)
    {
        var vectorAction = actions.ContinuousActions;
        _lastActions = vectorAction;
        _controller.CurrentSteeringAngle = vectorAction[0];
        _controller.CurrentAcceleration = vectorAction[1];
        _controller.CurrentBrakeTorque = vectorAction[2];
    }

    
    //viene utilizzato per la modalità di gioco manuale 
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // si ottiene l'ActionSegment<float> per le azioni continue (float)
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        continuousActions[2] = Input.GetAxis("Jump");
    }


    //getisce la collisione con altri oggetti nell'ambiente
    private void OnCollisionEnter(Collision other)
    {
        //se l'agente collide con qualche oggetto dell'ambiente specificato riceve una penalità e l'episodio finisce 
        if (other.gameObject.CompareTag("barrier") || other.gameObject.CompareTag("car") ||
            other.gameObject.CompareTag("tree") || other.gameObject.CompareTag("lamp") || other.gameObject.CompareTag("Boundary"))
        {
            AddReward(-0.01f);
            EndEpisode();
        }
    }

    //la funzione raccoglie le osservazioni tratte dall'ambiente 
    public override void CollectObservations(VectorSensor sensor)
    {
        //si controlla che la simulazione sia inizializzata correttamente 
        if (_simulationManager.InitComplete)
        {
            //riferimento al parcheggio vuoto più vicino 
            if(_nearestLot == null)
                _nearestLot = _simulationManager.GetRandomEmptyParkingSlot();

            //il vettore dirToTarget rappresenta la direzione normalizzata dello stallo di parcheggio più vicino all'agente 
            Vector3 dirToTarget = (_nearestLot.transform.position - transform.position).normalized;

            //le osservazioni vengono aggiunte al sensore specificato utilizzando 'AddObservation'
            //posizione normalizzata dell'agente 
            sensor.AddObservation(transform.position.normalized);
            
            //posizione del parcheggio rispetto all'agente 
            sensor.AddObservation(
                this.transform.InverseTransformPoint(_nearestLot.transform.position));
            
            //velocità normalizzata del rigidbody dell'agente trasformata nello spazio locale dell'agente 
            sensor.AddObservation(
                this.transform.InverseTransformVector(_rigitBody.velocity.normalized));
            
            //direzione del parcheggio rispetto all'agente 
            sensor.AddObservation(
                this.transform.InverseTransformDirection(dirToTarget));
            
            //direzione in avanti dell'agente nel suo spazio locale 
            sensor.AddObservation(transform.forward);

            //direzione verso destra dell'agente nel suo spazio locale 
            sensor.AddObservation(transform.right);

            //viene calcolato il dot product tra dirToTarget e la velocità del rigidbody 
            //questo valore rappresenta l'allineamento tra la direzione verso il parcheggio e la velocità dell'agente 
            float velocityAlignment = Vector3.Dot(dirToTarget, _rigitBody.velocity);
            
            //viene applicata una ricompensa per favorire il movimento dell'agente verso il parcheggio 
            AddReward(0.001f * velocityAlignment);
        }
        else
        {
            //se le condizioni non sono soddisfatte vengono aggiunte osservazioni vuote al sensore, per continuare a 
            //mantenere la coerenza delle dimensioni delle osservazioni anche quando non ci sono informazioni valide 
            sensor.AddObservation(new float[18]);
        }
    }

    //il metodo viene chiamato quando l'agente riesce a parcheggiare con successo 
    public IEnumerator JackpotReward(float bonus)
    {
        //viene calcolata una ricompensa bonus al raggiungimento del parcheggio e l'episodio viene terminato 
        AddReward(0.2f + bonus);
        yield return new WaitForEndOfFrame();//la funzione mette in pausa l'esecuzione della coroutine funo alla fine del frame corrente 
        EndEpisode();
    }
}
