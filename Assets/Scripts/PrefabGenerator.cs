using UnityEngine;
using Random = UnityEngine.Random;

public class PrefabGenerator: MonoBehaviour
{
    [Header("Prefab Settings")]
    //[Tooltip("The 2D prefab (e.g., a Sprite or UI element) to be instantiated.")]
    public GameObject prefabToGenerate;

    [Header("Generation Settings")]
    //[Tooltip("How many prefabs should be generated when the script starts.")]
    public int numberOfPrefabs = 10;
    //[Tooltip("The area within which the prefabs will be spawned.")]
    public Vector2 spawnArea = new Vector2(10f, 5f);


    [Header("Randomization Settings")]
    //[Tooltip("Minimum and maximum scale for the X and Y axes.")]
    public Vector2 minMaxScale = new Vector2(0.5f, 2.0f);

    private void Awake()
    {
        if(prefabToGenerate == null)
        {
            Debug.LogError($"{nameof(PrefabGenerator)} No prefab assigned.");
        }
    }

    void Start()
    {
        GeneratePrefabs();
    }


    // --- Core Generation Logic ---

    void GeneratePrefabs()
    {
        if(prefabToGenerate == null)
        {
            return;
        }

        float halfX = spawnArea.x * 0.5f;
        float halfY = spawnArea.y * 0.5f;

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            // Position
            Vector2 spawnPosition = new Vector2(
                Random.Range(-halfX, halfX),
                Random.Range(-halfX, halfX)
            );

            // Rotation
            Quaternion rotation = randomizeRotation
                ? Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
                : Quaternion.identity;
        }
    }

        /*
        for(int i = 0; i < numberOfPrefabs; i++)
        {
            // 1. Calculate a random position within the defined spawn area
            float randomX = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            float randomY = Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            // 2. Instantiate the prefab at the random position
            GameObject newPrefab = Instantiate(prefabToGenerate, spawnPosition, Quaternion.identity);

            // Optional: Parent the new prefab to this generator object for scene cleanliness
            newPrefab.transform.SetParent(this.transform);


            // 3. Randomize Size (Scale)

            // Get a random value between the minimum and maximum scale
            float randomScale = Random.Range(minMaxScale.x, minMaxScale.y);

            // Apply the new scale uniformly to all axes (X, Y, and Z)
            newPrefab.transform.localScale = new Vector3(randomScale, randomScale, 1f);


            // 4. Randomize Color

            // Generate a completely random color (R, G, B are all random between 0.0 and 1.0)
            Color randomColor = new Color(
                Random.value, // Random Red value
                Random.value, // Random Green value
                Random.value, // Random Blue value
                1f            // Full Alpha (opacity)
            );

            // Find the SpriteRenderer component and set its color
            SpriteRenderer sr = newPrefab.GetComponent<SpriteRenderer>();

            if(sr != null)
            {
                sr.color = randomColor;
            }
            else
            {
                // This handles cases where the prefab might use other renderers, 
                // like a CanvasGroup or Image component for UI
                Debug.LogWarning("Prefab " + newPrefab.name + " does not have a SpriteRenderer. Skipping color change.");
            }
        }

        Debug.Log($"Successfully generated {numberOfPrefabs} prefabs.");
    }*/
}