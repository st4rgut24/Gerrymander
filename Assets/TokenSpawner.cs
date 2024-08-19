using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject DemChipPrefab;

    [SerializeField]
    private GameObject RepChipPrefab;

    Party lastSPawnedParty = Party.None;

    Result res;
    
    Bounds bounds;

    float spawnInterval = .1f;
    //todo: add graivity to the chipsss

    public void StartSpawner(Result result)
    {
        //ResultsUI resUI = GameObject.Find("Canvas").GetComponent<ResultsUI>();
        res = result;

        StartCoroutine(SpawnTokens());
    }

    Vector2 GetSpawnLocationInScreenSpace()
    {
        float yMinSpawn = 0;
        float yMaxSpawn = 100;

        float xMinSpawn = 0;
        float xMaxSpawn = Screen.width;

        float randomY = Random.Range(yMinSpawn, yMaxSpawn);
        float randomX = Random.Range(xMinSpawn, xMaxSpawn);

        return new Vector2(randomX, randomY);
    }

    IEnumerator SpawnTokens()
    {
        while (true)
        {
            Vector2 screenSpawnLoc = GetSpawnLocationInScreenSpace();
            GameObject spawneedToken;

            if (res == Result.Dem)
            {
                spawneedToken = Instantiate(DemChipPrefab, transform);
            }
            else if (res == Result.Rep)
            {
                spawneedToken = Instantiate(RepChipPrefab, transform);
            }
            else
            {
                //lalternate spawn party chips
                if (lastSPawnedParty == Party.None || lastSPawnedParty == Party.Democrat)
                {
                    spawneedToken = Instantiate(RepChipPrefab, transform);
                    lastSPawnedParty = Party.Republican;
                }
                else
                {
                    spawneedToken = Instantiate(DemChipPrefab, transform);
                    lastSPawnedParty = Party.Democrat;
                }
            }
            spawneedToken.GetComponent<RectTransform>().anchoredPosition = screenSpawnLoc;
            Rigidbody2D rrgbd = spawneedToken.GetComponent<Rigidbody2D>();
            rrgbd.isKinematic = false;

            float randGravScale = Random.Range(1, 30);
            rrgbd.gravityScale = randGravScale;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
