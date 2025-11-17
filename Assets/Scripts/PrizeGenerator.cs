using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PrizeGenerator: MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("List of prefabs to randomly select from when generating prizes.")]
    public GameObject[] prefabsToGenerate;

    [Header("Generation Settings")]
    public int numberOfPrefabs = 10;
    public Vector2 spawnRange = new Vector2(5f, 5f);
    public float spawnDelay = 0.05f;
    public float spawnCheckRadius = 0.5f; // Radius used to detect overlap

    [Header("Randomization Settings")]
    public Vector2 minMaxScale = new Vector2(0.5f, 0.65f);
    public bool randomizeColor = true;
    public bool randomizeRotation = false;

    private void Awake()
    {
        if(prefabsToGenerate == null || prefabsToGenerate.Length == 0)
        {
            Debug.LogError($"{nameof(PrizeGenerator)}: No prefabs assigned.");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(GeneratePrizes());
        }
    }

    private IEnumerator GeneratePrizes()
    {
        if(prefabsToGenerate == null || prefabsToGenerate.Length == 0)
            yield break;

        float halfX = spawnRange.x * 0.5f;
        float halfY = spawnRange.y * 0.5f;

        for(int i = 0; i < numberOfPrefabs; i++)
        {
            // Try to find clear spawn position
            Vector2 spawnPosition = Vector2.zero;
            bool foundSpot = false;
            int attempts = 20;

            // Randomly select one prefab from the array
            GameObject selectedPrefab = prefabsToGenerate[Random.Range(0, prefabsToGenerate.Length)];
            if(selectedPrefab == null) continue;

            while(attempts-- > 0)
            {
                // Position
                spawnPosition = new Vector2(
                    transform.position.x + Random.Range(-halfX, halfX),
                    transform.position.y + Random.Range(-halfY, halfY)
                );

                // Overlap check
                if(Physics2D.OverlapCircle(spawnPosition, spawnCheckRadius) == null)
                {
                    foundSpot = true;
                }
            }
            if(!foundSpot)
            {
                Debug.LogWarning($"No clear space found for {selectedPrefab.name}. Skipping...");
            }

            // Rotation
            Quaternion rotation = randomizeRotation
                ? Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
                : Quaternion.identity;

            GameObject obj = Instantiate(selectedPrefab, spawnPosition, rotation, transform);

            // Scale
            float scale = Random.Range(minMaxScale.x, minMaxScale.y);
            obj.transform.localScale = new Vector3(scale, scale, 1f);

            // Color
            if(randomizeColor && obj.TryGetComponent<SpriteRenderer>(out var sr))
            {
                //sr.color = new Color(Random.value, Random.value, Random.value);

                // Bright and fun
                /*
                float min = 0.4f;
                sr.color = new Color(
                    Random.Range(min, 1f),
                    Random.Range(min, 1f),
                    Random.Range(min, 1f)
                );
                */

                // High saturation rainbow like colors                
                float hue = Random.value; // full rainbow
                float saturation = Random.Range(0.7f, 1f);
                float value = Random.Range(0.8f, 1f); // keeps it bright
                sr.color = Color.HSVToRGB(hue, saturation, value);
            }
            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log($"Generated {numberOfPrefabs} prizes.");
    }
}
