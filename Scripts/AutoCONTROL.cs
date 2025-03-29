using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCONTROL : MonoBehaviour
{
    //dichiarazione degli oggetti 'AxleInfo'
    //contengono informazioni su ogni asse del veicolo
    public List<AxleInfo> axleInfos; 

    //dichiarazione delle variabili che contengono i valori del motore, angolo di sterzata e freno 
    public float maxMotorTorque; 
    public float maxSteeringAngle;
    public float maxBrakeTorque;

    //dichiarazione della proprietà che permette di ottenere e impostare i valori correnti per accelerazione
    public float CurrentAcceleration
    {
        //viene restituito il valore della variabile di accelerazione corrente 
        get => m_currentAcceleration;

        //il valore di accelerazione sarà sempre tra 1 e -1 garantendo che il veicolo non superi i limiti di accelerazione
        set => m_currentAcceleration = Mathf.Clamp(value, -1f, 1f);
    }

    //dichiarazione della proprietà che permette di ottenere e impostare i valori della frenata 
    public float CurrentBrakeTorque
    {
        //viene restituita la varabile del freno corrente 
        get => m_currentBrakeTorque;

        //se il valore passato è <= di 0.8 la varaibile viene impostata a 0 altrimenti viene tenuto il valore corrente 
        set
        {
            m_currentBrakeTorque = value <= 0.8f ? 0f : value;
        } 
    }

    //dichiarazione della proprietà che permette di ottenere e impostare i valori dello sterzo  
    public float CurrentSteeringAngle
    {   
        //viene restituito il valore della variabile di angolo di sterzo corrente 
        get => m_currentSteeringAngle;

        //il valore dell'angolo di sterzo sarà sempre tra 1 e -1 garantendo che il veicolo non superi i limiti imposti
        set => m_currentSteeringAngle = Mathf.Clamp(value,-1f,1f);
    }

    //dichiarazione delle tre variabili che tengono i valori correnti di angolo di sterzata, accelerazione e freno 
    private float m_currentSteeringAngle = 0f;
    private float m_currentAcceleration = 0f;
    private float m_currentBrakeTorque = 0f;

    //viene controllato il comportamente di guida del veicolo durante ogni frame 
    public void FixedUpdate() 
    {
        //vegnono calcolati i valori correnti per le tre variabili in base ai valori massimi specificati moltiplicati per i valori correnti delle tre proprietà
        float motor = maxMotorTorque * m_currentAcceleration;
        float steering = maxSteeringAngle * m_currentSteeringAngle;
        float brake = maxBrakeTorque * m_currentBrakeTorque;
    
        foreach (AxleInfo axleInfo in axleInfos) {
            
            //se l'asse è configurato per lo sterzo viene imopstato l'angolo di sterzata delle ruote con il valore 'steering' 
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            //se l'asse è configurato per il motore viene applicata la forza motrice alle ruote utilizzando il valore 'motor'
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            //se l'asse è configurato per il freno viene applicata la forza di frenata alle ruote utilizzando il valore 'brake' 
            if (axleInfo.brake)
            {
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            }
        }
    }
}
//viene utilizzata la classe 'AxleInfo' per rappresentare le informazioni su un asse delle ruote del veicolo 
[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel; //ruota sinistra dell'asse
    public WheelCollider rightWheel; //ruota destra dell'asse 
    public bool motor;   //flag che indica se l'asse è collegato al motore 
    public bool steering; //flag che indica se l'asse è collegato allo sterzo 
    public bool brake;  //flag che indica se l'asse è collegato al freno 
}  