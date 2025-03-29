# Autonomous-parking

Questo progetto mira ad addestrare un agente, rappresentato da un'auto virtuale, a eseguire manovre di parcheggio autonomo in un ambiente simulato. Utilizzando tecniche di Reinforcement Learning, l'obiettivo è consentire all'agente di apprendere comportamenti efficienti e sicuri per parcheggiarsi autonomamente.

## Caratteristiche del Progetto

- **Ambiente Simulato:** Creazione di una scena in Unity con 8 stalli di parcheggio, ostacoli (alberi, barriere, lampioni) e trigger collider per monitorare lo stato dei parcheggi.
- **Agente Virtuale:** Un'auto dotata di sensori (Ray Cast) che rilevano ostacoli e spazi liberi, e che può eseguire manovre di accelerazione, frenata e sterzata.
- **Apprendimento per Rinforzo:** Utilizzo dell'algoritmo PPO (Proximal Policy Optimization) per addestrare l'agente con un sistema di ricompense e penalità.
- **Monitoraggio dell'Addestramento:** Impiego di TensorBoard per la visualizzazione dei progressi e l'analisi delle performance dell'agente.

## Strumenti e Tecnologie

- **Unity:** Per la creazione e la gestione dell'ambiente simulato.
- **C#:** Linguaggio utilizzato per sviluppare la logica di gioco e gli script in Unity.
- **ML-Agents:** Libreria di Unity per l'addestramento degli agenti tramite tecniche di apprendimento automatico.
- **Environment Conda:** Ambiente virtuale per gestire le dipendenze dei pacchetti Python necessari all'addestramento.
- **TensorBoard:** Strumento per la visualizzazione grafica dei risultati dell'addestramento.

## Video dimostrazione 
### Addestramento dell'agente dopo 700 mila step 
[Guarda il video](video_700mila_step.mp4)

### Addestramento dell'agente dopo 2.5 milioni di step, risultato finale 
[Guarda il video](video_2.5Milioni_step.mp4)
