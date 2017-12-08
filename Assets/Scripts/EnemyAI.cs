using Assets.Scripts.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
    public float speed = 10;
    private float movementX = 0;

    void Start()
    {
        bestBrain = NeuronalNetwork.Deserialize();
        if (bestBrain.AiScore == 0)
        {
            initAI();
            bestBrain = brain.CloneFullMesh();
        }
        else
        {
            brain = bestBrain.CloneFullMesh();
        }
    }

    void Update()
    {
        survivalTime += Time.deltaTime;
        calcAI();

        float amtToMove = speed * Time.deltaTime;
        transform.Translate(new Vector3(movementX, 0, 0) * amtToMove, Space.World);

        if (transform.position.x > 6.3f)
        {
            //movementX = -movementX;
            transform.position = new Vector3(6.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -6.3f)
        {
            //movementX = -movementX;
            transform.position = new Vector3(-6.3f, transform.position.y, transform.position.z);
        }

        brain.AiScore = survivalTime + ProjectileEvasionScore * 10;

        if (brain.AiScore > bestBrain.AiScore + 100)
        {
            bestBrain = brain.CloneFullMesh();
            bestBrain.Serialize();
        }
    }

    public void Reset()
    {
        brain.AiScore = survivalTime + ProjectileEvasionScore * 10;

        if (brain.AiScore > bestBrain.AiScore)
        {
            bestBrain = brain.CloneFullMesh();
            bestBrain.Serialize();
        }
        this.transform.position = new Vector3(0, 5, 0);
        movementX = 0;
        //initAI();
        generateChildOfBestBrain();
        survivalTime = 0;
        ProjectileEvasionScore = 0;
    }

    private float survivalTime = 0;
    public int ProjectileEvasionScore = 0;
    NeuronalNetwork brain;
    NeuronalNetwork bestBrain;

    void generateChildOfBestBrain()
    {
        brain = bestBrain.CloneFullMesh();
        for (int n = 0; n < brain.HiddenNeurons.Count; n++)
        {
            for (int i = 0; i < brain.HiddenNeurons[n].connections.Count; i++)
            {
                if (Random.Range(0f, 1f) <= 0.5f)
                {
                    brain.HiddenNeurons[n].connections[i].weight = Neuron.Sigmoid(brain.HiddenNeurons[n].connections[i].weight + Random.Range(-1f, 1f));
                }
            }
        }

        for (int n = 0; n < brain.OutputNeurons.Count; n++)
        {
            for (int i = 0; i < brain.OutputNeurons[n].connections.Count; i++)
            {
                if (Random.Range(0f, 1f) <= 0.5f)
                {
                    brain.OutputNeurons[n].connections[i].weight = Neuron.Sigmoid(brain.OutputNeurons[n].connections[i].weight + Random.Range(-1f, 1f));
                }
            }
        }
    }

    void initAI()
    {
        brain = new NeuronalNetwork();

        InputNeuron inXPosAI = new InputNeuron();
        inXPosAI.SetName("IN_XPOS_AI");
        InputNeuron inYPosAI = new InputNeuron();
        inYPosAI.SetName("IN_YPOS_AI");
        brain.AddInputNeuron(inXPosAI);
        brain.AddInputNeuron(inYPosAI);

        InputNeuron inLeftBorderDistance = new InputNeuron();
        inLeftBorderDistance.SetName("IN_LEFT_BORDER_DISTANCE");
        InputNeuron inRightBorderDistance = new InputNeuron();
        inRightBorderDistance.SetName("IN_RIGHT_BORDER_DISTANCE");
        brain.AddInputNeuron(inLeftBorderDistance);
        brain.AddInputNeuron(inRightBorderDistance);

        for (int i = 0; i < 6; i++)
        {
            InputNeuron inXPosProj = new InputNeuron();
            inXPosProj.SetName("IN_XPOS_PROJ" + i);
            brain.AddInputNeuron(inXPosProj);

            InputNeuron inYPosProj = new InputNeuron();
            inYPosProj.SetName("IN_YPOS_PROJ" + i);
            brain.AddInputNeuron(inYPosProj);
        }


        brain.GenerateHiddenNeurons(10);

        WorkingNeuron outMove = new WorkingNeuron();
        outMove.SetName("OUT_MOVE");

        brain.AddOutputNeuron(outMove);

        brain.GenerateFullMesh(); // IN - HIDDEN - OUT "verbinden"

        brain.RandomizeAllWeights();

    }

    void calcAI()
    {
        if (brain == null)
        {
            return;
        }

        var projectiles = sortProjectilesToNearest();

        brain.Invalidate();

        brain.InputNeurons[0].SetValue(transform.position.x);
        brain.InputNeurons[1].SetValue(transform.position.y);
        brain.InputNeurons[2].SetValue(6.3f + transform.position.x);
        brain.InputNeurons[3].SetValue(6.3f - transform.position.x);

        for (int i = 0; i < 6; i++)
        {
            brain.InputNeurons[4 + i].SetValue(projectiles.Length > i ? projectiles[i].transform.position.x : 0);
            brain.InputNeurons[5 + i].SetValue(projectiles.Length > i ? projectiles[i].transform.position.y : 0);
        }

        movementX = brain.OutputNeurons[0].GetValue();

        //Debug.Log("move: " + movementX);
    }

    Vector2 findNearestProjectile()
    {
        var projectiles = FindObjectsOfType<Projectile>();
        Vector2 nearestProjectile = new Vector2(0, 0);
        if (projectiles.Length > 0)
        {
            nearestProjectile = new Vector2(projectiles[0].transform.position.x, projectiles[0].transform.position.y);
            float xDifNearest = Mathf.Abs(transform.position.x - nearestProjectile.x);
            float yDifNearest = Mathf.Abs(transform.position.y - nearestProjectile.y);
            foreach (var proj in projectiles)
            {
                float xDif = Mathf.Abs(transform.position.x - proj.transform.position.x);
                float yDif = Mathf.Abs(transform.position.y - proj.transform.position.y);
                if ((xDif + yDif) < (xDifNearest + yDifNearest))
                {
                    nearestProjectile = new Vector2(proj.transform.position.x, proj.transform.position.y);
                    xDifNearest = xDif;
                    yDifNearest = yDif;
                }
            }
        }
        return nearestProjectile;
    }

    private Projectile[] sortProjectilesToNearest()
    {
        var projectiles = FindObjectsOfType<Projectile>();


        for (int i = 0; i < projectiles.Length; i++)
        {
            float xDif = Mathf.Abs(transform.position.x - projectiles[i].transform.position.x);
            float yDif = Mathf.Abs(transform.position.y - projectiles[i].transform.position.y);
            float dif = xDif + yDif;

            for (int j = i + 1; j < projectiles.Length; j++)
            {
                float xDif2 = Mathf.Abs(transform.position.x - projectiles[j].transform.position.x);
                float yDif2 = Mathf.Abs(transform.position.y - projectiles[j].transform.position.y);
                float dif2 = xDif2 + yDif2;

                if (dif2 < dif)
                {
                    var tmp = projectiles[i];
                    projectiles[i] = projectiles[j];
                    projectiles[j] = tmp;
                }
            }
        }

        return projectiles;
    }




}

